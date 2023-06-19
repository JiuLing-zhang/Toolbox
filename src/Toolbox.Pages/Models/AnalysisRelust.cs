namespace Toolbox.Pages.Models;

public class AnalysisRelust
{
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
    public int Malicious { get; set; }
    public int Undetected { get; set; }
}

public class PlatformAnalysisResults
{
    public string Category { get; set; }
    public string EngineName { get; set; }
    public string Result { get; set; }
    public int Sort
    {
        get
        {
            return Category switch
            {
                "malicious" => 1,
                "undetected" => 2,
                "type-unsupported" => 3,
                _ => 99,
            };
        }
    }
}