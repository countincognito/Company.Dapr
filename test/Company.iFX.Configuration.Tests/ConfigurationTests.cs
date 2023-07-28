using FluentAssertions;

namespace Company.iFX.Configuration.Tests
{
    public class ConfigurationTests
    {





        //[Fact]
        //public void A()
        //{
        //    Configuration.TestRequested.Should().BeFalse();
        //    Configuration.PlaceUnderTest();
        //    Configuration.TestRequested.Should().BeTrue();
        //}


        //[Fact]
        //public void B()
        //{
        //    Configuration.TestRequested.Should().BeFalse();
        //    Configuration.SystemUnderTest.Should().BeFalse();
        //    Configuration.PlaceUnderTest();
        //    Configuration.SystemUnderTest.Should().BeTrue();
        //    Configuration.TestRequested.Should().BeTrue();
        //}




        //[Theory]
        //[InlineData(ConfigurationState.NotSet)]
        //[InlineData(ConfigurationState.Standard)]
        //[InlineData(ConfigurationState.UnderTest)]
        //public void ConfigurationMode_GivenConstructor_ThenModeMatchesArg(ConfigurationState configurationState)
        //{
        //    var configMode = new ConfigurationMode(configurationState);
        //    configMode.Mode.Should().Be(configurationState);
        //}

        //[Theory]
        //[InlineData(ConfigurationState.NotSet, ConfigurationState.UnderTest)]
        //[InlineData(ConfigurationState.Standard, ConfigurationState.Standard)]
        //[InlineData(ConfigurationState.UnderTest, ConfigurationState.UnderTest)]
        //public void ConfigurationMode_GivenStandardMode_ThenCannotPlaceUnderTest(
        //    ConfigurationState start,
        //    ConfigurationState finish)
        //{
        //    var configMode = new ConfigurationMode(start);
        //    configMode.Mode.Should().Be(start);
        //    configMode.PlaceUnderTest();
        //    configMode.Mode.Should().Be(finish);
        //}
    }
}