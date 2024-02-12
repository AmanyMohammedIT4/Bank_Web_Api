using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Bank_Web_Api.Model
{
    [Table("Accounts")]
    public class Account
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
        [JsonIgnore]
        public byte[] PinHash { get; set; }
        [JsonIgnore]
        public byte[] PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        //Generate an accountNumber, let's do that in the contructor
        //first let's create a Random() obj;
        Random random = new Random();
        public Account()
        {
            AccountNumberGenerated= Convert.ToString((long) Math.Floor(random.NextDouble() * 9_000_000_000L + 1_000_000_000L));//we did 9_000_000_000 so we could get a 10-digit random number
            //also AccountName property = FirstName+LastName
            AccountName = $"{FirstName} {LastName}";
        }
    }
    public enum AccountType
    {
        Savings,//saving => 0 , current =>1 etc
        Current,
        Corporate,
        Government
    }

}
