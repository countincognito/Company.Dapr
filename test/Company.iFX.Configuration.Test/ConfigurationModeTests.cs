using FluentAssertions;

namespace Company.iFX.Configuration.Test
{
    public class ConfigurationModeTests
    {
        [Fact]
        public void ConfigurationMode_GivenDefaultConstructor_ThenModeIsNotSet()
        {
            var configMode = new ConfigurationMode();
            configMode.Mode.Should().Be(ConfigurationState.NotSet);
        }

        [Theory]
        [InlineData(ConfigurationState.NotSet)]
        [InlineData(ConfigurationState.Standard)]
        [InlineData(ConfigurationState.UnderTest)]
        public void ConfigurationMode_GivenConstructor_ThenModeMatchesArg(ConfigurationState configurationState)
        {
            var configMode = new ConfigurationMode(configurationState);
            configMode.Mode.Should().Be(configurationState);
        }

        [Theory]
        [InlineData(ConfigurationState.NotSet, ConfigurationState.UnderTest)]
        [InlineData(ConfigurationState.Standard, ConfigurationState.Standard)]
        [InlineData(ConfigurationState.UnderTest, ConfigurationState.UnderTest)]
        public void ConfigurationMode_GivenStandardMode_ThenCannotPlaceUnderTest(
            ConfigurationState start,
            ConfigurationState finish)
        {
            var configMode = new ConfigurationMode(start);
            configMode.Mode.Should().Be(start);
            configMode.PlaceUnderTest();
            configMode.Mode.Should().Be(finish);
        }
    }
}