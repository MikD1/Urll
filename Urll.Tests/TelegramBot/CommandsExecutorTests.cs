using Moq;
using Refit;
using Urll.Links.Contracts;
using Urll.Links.Contracts.Dto;
using Urll.TelegramBot;

namespace Urll.Tests.TelegramBot;

[TestClass]
public class CommandsExecutorTests
{
    [TestMethod]
    public async Task Execute_StartCommand_ReturnsHelpText()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);

        string result = await commandsExecutor.Execute("/start");

        Assert.AreEqual(@"Commands
{url} - Add Link with generated code
{url} {code} - Add Link
/start - Show help
/list - List all Links
/get {code} - Return Link by code
/delete {code} - Delete Link", result);
    }

    [TestMethod]
    public async Task Execute_ListCommand_Success_ReturnsListOfLinks()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        LinkDto[] links = new LinkDto[]
        {
            new(DateTime.UtcNow, "http://example.com/1", "code1"),
            new(DateTime.UtcNow, "http://example.com/2", "code2")
        };

        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<LinkDto[]> expectedResponse = new(httpResponseMessage, links, new RefitSettings());
        linksClientMock.Setup(client => client.GetAll()).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/list");

        Assert.IsTrue(result.Contains("code1: http://example.com/1"));
        Assert.IsTrue(result.Contains("code2: http://example.com/2"));
    }

    [TestMethod]
    public async Task Execute_ListCommand_NoLinks_ReturnsMessage()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        LinkDto[] links = Array.Empty<LinkDto>();
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<LinkDto[]> expectedResponse = new(httpResponseMessage, links, new RefitSettings());
        linksClientMock.Setup(client => client.GetAll()).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/list");

        Assert.AreEqual("No Links found", result);
    }

    [TestMethod]
    public async Task Execute_GetCommand_Success_ReturnsLinkUrl()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        LinkDto link = new(DateTime.UtcNow, "http://example.com/1", "code1");
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<LinkDto> expectedResponse = new(httpResponseMessage, link, new RefitSettings());
        linksClientMock.Setup(client => client.Get("code1")).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/get code1");

        Assert.AreEqual("http://example.com/1", result);
    }

    [TestMethod]
    public async Task Execute_AddCommandWithOneArg_Success_ReturnsCode()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        LinkDto link = new(DateTime.UtcNow, "http://example.com/1", "code1");
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<LinkDto> expectedResponse = new(httpResponseMessage, link, new RefitSettings());
        linksClientMock.Setup(client => client.Add(It.IsAny<LinkAddDto>())).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("http://example.com/1");

        Assert.AreEqual("code1", result);
    }

    [TestMethod]
    public async Task Execute_AddCommandWithTwoArgs_Success_ReturnsCode()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        LinkDto link = new(DateTime.UtcNow, "http://example.com/1", "code1");
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<LinkDto> expectedResponse = new(httpResponseMessage, link, new RefitSettings());
        linksClientMock.Setup(client => client.Add(It.IsAny<LinkAddDto>())).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("http://example.com/1 code1");

        Assert.AreEqual("code1", result);
    }

    [TestMethod]
    public async Task Execute_DeleteCommand_Success_ReturnsSuccessMessage()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.OK);
        ApiResponse<object> expectedResponse = new(httpResponseMessage, null, new RefitSettings());
        linksClientMock.Setup(client => client.Delete("code1")).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/delete code1");

        Assert.AreEqual("Link deleted", result);
    }

    [TestMethod]
    public async Task Execute_InvalidCommand_ReturnsCommandNotFound()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);

        string result = await commandsExecutor.Execute("/invalid");

        Assert.AreEqual("Command not found", result);
    }

    [TestMethod]
    public async Task Execute_ListCommand_FailedApiResponse_ReturnsErrorMessage()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.BadRequest);
        ApiException apiException = await ApiException.Create("API Error", new HttpRequestMessage(), HttpMethod.Get, httpResponseMessage, new RefitSettings());
        ApiResponse<LinkDto[]> expectedResponse = new ApiResponse<LinkDto[]>(httpResponseMessage, null, new RefitSettings(), apiException);
        linksClientMock.Setup(client => client.GetAll()).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/list");

        Assert.AreEqual("API Error", result);
    }

    [TestMethod]
    public async Task Execute_GetCommand_FailedApiResponse_ReturnsErrorMessage()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.BadRequest);
        ApiException apiException = await ApiException.Create("API Error", new HttpRequestMessage(), HttpMethod.Get, httpResponseMessage, new RefitSettings());
        ApiResponse<LinkDto> expectedResponse = new(httpResponseMessage, null, new RefitSettings(), apiException);
        linksClientMock.Setup(client => client.Get("code1")).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/get code1");

        Assert.AreEqual("API Error", result);
    }

    [TestMethod]
    public async Task Execute_DeleteCommand_FailedApiResponse_ReturnsErrorMessage()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);
        HttpResponseMessage httpResponseMessage = new(System.Net.HttpStatusCode.BadRequest);
        ApiException apiException = await ApiException.Create("API Error", new HttpRequestMessage(), HttpMethod.Get, httpResponseMessage, new RefitSettings());
        ApiResponse<object> expectedResponse = new(httpResponseMessage, null, new RefitSettings(), apiException);
        linksClientMock.Setup(client => client.Delete("code1")).ReturnsAsync(expectedResponse);

        string result = await commandsExecutor.Execute("/delete code1");

        Assert.AreEqual("API Error", result);
    }

    [TestMethod]
    public async Task Execute_AddCommand_InvalidArgs_ReturnsHelpText()
    {
        Mock<ILinksClient> linksClientMock = new();
        CommandsExecutor commandsExecutor = new(linksClientMock.Object);

        string result = await commandsExecutor.Execute("arg1 arg2 arg3");

        Assert.AreEqual(@"Commands
{url} - Add Link with generated code
{url} {code} - Add Link
/start - Show help
/list - List all Links
/get {code} - Return Link by code
/delete {code} - Delete Link", result);
    }
}
