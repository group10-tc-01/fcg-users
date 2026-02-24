using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.FunctionalTests.Fixtures;
using FluentAssertions;
using Reqnroll;

namespace FCG.Users.FunctionalTests.Steps.Users
{
    [Binding]
    public class RegisterUserStepDefinition(FixtureManager fixtureManager)
    {
        private readonly FixtureManager _fixtureManager = fixtureManager;
        private RegisterUserRequest? _registerUserRequest;
        private FCG.Users.Application.Abstractions.Results.Result<RegisterUserResponse>? _registerUserResponse;

        [Given(@"a criacao de um novo usuario")]
        public void GivenTheCreationOfANewUser()
        {
            _registerUserRequest = _fixtureManager.RegisterUser.RegisterUserRequest;
        }

        [When(@"o usuario envia uma requisicao de registro com dados validos")]
        public async Task WhenUserSendsRegistrationRequestWithValidData()
        {
            _registerUserResponse = await _fixtureManager.RegisterUser.RegisterUserUseCase.Handle(_registerUserRequest!, CancellationToken.None);
        }

        [Then(@"o usuario deve ser criado com sucesso")]
        public void ThenTheUserShouldBeCreatedSuccessfully()
        {
            _registerUserResponse.Should().NotBeNull();
            _registerUserResponse!.IsSuccess.Should().BeTrue();
            _registerUserResponse.Value.Should().NotBeNull();
        }
    }
}
