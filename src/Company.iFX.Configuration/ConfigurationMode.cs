namespace Company.iFX.Configuration
{
    public class ConfigurationMode
    {
        private ConfigurationState m_State = ConfigurationState.NotSet;

        public ConfigurationMode()
        {
        }

        public ConfigurationMode(ConfigurationState state)
        {
            m_State = state;
        }

        public ConfigurationState Mode
        {
            get
            {
                return m_State;
            }
            internal set
            {
                m_State = value;
            }
        }
    }
}
