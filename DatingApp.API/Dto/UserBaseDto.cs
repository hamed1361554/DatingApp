using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dto
{
    public class UserBaseDto
    {
        [Required]
        [StringLength(32, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
    }
}