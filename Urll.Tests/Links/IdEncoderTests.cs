using Urll.Links;

namespace Urll.Tests.Links;

[TestClass]
public class IdEncoderTests
{
    [DataTestMethod]
    [DataRow(0, "wH")]
    [DataRow(1, "Z7")]
    [DataRow(4242007, "Fc7lV")]
    [DataRow(9782763, "5jq-Z")]
    [DataRow(15000000, "wAw49")]
    [DataRow(2147483647, "LjG66Gg")]
    public void Encode_Id_EncodesCorrectly(int id, string expectedCode)
    {
        IdEncoder encoder = new();

        string code = encoder.Encode(id);

        Assert.AreEqual(expectedCode, code);
    }

    [TestMethod]
    public void EncodeAndDecode_ReturnsSameValue()
    {
        IdEncoder encoder = new();

        for (int i = 0; i < 100000; ++i)
        {
            string code = encoder.Encode(i);
            int id = encoder.Decode(code);
            Assert.AreEqual(i, id);
        }
    }
}
