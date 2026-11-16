using FluentValidation;
using MoviesService.Domain.Entities;

namespace MoviesService.Application.Validators
{
    /// <summary>
    /// Validates movie entity properties using FluentValidation rules.
    /// </summary>
    public class MoviesValidator : AbstractValidator<Movies>
    {
        /// <summary>
        /// Initializes validation rules for the <see cref="Movies"/> entity.
        /// </summary>
        public MoviesValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title not be empty");

            RuleFor(x => x.Actor)
                .NotEmpty()
                .WithMessage("add Actor name");
        }
    }
}