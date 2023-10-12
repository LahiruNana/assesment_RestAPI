# installed following dependencies

dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package System.Net.Http

# build the projct
dotnet build

# Run tests
dotnet test