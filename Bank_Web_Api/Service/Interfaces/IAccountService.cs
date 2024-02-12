using Bank_Web_Api.Model;

namespace Bank_Web_Api.Service.Interfaces
{
    public interface IAccountService
    {
        Account Authentication(string AccountNumber, string Pin);
        Account Create(Account account, string Pin, string ConfirmPin);
        Account GetById(int Id);
        Account GetByAccountNumber(string AccountNumber);
        IEnumerable<Account> GetAllAccount();
        void Update(Account account, string Pin = null);
        void Delete(int Id);
    }
}
