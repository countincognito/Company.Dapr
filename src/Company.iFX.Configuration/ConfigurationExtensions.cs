namespace Company.iFX.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void PlaceUnderTest(this ConfigurationMode configuration)
        {
            if (configuration.Mode == ConfigurationState.NotSet)
            {
                configuration.Mode = ConfigurationState.UnderTest;
            }
        }
    }
}
