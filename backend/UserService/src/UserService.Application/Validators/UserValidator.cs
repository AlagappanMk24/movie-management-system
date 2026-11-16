using FluentValidation;
using UserService.Domain.Entities;

namespace UserService.Application.Validators
{
    /// <summary>
    /// Validates user registration and profile properties using FluentValidation rules.
    /// </summary>
    public class UserValidator : AbstractValidator<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserValidator"/> class with rules for email, password, phone number, and username.
        /// </summary>
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email Is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(10)
                .WithMessage("Mobile number must be 10 digit and not empty");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("UserName is Required");
        }
    }
}