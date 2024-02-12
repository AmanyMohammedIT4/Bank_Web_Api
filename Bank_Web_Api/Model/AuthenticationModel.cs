using System.ComponentModel.DataAnnotations;

namespace Bank_Web_Api.Model
{
    public class AuthenticationModel
    {
        [Required]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{9}")]
        public string AccountNumber { get; set; }
        [Required]
        public string Pin { get; set; }
    }
}
