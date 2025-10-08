using System.ComponentModel.DataAnnotations;

namespace Application.Validations.DataAnnotations
{
    public class PorcentagemAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var porcentagem = Convert.ToInt16(value);

            if (porcentagem < 1 || porcentagem > 100)
                return new ValidationResult($"A porcentagem deve ser entre 1 e 100");

            return ValidationResult.Success;
        }
    }
}
