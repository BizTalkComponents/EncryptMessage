namespace BizTalkComponents.Utils.LookupUtils
{
    public class SSOLookupRepository : ISSOLookupRepository
    {
        public string Read(string application, string key)
        {
            return SSOClientHelper.Read(application, key);
        }
    }
}
