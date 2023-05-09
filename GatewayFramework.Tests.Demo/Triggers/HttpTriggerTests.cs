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
        Assert.AreEqual(body2, body1);
    }

    [TestMethod]
    public async Task Aggregator_ReturnsFirstAndSecond()
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
        Assert.AreEqual($"{{\"first\":{firstBodyString},\"second\":{secondBodyString}}}", body);
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
        Assert.AreEqual(JsonSerializer.Serialize(new ExampleResponse()), body);
    }
}