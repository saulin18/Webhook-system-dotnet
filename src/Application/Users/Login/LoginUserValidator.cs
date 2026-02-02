using FluentValidation;
using Application.Users.Login;

namespace Application.Users.Login;

internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");
        RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required").MinimumLength(7).WithMessage("Password must be at least 8 characters long");
    }
}