using System.Text.Json.Serialization;

namespace Toolbox.Api.Models.VirusTotal;

public class AnalysisRelust
{
    public AnalysisRelustError? error { get; set; }
    public AnalysisRelustData? data { get; set; }
}

public class AnalysisRelustError
{
    public string message { get; set; }
    public string code { get; set; }
}

public class AnalysisRelustData
{
    public AnalysisDetail attributes { get; set; }
}

public class AnalysisDetail
{
    public Known_Distributors? known_distributors { get; set; }
    public int size { get; set; }
    public string meaningful_name { get; set; }
    public int last_analysis_date { get; set; }
    public string sha1 { get; set; }
    public string md5 { get; set; }
    public Last_Analysis_Stats last_analysis_stats { get; set; }
    public Last_Analysis_Results last_analysis_results { get; set; }
    public int reputation { get; set; }
}

public class Known_Distributors
{
    public List<string> distributors { get; set; }
}

public class Last_Analysis_Stats
{
    public int harmless { get; set; }

    [JsonPropertyName("type-unsupported")]
    public int typeunsupported { get; set; }
    public int suspicious { get; set; }
    public int confirmedtimeout { get; set; }
    public int timeout { get; set; }
    public int failure { get; set; }
    public int malicious { get; set; }
    public int undetected { get; set; }
}

public class Last_Analysis_Results
{
    public AnalysisResults? Bkav { get; set; }
    public AnalysisResults? Lionic { get; set; }
    public AnalysisResults? Elastic { get; set; }
    public AnalysisResults? MicroWorldeScan { get; set; }
    public AnalysisResults? ClamAV { get; set; }
    public AnalysisResults? FireEye { get; set; }
    public AnalysisResults? CATQuickHeal { get; set; }
    public AnalysisResults? ALYac { get; set; }
    public AnalysisResults? Cylance { get; set; }
    public AnalysisResults? Zillya { get; set; }
    public AnalysisResults? Sangfor { get; set; }
    public AnalysisResults? K7AntiVirus { get; set; }
    public AnalysisResults? Alibaba { get; set; }
    public AnalysisResults? K7GW { get; set; }
    public AnalysisResults? Trustlook { get; set; }
    public AnalysisResults? Arcabit { get; set; }
    public AnalysisResults? BitDefenderTheta { get; set; }
    public AnalysisResults? VirIT { get; set; }
    public AnalysisResults? Cyren { get; set; }
    public AnalysisResults? SymantecMobileInsight { get; set; }
    public AnalysisResults? Symantec { get; set; }
    public AnalysisResults? tehtris { get; set; }
    public AnalysisResults? ESETNOD32 { get; set; }
    public AnalysisResults? Zoner { get; set; }
    public AnalysisResults? APEX { get; set; }
    public AnalysisResults? Paloalto { get; set; }
    public AnalysisResults? Cynet { get; set; }
    public AnalysisResults? Kaspersky { get; set; }
    public AnalysisResults? BitDefender { get; set; }
    public AnalysisResults? NANOAntivirus { get; set; }
    public AnalysisResults? ViRobot { get; set; }
    public AnalysisResults? Avast { get; set; }
    public AnalysisResults? Tencent { get; set; }
    public AnalysisResults? TACHYON { get; set; }
    public AnalysisResults? Sophos { get; set; }
    public AnalysisResults? Baidu { get; set; }
    public AnalysisResults? FSecure { get; set; }
    public AnalysisResults? DrWeb { get; set; }
    public AnalysisResults? VIPRE { get; set; }
    public AnalysisResults? TrendMicro { get; set; }
    public AnalysisResults? McAfeeGWEdition { get; set; }
    public AnalysisResults? Trapmine { get; set; }
    public AnalysisResults? CMC { get; set; }
    public AnalysisResults? Emsisoft { get; set; }
    public AnalysisResults? Ikarus { get; set; }
    public AnalysisResults? AvastMobile { get; set; }
    public AnalysisResults? Jiangmin { get; set; }
    public AnalysisResults? Webroot { get; set; }
    public AnalysisResults? Avira { get; set; }
    public AnalysisResults? AntiyAVL { get; set; }
    public AnalysisResults? Gridinsoft { get; set; }
    public AnalysisResults? Xcitium { get; set; }
    public AnalysisResults? Microsoft { get; set; }
    public AnalysisResults? SUPERAntiSpyware { get; set; }
    public AnalysisResults? ZoneAlarm { get; set; }
    public AnalysisResults? GData { get; set; }
    public AnalysisResults? Google { get; set; }
    public AnalysisResults? BitDefenderFalx { get; set; }
    public AnalysisResults? AhnLabV3 { get; set; }
    public AnalysisResults? Acronis { get; set; }
    public AnalysisResults? McAfee { get; set; }
    public AnalysisResults? MAX { get; set; }
    public AnalysisResults? VBA32 { get; set; }
    public AnalysisResults? Malwarebytes { get; set; }
    public AnalysisResults? Panda { get; set; }
    public AnalysisResults? TrendMicroHouseCall { get; set; }
    public AnalysisResults? Rising { get; set; }
    public AnalysisResults? Yandex { get; set; }
    public AnalysisResults? SentinelOne { get; set; }
    public AnalysisResults? MaxSecure { get; set; }
    public AnalysisResults? Fortinet { get; set; }
    public AnalysisResults? AVG { get; set; }
    public AnalysisResults? DeepInstinct { get; set; }
}

public class AnalysisResults
{
    public string category { get; set; }
    public string engine_name { get; set; }
    public string result { get; set; }
}