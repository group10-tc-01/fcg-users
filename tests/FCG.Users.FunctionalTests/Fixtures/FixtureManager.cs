using FCG.Users.FunctionalTests.Extensions;
using FCG.Users.FunctionalTests.Fixtures.Users;
using Reqnroll;

namespace FCG.Users.FunctionalTests.Fixtures
{
    public class FixtureManager
    {
        private readonly ScenarioContext _scenarioContext;

        public FixtureManager(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public RegisterUserFixture RegisterUser => GetOrCreateFixture<RegisterUserFixture>();

        private T GetOrCreateFixture<T>() where T : new()
        {
            var key = typeof(T).Name;
            var fixture = _scenarioContext.GetScenario<T>(key);

            if (fixture == null)
            {
                fixture = new T();
                _scenarioContext[key] = fixture;
            }
            return fixture;
        }
    }
}
