using Bank_Web_Api.Data;
using Bank_Web_Api.Model;
using Bank_Web_Api.Service.Interfaces;
using System.Net.NetworkInformation;
using System.Text;

namespace Bank_Web_Api.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private BankingDbContext _dbContext;
        public AccountService(BankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Account Authentication(string AccountNumber, string Pin)
        {
            //does account exist for that number
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;
            //ok we have a match
            //verify pinHash
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;
            //ok so Authentication is passed
            return account;

        }
        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentException("Pin");
            //now let's verify pin
            using(var hmac=new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for(int i=0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;

        }
        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            //Create a new account
            if (_dbContext.Accounts.Any(e => e.Email == account.Email))
                throw new ApplicationException("An account already exists with this email ");
            if (!Pin.Equals(ConfirmPin))
                throw new ArgumentException("Pins does not match", "Pin");
            //now all validation passes,let create account
            //we are hashing /encrypting pin first
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);//method
            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            //all good add new account to db
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return account;
        }
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }
        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if(account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }

        }

        public IEnumerable<Account> GetAllAccount()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null) return null;
            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == Id).FirstOrDefault();
            if (account == null) return null;
            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            //update more tasky
            var accountToBeUpdated = _dbContext.Accounts.Where(x => x.Email == account.Email).SingleOrDefault();

            if (accountToBeUpdated == null) throw new ArgumentException("Account does not exists");
            //if it is exist, let's listen to the user wanting to change any of his/her property
            //if (!string.IsNullOrWhiteSpace(account.Email))
            //{
            //    //this means the user wash to change email
            //    //check if one he is changing to is not already token
            //    if (_dbContext.Accounts.Any(x => x.Email == account.Email)) 
            //        throw new ArgumentException("The Email" + account.Email + "already exists");
            //    //else change email
            //    accountToBeUpdated.Email = account.Email;
            //}
            //actually we want the user to be able to change only Email,phoneNumber,and Pin
            //PhoneNumber
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber))
                    throw new ArgumentException("The Phone Number " + account.PhoneNumber + "arleady exists");
                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);
                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }
            accountToBeUpdated.DateLastUpdated = DateTime.Now;
            //now persist this update to db
            _dbContext.Accounts.Update(accountToBeUpdated);
            _dbContext.SaveChanges();
        }
    }
}
