using System.Text.Json;
using Urll.Core;

namespace Urll.Tests;

[TestClass]
public class LinkJsonTests
{
    [TestMethod]
    public void Link_SerializeAndDesirialize()
    {
        Link? link = Link.Create("http://urll.dev", "url", out string[] _);

        string json = JsonSerializer.Serialize(link);
        Link? desirializedLink = JsonSerializer.Deserialize<Link>(json);

        Assert.IsNotNull(desirializedLink);
        Assert.AreNotEqual(default(DateTime), link?.Created);
        Assert.AreEqual("http://urll.dev", link?.Url);
        Assert.AreEqual("url", link?.Code);
    }
}
