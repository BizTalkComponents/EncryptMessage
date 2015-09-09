namespace BizTalkComponents.Utils.LookupUtils
{
    public class SSOConfigReader
    {
        private readonly ISSOConfigRepository _ssoConfigRepository;

        public SSOConfigReader(ISSOConfigRepository ssoConfigRepository)
        {
            _ssoConfigRepository = ssoConfigRepository;
        }

        public string GetConfigValue(string application, string key)
        {
            return _ssoConfigRepository.Read(application, key);
        }
    }
}