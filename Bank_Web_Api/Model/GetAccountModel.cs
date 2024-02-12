using System.ComponentModel.DataAnnotations;

namespace Bank_Web_Api.Model
{
    public class GetAccountModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; }//enum to show if the account to be created is "savings" or "current"
        public string AccountNumberGenerated { get; set; }//we shall generate accountNumber here!

        //we'll also store the hash and salt of the Account Transaction pin
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }
}
