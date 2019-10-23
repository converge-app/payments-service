using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
    public class CreateAccountDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string AuthorizationCode { get; set; }
    }
}