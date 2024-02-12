using Bank_Web_Api.Data;
using Bank_Web_Api.Model;
using Bank_Web_Api.Service.Interfaces;
using Bank_Web_Api.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
namespace Bank_Web_Api.Service.Implementation
{
    public class TransactionService : ITransactionService
    {
        private BankingDbContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountService _accountService;

        public TransactionService(BankingDbContext dbContext, ILogger<TransactionService> logger,
            IOptions<AppSettings> settings, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _accountService = accountService;
        }

        public Response CreateNewTransaction(Transaction transaction)
        {
            //create new transaction
            Response response = new Response();
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction Created Successfully!";
            response.Data = null;

            return response;
        }

        public Response FindeTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transaction = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList();//because there are many trans in a day
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction Created Successfully!";
            response.Data = transaction;

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            //make deposit..
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need authentication in UserService, so let's inject IUserService here
            var authUser = _accountService.Authentication(AccountNumber, TransactionPin);
            
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            //so validation passes
            try
            {
                //for deposit, out banksettlementAccount is the source giving money to the user's account
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount= _accountService.GetByAccountNumber(AccountNumber);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update 
                if((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful!";
                    response.Data = null;
                }
                else
                {
                    //so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed!";
                    response.Data = null;
                }
                   

            }
            catch(Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            //set other props of transactio here 
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = 
                $"NEW TRANSACTION FROM SOURCE => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                $"TO DESTINATION ACCOUNT => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => " +
                $"{transaction.TransactionType} TRANSACTION STATUS => " +
                $"{transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            //make Withdrawal..
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need authentication in UserService, so let's inject IUserService here
            var authUser = _accountService.Authentication(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            //so validation passes
            try
            {
                //for deposit, out banksettlementAccount is the destination getting money from the user's account
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update 
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful!";
                    response.Data = null;
                }
                else
                {
                    //so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed!";
                    response.Data = null;
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            //set other props of transactio here 
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars =
                 $"NEW TRANSACTION FROM SOURCE => " +
                 $"{JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                 $"TO DESTINATION ACCOUNT => " +
                 $"{JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                 $"ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
                 $"{JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => " +
                 $"{transaction.TransactionType} TRANSACTION STATUS => " +
                 $"{transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;

        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            //make Withdrawal..
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            //first check that user - account owner is valid,
            //we'll need authentication in UserService, so let's inject IUserService here
            var authUser = _accountService.Authentication(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            //so validation passes
            try
            {
                //for deposit, out banksettlementAccount is the destination getting money from the user's account
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementAccount);

                //now let's update their account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update 
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) &&
                    (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    //so transaction is successful
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction Successful!";
                    response.Data = null;
                }
                else
                {
                    //so transaction is unsuccessful
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction Failed!";
                    response.Data = null;
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            //set other props of transactio here 
            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars =
                $"NEW TRANSACTION FROM SOURCE => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} " +
                $"TO DESTINATION ACCOUNT => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} " +
                $"ON DATE => {transaction.TransactionDate} FOR AMOUNT => " +
                $"{JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => " +
                $"{transaction.TransactionType} TRANSACTION STATUS => " +
                $"{transaction.TransactionStatus}";

            //All done, let's commit to db
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return response;

        }
    }
}
