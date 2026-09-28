using FluentValidation.TestHelper;
using OrdersBackend.Application.Auth.Commands.Login;

namespace OrdersBackend.Application.Tests.Auth.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(new LoginCommand("dev@martech.com", "Senha@123"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_HasErrorForEmail(string email)
    {
        var result = _validator.TestValidate(new LoginCommand(email, "Senha@123"));

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithEmptyPassword_HasErrorForPassword()
    {
        var result = _validator.TestValidate(new LoginCommand("dev@martech.com", ""));

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
