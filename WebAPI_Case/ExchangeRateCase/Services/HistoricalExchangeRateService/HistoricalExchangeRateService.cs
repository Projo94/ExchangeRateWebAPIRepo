using ExchangeRateCase.Models;
using ExchangeRateCaseSolution.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateCase.Services
{
    public class HistoricalExchangeRateService : IHistoricalExchangeRateService
    {
        private readonly IApiCall _apiCall;

        public HistoricalExchangeRateService(IApiCall apiCall)
        {
            _apiCall = apiCall ?? throw new ArgumentNullException(nameof(apiCall));
        }

        public Task<AvgMinMax> getAvgMinMax(ConcurrentBag<ExchangeRate> list)
        {
            ConcurrentDictionary<string, double> dictionary = new ConcurrentDictionary<string, double>();
            double sumValues = 0;

            List<Task> TaskList = new List<Task>();
            Parallel.ForEach(list, item =>
            {
                if (item.rates.Count == 1 && !dictionary.ContainsKey(item.rates.Keys.First()))
                {
                    double value = item.rates[item.rates.Keys.First()][item.rates[item.rates.Keys.First()].Keys.First()];
                    sumValues += value;
                    dictionary.TryAdd(item.rates.Keys.First(), value);
                }

            });

            Task<AvgMinMax> avgMinMax = Task.FromResult(new AvgMinMax());

            if (dictionary.Keys.Count > 0)
            {

                avgMinMax.Result.avg = Math.Round(sumValues / list.Count, 12, MidpointRounding.AwayFromZero);

                avgMinMax.Result.dateMin = dictionary.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                avgMinMax.Result.min = dictionary[avgMinMax.Result.dateMin];

                avgMinMax.Result.dateMax = dictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                avgMinMax.Result.max = dictionary[avgMinMax.Result.dateMax];
            }


            return avgMinMax;
        }

        public Task<ResponseMinMaxAvg> getAvgMinMaxToString(AvgMinMax avgMinMax)
        {
            var min = $"{Strings_lang.MIN_RATE} {avgMinMax.min} {Strings_lang.ON} {avgMinMax.dateMin}";
            var max = $"{Strings_lang.MAX_RATE} {avgMinMax.max} {Strings_lang.ON} {avgMinMax.dateMax}";
            var avg = $"{Strings_lang.AVG_RATE} {avgMinMax.avg}";

            Task<ResponseMinMaxAvg> responseMinMaxAvg = Task.FromResult(new ResponseMinMaxAvg(min, max, avg));

            return responseMinMaxAvg;
        }

        public async Task<string> getResponseJSON(string baseCurrency, string targetCurrency, string date)
        {
            var url = "https://api.exchangeratesapi.io/history?start_at=" + date + "&end_at=" + date + "&base=" + baseCurrency + "&symbols=" + targetCurrency;

            return await _apiCall.getData(url);
        }

        public Task<ConcurrentBag<ExchangeRate>> sendRequestRemoteServer(string baseCurrency, string targetCurrency, List<string> listDate)
        {
            Task<ConcurrentBag<ExchangeRate>> list = Task.FromResult(new ConcurrentBag<ExchangeRate>());

            try
            {
                List<Task> TaskList = new List<Task>();

                Parallel.ForEach(listDate, date =>
                {
                    var task = Task.Run(async delegate
                    {
                        var responseString = await getResponseJSON(baseCurrency, targetCurrency, date);

                        var response = JsonConvert.DeserializeObject<ExchangeRate>(responseString);
                        list.Result.Add(response);
                    });

                    TaskList.Add(task);

                });

                Task.WaitAll(TaskList.ToArray());

            }
            catch
            {
                list.Result.Clear();
                return list;
            }

            return list;

        }


    }
}
