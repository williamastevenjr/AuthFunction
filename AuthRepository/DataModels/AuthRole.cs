using System.ComponentModel.DataAnnotations;

namespace AuthRepository.DataModels
{
    public class AuthRole
    {
        [Key]
        public byte Id { get; set; }

        [Required, MaxLength(10)]
        public string Name { get; set; }
    }
}
