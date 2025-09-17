using Shouldly;

namespace Company.iFX.Configuration.Tests
{
    public class ConfigurationModeTests
    {
        [Fact]
        public void ConfigurationMode_GivenDefaultConstructor_ThenNotSetBecomesStandard()
        {
            var configMode = new ConfigurationMode();
            configMode.State.ShouldBe(ConfigurationState.Standard);
        }

        [Theory]
        [InlineData(ConfigurationState.NotSet, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.Standard, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.Test, ConfigurationState.Test)]
        public void ConfigurationMode_GivenConstructor_WhenArgIsNotSet_ThenNotSetBecomesStandard(
            ConfigurationState input,
            ConfigurationState result)
        {
            var configMode = new ConfigurationMode(input);
            configMode.State.ShouldBe(result);
        }

        [Theory]
        [InlineData(ConfigurationState.NotSet, ConfigurationState.Test)]
        [InlineData(ConfigurationState.Standard, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.Test, ConfigurationState.Test)]
        public void ConfigurationMode_GivenActivateTest_WhenIsFirstAction_ThenNotSetBecomesTest(
            ConfigurationState input,
            ConfigurationState result)
        {
            var configMode = new ConfigurationMode(input);
            configMode.ActivateTest();
            configMode.State.ShouldBe(result);
        }

        [Theory]
        [InlineData(ConfigurationState.NotSet, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.Standard, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.Test, ConfigurationState.Test)]
        public void ConfigurationMode_GivenActivateTest_WhenStateIsFirstChecked_ThenNotSetBecomesStandard(
            ConfigurationState input,
            ConfigurationState result)
        {
            var configMode = new ConfigurationMode(input);
            configMode.State.ShouldBe(result);
            configMode.ActivateTest();
            configMode.State.ShouldBe(result);
        }
    }
}