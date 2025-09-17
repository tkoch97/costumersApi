using costumersApi.Models;
using FluentValidation;

namespace costumersApi.Validators
{
  public class CostumerValidator : AbstractValidator<Costumer>
  {
    public CostumerValidator()
    {
      RuleFor(costumer => costumer.Name)
      .NotEmpty().WithMessage("Nome é obrigatório. Por favor, informe.");

      RuleFor(costumer => costumer.Email)
      .NotEmpty().EmailAddress().WithMessage("Insira um endereço de email válido.");
    }
  }
}