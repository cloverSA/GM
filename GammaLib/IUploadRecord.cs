namespace GeneralUtility
{
    public interface IUploadRecord
    {
        string Passwd { get; set; }
        string Source { get; set; }
        string Target { get; set; }
        string Usr { get; set; }
    }
}