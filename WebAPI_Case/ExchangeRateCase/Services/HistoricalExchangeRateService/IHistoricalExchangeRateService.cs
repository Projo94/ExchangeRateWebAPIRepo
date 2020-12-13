using ExchangeRateCase.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeRateCase.Services
{
    public interface IHistoricalExchangeRateService
    {
        Task<ConcurrentBag<ExchangeRate>> sendRequestRemoteServer(string baseCurrency, string targetCurrency, List<string> listDate);

        Task<string> getResponseJSON(string baseCurrency, string targetCurrency, string date);

        Task<AvgMinMax> getAvgMinMax(ConcurrentBag<ExchangeRate> list);

        Task<ResponseMinMaxAvg> getAvgMinMaxToString(AvgMinMax avgMinMax);

    }
}
