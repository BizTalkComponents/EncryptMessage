namespace BizTalkComponents.Utils.LookupUtils
{
    public class SSOLookupManager
    {
        private readonly ISSOLookupRepository _ssoConfigRepository;

        public SSOLookupManager(ISSOLookupRepository ssoConfigRepository)
        {
            _ssoConfigRepository = ssoConfigRepository;
        }

        public string GetConfigValue(string application, string key)
        {
            return _ssoConfigRepository.Read(application, key);
        }
    }
}