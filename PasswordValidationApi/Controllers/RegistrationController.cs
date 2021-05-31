using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PasswordValidationApi.Models;
using System.Linq;

namespace PasswordValidationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IValidator<PasswordModel> validator;

        public RegistrationController(IValidator<PasswordModel> validator)
        {
            this.validator = validator;
        }

        [HttpPost]
        public IActionResult Register(PasswordModel password)
        {
            var validationResult = validator.Validate(password);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.First().ErrorMessage);
            }
            return Ok(true);
        }
    }
}
