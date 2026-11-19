namespace BankOfChocboy.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword(Account account, string password);
    bool VerifyPassword(Account account, string hashedPassword, string password);
}
