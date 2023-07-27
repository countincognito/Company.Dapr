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

        public ConfigurationState State
        {
            get
            {
                ActivateStandard();
                return m_State;
            }
        }

        public void ActivateStandard()
        {
            if (m_State == ConfigurationState.NotSet)
            {
                m_State = ConfigurationState.Standard;
            }
        }

        public void ActivateTest()
        {
            if (m_State == ConfigurationState.NotSet)
            {
                m_State = ConfigurationState.Test;
            }
        }
    }
}
