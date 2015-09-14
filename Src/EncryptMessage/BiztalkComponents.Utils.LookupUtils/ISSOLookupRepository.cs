namespace BizTalkComponents.Utils.LookupUtils
{
    public interface ISSOLookupRepository
    {
        string Read(string application, string key);
    }
}