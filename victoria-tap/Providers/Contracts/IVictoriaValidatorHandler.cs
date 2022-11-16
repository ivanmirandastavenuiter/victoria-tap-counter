using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace victoria_tap.Providers.Contracts
{
    // Interface segregation is key for abstraction, loose dependencies and test-friendly approach
    public interface IVictoriaValidatorHandler
    {
        public IActionResult Validate(object objectToValidate);
    }
}
