namespace Toolbox.Api.Models.VirusTotal;
public class AnalysisRelustResponse
{
    public AnalysisRelustResponse(string fileName, int size, string sha1, string md5, int analysisDate, PlatformAnalysisStats platformStats, List<PlatformAnalysisResults> platformResults, int reputation, string? knownDistributor)
    {
        FileName = fileName;
        Size = size;
        SHA1 = sha1;
        MD5 = md5;
        AnalysisDate = analysisDate;
        PlatformStats = platformStats;
        PlatformResults = platformResults;
        Reputation = reputation;
        KnownDistributor = knownDistributor;
    }

    public string FileName { get; set; }
    public int Size { get; set; }
    public string SHA1 { get; set; }
    public string MD5 { get; set; }
    public int AnalysisDate { get; set; }
    public PlatformAnalysisStats PlatformStats { get; set; }
    public List<PlatformAnalysisResults> PlatformResults { get; set; }
    public int Reputation { get; set; }
    public string? KnownDistributor { get; set; }
}

public class PlatformAnalysisStats
{
    public PlatformAnalysisStats(int malicious, int undetected)
    {
        Malicious = malicious;
        Undetected = undetected;
    }

    public int Malicious { get; set; }
    public int Undetected { get; set; }

}

public class PlatformAnalysisResults
{
    public PlatformAnalysisResults(string category, string engineName, string result)
    {
        Category = category;
        EngineName = engineName;
        Result = result;
    }

    public string Category { get; set; }
    public string EngineName { get; set; }
    public string Result { get; set; }
}