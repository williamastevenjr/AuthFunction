using System.ComponentModel.DataAnnotations;

namespace AuthDtos.Request
{
    public class JwtAuthRequest
    {
        [Required]
        public string AccountName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
