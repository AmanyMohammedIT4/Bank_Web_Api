using System.ComponentModel.DataAnnotations;

namespace Bank_Web_Api.Model
{
    public class TransactionRequestDto
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; }//this is an enum
        public DateTime TransactionDate { get; set; }

    }
}
