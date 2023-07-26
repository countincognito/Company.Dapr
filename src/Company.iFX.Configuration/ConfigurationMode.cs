namespace Company.iFX.Configuration
{
    public class ConfigurationMode
    {
        private ConfigurationState m_State = ConfigurationState.NotSet;

        public ConfigurationMode()
        {
        }

        public ConfigurationMode(ConfigurationState underTest)
        {
            m_State = underTest;
        }

        public ConfigurationState Mode
        {
            get
            {
                if (m_State == ConfigurationState.NotSet)
                {
                    m_State = ConfigurationState.Standard;
                }
                return m_State;
            }
            internal set
            {
                m_State = value;
            }
        }
    }
}
