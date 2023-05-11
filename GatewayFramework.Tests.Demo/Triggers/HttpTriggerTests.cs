using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Utils.GatewayFramework.Http;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Semifinals.Utils.GatewayFramework;

[TestClass]
public class HttpTriggerTests
{
    [TestMethod]
    public async Task Passthrough_ReturnsSameAsFirst()
    {
        // Arrange
        Request request2 = new("GET", "http://localhost:7249/first");
        Client get2 = new(request2);
        HttpResponseMessage res2 = await get2.SubmitAsync();
        string body2 = await res2.Content.ReadAsStringAsync();

        Request request1 = new("GET", "http://localhost:7249/passthrough");
        Client get1 = new(request1);

        // Act
        HttpResponseMessage res1 = await get1.SubmitAsync();
        string body1 = await res1.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual((int)res2.StatusCode, (int)res1.StatusCode);
        Assert.AreEqual(body2, body1);
    }

    [TestMethod]
    public async Task Passthrough201_ReturnsCreatedObject()
    {
        // Arrange
        Request request2 = new("GET", "http://localhost:7249/created201");
        Client get2 = new(request2);
        HttpResponseMessage res2 = await get2.SubmitAsync();
        string body2 = await res2.Content.ReadAsStringAsync();
        string? location2 = res2.Headers.Location?.ToString();

        Request request1 = new("GET", "http://localhost:7249/passthrough201");
        Client get1 = new(request1);

        // Act
        HttpResponseMessage res1 = await get1.SubmitAsync();
        string body1 = await res1.Content.ReadAsStringAsync();
        string? location1 = res1.Headers.Location?.ToString();

        // Assert
        Assert.AreEqual((int)res2.StatusCode, (int)res1.StatusCode);
        Assert.AreEqual(body2, body1);
        Assert.AreEqual(location2, location1);
    }

    [TestMethod]
    public async Task Passthrough204_ReturnsEmptyResultWithCorrectStatusCode()
    {
        // Arrange
        Request request2 = new("GET", "http://localhost:7249/nocontent204");
        Client get2 = new(request2);
        HttpResponseMessage res2 = await get2.SubmitAsync();

        Request request1 = new("GET", "http://localhost:7249/passthrough204");
        Client get1 = new(request1);

        // Act
        HttpResponseMessage res1 = await get1.SubmitAsync();

        // Assert
        Assert.AreEqual((int)res2.StatusCode, (int)res1.StatusCode);
    }

    [TestMethod]
    public async Task Passthrough400_ReturnsErrorObject()
    {
        // Arrange
        Request request2 = new("GET", "http://localhost:7249/error400");
        Client get2 = new(request2);
        HttpResponseMessage res2 = await get2.SubmitAsync();
        string body2 = await res2.Content.ReadAsStringAsync();

        Request request1 = new("GET", "http://localhost:7249/passthrough400");
        Client get1 = new(request1);

        // Act
        HttpResponseMessage res1 = await get1.SubmitAsync();
        string body1 = await res1.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual((int)res2.StatusCode, (int)res1.StatusCode);
        Assert.AreEqual(body2, body1);
    }

    [TestMethod]
    public async Task Aggregate_ReturnsFirstAndSecond()
    {
        // Arrange
        Request firstReq = new("GET", "http://localhost:7249/first");
        Client firstClient = new(firstReq);
        HttpResponseMessage firstRes = await firstClient.SubmitAsync();
        string firstBodyString = await firstRes.Content.ReadAsStringAsync();

        Request secondReq = new("GET", "http://localhost:7249/second");
        Client secondClient = new(secondReq);
        HttpResponseMessage secondRes = await secondClient.SubmitAsync();
        string secondBodyString = await secondRes.Content.ReadAsStringAsync();

        Request req = new("GET", "http://localhost:7249/aggregate");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.IsTrue(body.Contains(firstBodyString));
        Assert.IsTrue(body.Contains(secondBodyString));
    }

    [TestMethod]
    public async Task Aggregate_HandlesMixtureWithCreated()
    {
        // Arrange
        Request firstReq = new("GET", "http://localhost:7249/first");
        Client firstClient = new(firstReq);
        HttpResponseMessage firstRes = await firstClient.SubmitAsync();
        string firstBodyString = await firstRes.Content.ReadAsStringAsync();

        Request secondReq = new("GET", "http://localhost:7249/created201");
        Client secondClient = new(secondReq);
        HttpResponseMessage secondRes = await secondClient.SubmitAsync();
        string secondBodyString = await secondRes.Content.ReadAsStringAsync();

        Request req = new("GET", "http://localhost:7249/aggregate201");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.IsTrue(body.Contains(firstBodyString));
        Assert.IsTrue(body.Contains(secondBodyString));
        Assert.IsTrue(body.Contains(((int)secondRes.StatusCode).ToString()));
    }

    [TestMethod]
    public async Task Aggregate_HandlesMixtureWithNoContent()
    {
        // Arrange
        Request firstReq = new("GET", "http://localhost:7249/first");
        Client firstClient = new(firstReq);
        HttpResponseMessage firstRes = await firstClient.SubmitAsync();
        string firstBodyString = await firstRes.Content.ReadAsStringAsync();

        Request secondReq = new("GET", "http://localhost:7249/nocontent204");
        Client secondClient = new(secondReq);
        HttpResponseMessage secondRes = await secondClient.SubmitAsync();

        Request req = new("GET", "http://localhost:7249/aggregate204");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.IsTrue(body.Contains(firstBodyString));
        Assert.IsTrue(body.Contains(((int)secondRes.StatusCode).ToString()));
    }

    [TestMethod]
    public async Task Aggregate_HandlesMixtureWithBadRequest()
    {
        // Arrange
        Request firstReq = new("GET", "http://localhost:7249/first");
        Client firstClient = new(firstReq);
        HttpResponseMessage firstRes = await firstClient.SubmitAsync();
        string firstBodyString = await firstRes.Content.ReadAsStringAsync();

        Request secondReq = new("GET", "http://localhost:7249/error400");
        Client secondClient = new(secondReq);
        HttpResponseMessage secondRes = await secondClient.SubmitAsync();
        string secondBodyString = await secondRes.Content.ReadAsStringAsync();

        Request req = new("GET", "http://localhost:7249/aggregate400");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.IsTrue(body.Contains(firstBodyString));
        Assert.IsTrue(body.Contains(secondBodyString));
        Assert.IsTrue(body.Contains(((int)secondRes.StatusCode).ToString()));
    }

    [TestMethod]
    public async Task First_ReturnsExampleResponse()
    {
        // Arrange
        Request req = new("GET", "http://localhost:7249/first");
        Client client = new(req);


        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.AreEqual(JsonSerializer.Serialize(new ExampleResponse()), body);
    }

    [TestMethod]
    public async Task Second_ReturnsExampleResponse()
    {
        // Arrange
        Request req = new("GET", "http://localhost:7249/second");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(200, (int)res.StatusCode);
        Assert.AreEqual(JsonSerializer.Serialize(new ExampleResponse()), body);
    }

    [TestMethod]
    public async Task Created201_Returns201()
    {
        // Arrange
        Request req = new("GET", "http://localhost:7249/created201");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();
        string? location = res.Headers.Location?.ToString();

        // Assert
        Assert.AreEqual(201, (int)res.StatusCode);
        Assert.AreEqual(JsonSerializer.Serialize(new ExampleResponse()), body);
        Assert.AreEqual("https://example.com/", location);
    }

    [TestMethod]
    public async Task NoContent204_Returns204()
    {
        // Arrange
        Request req = new("GET", "http://localhost:7249/nocontent204");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();

        // Assert
        Assert.AreEqual(204, (int)res.StatusCode);
    }

    [TestMethod]
    public async Task Error400_Returns400()
    {
        // Arrange
        Request req = new("GET", "http://localhost:7249/error400");
        Client client = new(req);

        // Act
        HttpResponseMessage res = await client.SubmitAsync();
        string body = await res.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(400, (int)res.StatusCode);
        Assert.AreEqual(JsonSerializer.Serialize(new string[] { "error1", "error2" }), body);
    }
}