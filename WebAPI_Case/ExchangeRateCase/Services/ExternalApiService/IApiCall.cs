using System.Threading.Tasks;

namespace ExchangeRateCase.Services
{
    public interface IApiCall
    {
        Task<string> getData(string url);
    }
}
