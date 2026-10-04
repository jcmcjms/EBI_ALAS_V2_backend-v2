namespace EBI.ALAS.Api.Features.Auth;

public interface ITempPasswordGenerator
{
    string Generate();
}

public sealed class TempPasswordGenerator : ITempPasswordGenerator
{
    private static readonly string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private static readonly string Lower = "abcdefghijkmnpqrstuvwxyz";
    private static readonly string Digits = "23456789";
    private static readonly string Special = "!?*.";
    private static readonly string All = Upper + Lower + Digits + Special;

    public string Generate()
    {
        var random = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        random.GetBytes(bytes);

        var password = new char[12];
        password[0] = Upper[bytes[0] % Upper.Length];
        password[1] = Lower[bytes[1] % Lower.Length];
        password[2] = Digits[bytes[2] % Digits.Length];
        password[3] = Special[bytes[3] % Special.Length];

        random.GetBytes(bytes);
        for (int i = 4; i < 12; i++)
        {
            random.GetBytes(bytes);
            password[i] = All[bytes[0] % All.Length];
        }

        // Shuffle
        for (int i = password.Length - 1; i > 0; i--)
        {
            random.GetBytes(bytes);
            var j = bytes[0] % (i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}