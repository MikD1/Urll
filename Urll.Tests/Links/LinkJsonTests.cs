using System.Text.Json;
using Urll.Links;

namespace Urll.Tests.Links;

[TestClass]
public class LinkJsonTests
{
    [TestMethod]
    public void Link_SerializeAndDeserialize()
    {
        Link? link = Link.Create("http://urll.dev", "url", out string[] _);

        string json = JsonSerializer.Serialize(link);
        Link? deserializedLink = JsonSerializer.Deserialize<Link>(json);

        Assert.IsNotNull(deserializedLink);
        Assert.AreNotEqual(default(DateTime), link?.Created);
        Assert.AreEqual("http://urll.dev", link?.Url);
        Assert.AreEqual("url", link?.Code);
    }
}
