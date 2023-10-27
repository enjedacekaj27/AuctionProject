using System.ComponentModel.DataAnnotations;

namespace Entities.AttributeValidation;

public class DateLaterThanOrEqualToToday : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return "Date value must be a future date";
    }

    protected override ValidationResult IsValid(object objValue,
                                                   ValidationContext validationContext)
    {
        var dateValue = objValue as DateTime? ?? new DateTime();

        if (dateValue.Date > DateTime.Now.Date)
        {
            return ValidationResult.Success;
        }
        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}
