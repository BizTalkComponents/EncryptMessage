namespace BizTalkComponents.Utils.LookupUtils
{
    public interface ISSOConfigRepository
    {
        string Read(string application, string key);
    }
}