namespace BizTalkComponents.Utils.LookupUtils
{
    public class SSOConfigRepository : ISSOConfigRepository
    {
        public string Read(string application, string key)
        {
            return SSOClientHelper.Read(application, key);
        }
    }
}
