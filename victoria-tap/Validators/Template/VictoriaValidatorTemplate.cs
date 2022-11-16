using FluentValidation;

namespace victoria_tap.Validators.Template
{
    // Template design pattern allows us to easily extend and maintain code that share common features or behaviors
    public class VictoriaValidatorTemplate<T> : AbstractValidator<T>
    {
        protected virtual void CheckStringIsNotNullOrEmpty()
        {
            RuleFor(x => x)
                .Must(input =>
                {
                    var properties = input!.GetType().GetProperties();
                    return properties.All(prop =>
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            return !string.IsNullOrWhiteSpace(prop?.GetValue(input)?.ToString());
                        }
                        return true;
                    });
                })
                .WithMessage($"String cannot be null, white space or empty")
                .WithErrorCode("400");
        }

        protected virtual void CheckFloatIsGreaterThanZero()
        {
            RuleFor(x => x)
                .Must(input =>
                {
                    var properties = input!.GetType().GetProperties();
                    return properties.All(prop =>
                    {
                        if (prop.PropertyType == typeof(float))
                        {
                            float validatedFloat;
                            float.TryParse(prop?.GetValue(input)?.ToString(), out validatedFloat);
                            return validatedFloat > 0;
                        }
                        return true;
                    });
                })
                .WithMessage($"Float number must be valid and greater than zero")
                .WithErrorCode("400");
        }
    }
}
