using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
  public class TransferCreationDto
  {
    [Required]
    public string SenderId { get; set; }

    [Required]
    public string ReceiverId { get; set; }

    [Required]
    public long Amount { get; set; }
  }

}