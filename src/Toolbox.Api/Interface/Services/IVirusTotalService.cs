using Toolbox.Api.Models.VirusTotal;

namespace Toolbox.Api.Interface.Services;
public interface IVirusTotalService
{
    Task<(bool Succeed, string Message, AnalysisRelustResponse? Data)> CheckByFileHashAsync(string sha1);
}