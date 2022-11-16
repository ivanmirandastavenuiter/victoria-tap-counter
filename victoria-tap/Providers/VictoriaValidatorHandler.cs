using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Providers.Contracts;

namespace victoria_tap.Providers
{
    public class VictoriaValidatorHandler : IVictoriaValidatorHandler
    {
        private readonly IValidator<ChangeDispenserStatusRequest> _changeDispenserStatusValidator;
        private readonly IValidator<CreateDispenserRequest> _createDispenserValidator;
        private readonly IValidator<GetDispenserSpendingInfoRequest> _getDispenserSpendingInfoValidator;
        private const string CHANGE_DISPENSER_STATUS = "ChangeDispenserStatusRequest";
        private const string CREATE_DISPENSER = "CreateDispenserRequest";
        private const string GET_DISPENSER_SPENDING_INFO = "GetDispenserSpendingInfoRequest";

        public VictoriaValidatorHandler(
            IValidator<ChangeDispenserStatusRequest> changeDispenserStatusValidator,
            IValidator<CreateDispenserRequest> createDispenserValidator,
            IValidator<GetDispenserSpendingInfoRequest> getDispenserSpendingInfoValidator)
        {
            _changeDispenserStatusValidator = changeDispenserStatusValidator;
            _createDispenserValidator = createDispenserValidator;
            _getDispenserSpendingInfoValidator = getDispenserSpendingInfoValidator;
        }

        public IActionResult Validate(object objectToValidate)
        {
            var objectType = objectToValidate.GetType()
                                             .ToString()
                                             .Split(".")
                                             .Last();

            var validation = objectType switch
            {
                CHANGE_DISPENSER_STATUS => _changeDispenserStatusValidator.Validate((ChangeDispenserStatusRequest)objectToValidate),
                CREATE_DISPENSER => _createDispenserValidator.Validate((CreateDispenserRequest)objectToValidate),
                GET_DISPENSER_SPENDING_INFO => _getDispenserSpendingInfoValidator.Validate((GetDispenserSpendingInfoRequest)objectToValidate),
                _ => new()
            };

            if (validation != null && validation.Errors.Any())
            {
                return BuildResponse(validation);
            }

            return null;
        }

        private IActionResult BuildResponse(ValidationResult validation)
        {
            var error = validation.Errors.First();
            var code = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), error.ErrorCode);

            IActionResult response = code switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(error),
                HttpStatusCode.NotFound => new NotFoundObjectResult(error),
                HttpStatusCode.Conflict => new ConflictObjectResult(error),
                HttpStatusCode.InternalServerError => new StatusCodeResult(StatusCodes.Status500InternalServerError),
                _ => throw new NotImplementedException()
            };

            return response;
        }
    }
}
