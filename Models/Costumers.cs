namespace costumersApi.Models
{
  public class Costumer()
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; init; }
  }
}