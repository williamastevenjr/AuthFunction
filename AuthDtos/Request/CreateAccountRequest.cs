using System.ComponentModel.DataAnnotations;

namespace AuthDtos.Request
{
    public class CreateAccountRequest
    {
        [Required, MinLength(3), MaxLength(30)]
        public string AccountName { get; set; }

        [Required, MinLength(6), MaxLength(256)]
        public string Password { get; set; }

        [Required, MinLength(6), MaxLength(256)]
        public string ConfirmPassword { get; set; }
    }
}
