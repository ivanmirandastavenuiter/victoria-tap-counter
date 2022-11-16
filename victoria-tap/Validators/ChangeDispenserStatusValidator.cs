using FluentValidation;
using System.Text.RegularExpressions;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Validators.Template;

namespace victoria_tap.Validators
{
    // O/C Principle: coherent extension of superclasses
    public class ChangeDispenserStatusValidator : VictoriaValidatorTemplate<ChangeDispenserStatusRequest>
    {
        private readonly IVictoriaTapRepository _victoriaTapRepository;
        private static readonly string ValidDatePattern = "^([\\+-]?\\d{4}(?!\\d{2}\\b))((-?)((0[1-9]|1[0-2])(\\3([12]\\d|0[1-9]|3[01]))?|W([0-4]\\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\\d|[12]\\d{2}|3([0-5]\\d|6[1-6])))([T\\s]((([01]\\d|2[0-3])((:?)[0-5]\\d)?|24\\:?00)([\\.,]\\d+(?!:))?)?(\\17[0-5]\\d([\\.,]\\d+)?)?([zZ]|([\\+-])([01]\\d|2[0-3]):?([0-5]\\d)?)?)?)?$";

        public ChangeDispenserStatusValidator(IVictoriaTapRepository victoriaTapRepository)
        {
            _victoriaTapRepository = victoriaTapRepository;
            RegisterValidations();
        }

        private void RegisterValidations()
        {
            base.CheckStringIsNotNullOrEmpty();
            CheckDispenserExists();
            CheckStatusIsValid();
            CheckDateFormatIsValid();
            CheckIfStatusIsValid();
            CheckClosingDateIsAheadOfOpenedAt();
        }

        private void CheckDispenserExists()
        {
            RuleFor(x => x.Id)
                .Must(id => _victoriaTapRepository.GetDispenserById(id) != null)
                .WithMessage($"Dispenser does not exist")
                .WithErrorCode("400");
        }

        private void CheckStatusIsValid()
        {
            RuleFor(x => x.Status)
                .Must(status => (new[] { "open", "closed" }).Contains(status))
                .WithMessage("Invalid status. Status must be 'open' or 'closed'")
                .WithErrorCode("400");
        }

        private void CheckDateFormatIsValid()
        {
            RuleFor(x => x.UpdatedAt)
                .Must(updatedAt =>
                {
                    var isValidIsoFormat = Regex.Match(updatedAt, ValidDatePattern).Success;
                    return isValidIsoFormat;
                })
                .WithMessage("Invalid ISO date format")
                .WithErrorCode("400");
        }

        private void CheckIfStatusIsValid()
        {
            RuleFor(x => x)
              .Must(request =>
              {
                  var validStatus = false;
                  if (IsStatusOpen(request.Status))
                  {
                      var dispenserInfo = _victoriaTapRepository.GetDispenserInfoById(request.Id);

                      if (dispenserInfo == null)
                      {
                          validStatus = true;
                      }
                      else
                      {
                          var usages = dispenserInfo.Usages;
                          validStatus = usages.All(u => u.ClosedAt != null);
                      }
                  }

                  if (IsStatusClosed(request.Status))
                  {
                      var dispenserInfo = _victoriaTapRepository.GetDispenserInfoById(request.Id);
                      validStatus = dispenserInfo != null && dispenserInfo.Usages.Any(x => x.ClosedAt == null);
                  }

                  return validStatus;
              })
              .WithMessage("Invalid status for current state of dispenser")
              .WithErrorCode("409");
        }

        private void CheckClosingDateIsAheadOfOpenedAt()
        {
            RuleFor(x => x)
              .Must(request =>
              {
                  var validClosingDate = false;

                  var dispenserInfo = _victoriaTapRepository.GetDispenserInfoById(request.Id);
                  var isClosedValidState = dispenserInfo != null && dispenserInfo.Usages.Any(x => x.ClosedAt == null);

                  if (isClosedValidState && dispenserInfo != null)
                  {
                      var closingDate = DateTimeOffset.Parse(request.UpdatedAt);

                      var currentUsage = dispenserInfo.Usages.Single(x => x.ClosedAt == null);

                      var openingDate = DateTimeOffset.Parse(currentUsage.OpenedAt);

                      validClosingDate = closingDate > openingDate;
                  }

                  return validClosingDate;
              })
              .WithMessage("Invalid closing date for opened dispenser")
              .When(x => IsStatusClosed(x.Status) && Regex.Match(x.UpdatedAt, ValidDatePattern).Success)
              .WithErrorCode("400");
        }

        private bool IsStatusOpen(string status) => status.Equals("open");
        private bool IsStatusClosed(string status) => status.Equals("closed");
    }
}
