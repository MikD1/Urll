using Urll.Core;

namespace Urll.Tests;

[TestClass]
public class LinkTests
{
    [TestMethod]
    public void Create_ValidInput_ReturnsLink()
    {
        string url = "https://example.com";
        string code = "valid_code";

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(new string[0], validationResult);
        Assert.AreEqual(url, result.Url);
        Assert.AreEqual(code, result.Code);
    }

    [TestMethod]
    public void Create_InvalidUrl_ReturnsNull()
    {
        string url = "invalid_url";
        string code = "valid_code";

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Invalid Url format.");
    }

    [TestMethod]
    public void Create_InvalidCode_ReturnsNull()
    {
        string url = "https://example.com";
        string code = "invalid code!";

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Code can only contain alphanumeric characters, underscores, and dashes.");
    }

    [TestMethod]
    public void Create_EmptyUrl_ReturnsNull()
    {
        string url = string.Empty;
        string code = "valid_code";

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Url cannot be empty.");
    }

    [TestMethod]
    public void Create_LongUrl_ReturnsNull()
    {
        string url = "https://" + new string('a', 1001);
        string code = "valid_code";

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Url length cannot be greater than 1000.");
    }

    [TestMethod]
    public void Create_EmptyCode_ReturnsNull()
    {
        string url = "https://example.com";
        string code = string.Empty;

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Code cannot be empty.");
    }

    [TestMethod]
    public void Create_LongCode_ReturnsNull()
    {
        string url = "https://example.com";
        string code = new string('a', 51);

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Code length cannot be greater than 50.");
    }

    [TestMethod]
    public void Create_LongCodeAndEmptyUrl_ReturnsNull()
    {
        string url = string.Empty;
        string code = new string('a', 51);

        Link? result = Link.Create(url, code, out string[] validationResult);

        Assert.IsNull(result);
        CollectionAssert.Contains(validationResult, "Url cannot be empty.");
        CollectionAssert.Contains(validationResult, "Code length cannot be greater than 50.");
    }
}
