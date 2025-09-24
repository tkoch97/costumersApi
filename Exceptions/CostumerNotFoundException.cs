namespace costumersApi.Exceptions
{
  public class CostumerNotFoundException : Exception
  {
    public CostumerNotFoundException(int id) : base($"Cliente com id {id} não encontrado")
    {

    }
  }
}