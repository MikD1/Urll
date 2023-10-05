using Sqids;

namespace Urll.Links;

public class IdEncoder
{
    public IdEncoder()
    {
        _encoder = new SqidsEncoder<int>(new()
        {
            Alphabet = "_gzbU690GBhMNR2TPQknE-1lxa7dVYoecJHjF3qAyDOXS4IrpZwmui5CKfL8tsWv",
        });
    }

    public string Encode(int id)
    {
        return _encoder.Encode(id);
    }

    public int Decode(string code)
    {
        return _encoder.Decode(code).Single();
    }

    private readonly SqidsEncoder<int> _encoder;
}
