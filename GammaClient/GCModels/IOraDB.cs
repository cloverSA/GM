namespace GammaClient.GCModels
{
    public interface IOraDB
    {
        string DBHome { get; set; }
        int DBID { get; set; }
        string DBName { get; set; }
        bool IsSelected { get; set; }
        WorkLoad WorkLoad { get; set; }
    }
}