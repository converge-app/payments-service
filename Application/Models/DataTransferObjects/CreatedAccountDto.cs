namespace Application.Models.DataTransferObjects
{
  public class CreatedAccountDto
  {
    public string Id { get; set; }
    public string UserId { get; set; }
    public bool LiveMode { get; set; }
    public string StripeUserId { get; set; }
    public string Scope { get; set; }
  }
}