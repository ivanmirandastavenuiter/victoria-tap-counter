using FluentValidation;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Validators.Template;

namespace victoria_tap.Validators
{
    public class GetDispenserSpendingInfoValidator : VictoriaValidatorTemplate<GetDispenserSpendingInfoRequest>
    {
        private readonly IVictoriaTapRepository _victoriaTapRepository;

        public GetDispenserSpendingInfoValidator(IVictoriaTapRepository victoriaTapRepository)
        {
            _victoriaTapRepository = victoriaTapRepository;
            RegisterValidations();
        }

        private void RegisterValidations()
        {
            base.CheckStringIsNotNullOrEmpty();
            CheckDispenserExists();
        }

        private void CheckDispenserExists()
        {
            RuleFor(x => x.Id)
                .Must(id => _victoriaTapRepository.GetDispenserById(id) != null)
                .WithMessage($"Dispenser does not exist")
                .WithErrorCode("404");
        }
    }
}
