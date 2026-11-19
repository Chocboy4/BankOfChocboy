using System.Security.Cryptography;

namespace  BankOfChocboy.Services;

public class AccountNumberGenerator
{
    public string Generate()
    {
        int number = RandomNumberGenerator.GetInt32(
            0, 1_000_000_000);

        return number.ToString("D10");
    }
}