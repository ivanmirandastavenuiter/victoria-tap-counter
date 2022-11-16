using victoria_tap.Controllers.Contracts;
using victoria_tap.Validators.Template;

namespace victoria_tap.Validators
{
    public class CreateDispenserValidator : VictoriaValidatorTemplate<CreateDispenserRequest>
    {
        public CreateDispenserValidator()
        {
            base.CheckFloatIsGreaterThanZero();
        }
    }
}
