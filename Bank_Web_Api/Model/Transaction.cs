using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bank_Web_Api.Model
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionUniqueReference { get; set; }//This we will generate in every instance of this class
        public decimal TransactionAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }//this is an enum
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success);//this depende on value of TransactionStatus
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public string TransactionParticulars { get; set; }
        public TranType TransactionType { get; set; }//this is an enum
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {
            TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";//we will use guid to generate it
        }

    }
    public enum TranStatus
    {
        Failed,
        Success,
        Error
    }
    public enum TranType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
}