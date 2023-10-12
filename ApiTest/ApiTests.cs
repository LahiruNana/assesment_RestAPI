using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

[TestFixture]
public class ApiTests
{
    private HttpClient _httpClient;
    private const string BaseUrl = "https://api.restful-api.dev/";

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    [Test]
    public async Task GetRequest_AllObjects_ReturnsSuccess()
    {
        // Arrange
        var endpoint = "/objects";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<List<MyApiResponseModel>>(jsonResponse);

        // Assert specific properties
        Assert.NotNull(responseData);
        Assert.That(responseData.Count, Is.GreaterThan(1));
        Assert.That(responseData.Count, Is.EqualTo(13));
    }

    [Test]
    public async Task GetRequest_SingleObject_ReturnsSuccess()
    {
        // Arrange
        var endpoint = "/objects/1";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<MyApiResponseModel>(jsonResponse);

        // Assert specific properties
        Assert.NotNull(responseData);
        Assert.AreEqual("1", responseData.Id);
        Assert.AreEqual("Google Pixel 6 Pro", responseData.Name);
    }

    [Test]
    public async Task PostRequest_AddObject_ReturnsSuccess()
    {
        // Arrange
        var endpoint = "/objects";

        // Assign values
        var request = new MyApiResponseModel()
        {
            Name = "Test Name",
            Data = new Data()
            {
                Year = 2000,
                Price = 3000,
                CPUModel = "I7",
                HardDiskSize = "20 TB",
                Color = "Green"
            }
        };

        var response = await _httpClient.PostAsJsonAsync<MyApiResponseModel>(endpoint, request);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<MyApiResponseModel>(jsonResponse);

        // Assert specific properties
        Assert.NotNull(responseData);
        Assert.AreEqual("Test Name", responseData.Name);
        Assert.NotNull(responseData.Data);
    }

    [Test]
    public async Task PutRequest_UpdateObject_ReturnsSuccess()
    {
        // Arrange
        var endpoint = "/objects/1";

        // Act
        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var getResDeserialize = JsonConvert.DeserializeObject<MyApiResponseModel>(jsonResponse);
        getResDeserialize.Name = "Updated Name";
        var serializedobj = System.Text.Json.JsonSerializer.Serialize(getResDeserialize);

        //var content = new StringContent(serializedobj, Encoding.UTF8, "application/json");
        var jsonResponse1 = await _httpClient.PutAsJsonAsync(endpoint, serializedobj);
        jsonResponse1.EnsureSuccessStatusCode();
        var putResponse = await jsonResponse1.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<MyApiResponseModel>(putResponse);

        // Assert specific properties
        Assert.NotNull(responseData);
        Assert.AreEqual("Updated Name", responseData.Name);
    }

    [Test]
    public async Task DelRequest_UpdateObject_ReturnsSuccess()
    {
        // Arrange
        var endpoint = "/objects/6";

        // Act
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var getResDeserialize = JsonConvert.DeserializeObject<MyDeleteResponse>(jsonResponse);

        // Assert specific properties
        Assert.NotNull(getResDeserialize);
        Assert.AreEqual("Object with id = 6, has been deleted.", getResDeserialize.Message);
    }

    [TearDown]
    public void Cleanup()
    {
        _httpClient.Dispose();
    }
}

public class MyApiResponseModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Data
{
    [JsonPropertyName("year")]
    public int Year { get; set; }
    [JsonPropertyName("price")]
    public float Price { get; set; }
    [JsonPropertyName("CPU model")]
    public string CPUModel { get; set; }
    [JsonPropertyName("Hard disk size")]
    public string HardDiskSize { get; set; }
    [JsonPropertyName("color")]
    public string Color { get; set; }
}

public class MyDeleteResponse
{
    public string Message { get; set; }
}