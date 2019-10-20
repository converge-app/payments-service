using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
  public class DepositCreationDto
  {
    [Required]
    public string UserId { get; set; }
    public long Amount { get; set; }
  }
}