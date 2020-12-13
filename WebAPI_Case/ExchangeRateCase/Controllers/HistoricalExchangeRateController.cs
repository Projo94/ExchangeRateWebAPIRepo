using System;
using System.Collections.Generic;
using ExchangeRateCase.Models;
using ExchangeRateCase.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ExchangeRateCaseSolution.Localization;
using ExchangeRateCaseSolution.Models.HistoryExchangeRate;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ExchangeRateCase.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricalExchangeRateController : ControllerBase
    {
        private readonly IHistoricalExchangeRateService _historicalExchangeRateService;

        public HistoricalExchangeRateController(IHistoricalExchangeRateService historicalExchangeRateService)
        {
            _historicalExchangeRateService = historicalExchangeRateService ?? throw new ArgumentNullException(nameof(historicalExchangeRateService));
        }

        [HttpPost]
        [Route("rates")]
        public async Task<IActionResult> MaxMinAvgRates(string baseCurrency, string targetCurrency, [FromBody] DateData listDate)
        {
            List<string> listOfDates = listDate.date[listDate.date.Keys.First()];

            ConcurrentBag<string> resultCollection = new ConcurrentBag<string>();

            Parallel.ForEach(listOfDates, date =>
            {
                DateTime dateTime;

                if (!DateTime.TryParse(date, out dateTime))
                {
                    resultCollection.Add(date);
                }

            });

            if (resultCollection.Count > 0)
            {
                ModelState.AddModelError(
                   Strings_lang.DATE_ERROR_KEY,
                  $"{Strings_lang.DATE_FORMAT_INCORRECT_ERROR_MESSAGE} {resultCollection.FirstOrDefault()}"
               );
                return BadRequest(ModelState);
            }


            if (listOfDates.Count <= 0)
            {
                ModelState.AddModelError(
                    Strings_lang.DATE_ERROR_KEY,
                    Strings_lang.DATE_LIST_ERROR_MESSAGE
                );
                return BadRequest(ModelState);
            }

            ConcurrentBag<ExchangeRate> list = await _historicalExchangeRateService.sendRequestRemoteServer(baseCurrency, targetCurrency, listOfDates);

            if (list.Count > 0)
            {
                var avgMinMax = await _historicalExchangeRateService.getAvgMinMax(list);

                if (avgMinMax.dateMax == null || avgMinMax.dateMin == null)
                {
                    ModelState.AddModelError(
                     Strings_lang.DATE_ERROR_KEY,
                     Strings_lang.DATE_ITEM_ERROR_MESSAGE
                 );
                    return BadRequest(ModelState);
                }

                var result = await _historicalExchangeRateService.getAvgMinMaxToString(avgMinMax);

                var response = ResponseData.ok(result);

                return Ok(response);
            }
            else
            {
                ModelState.AddModelError(
                       Strings_lang.RATE_ERROR_KEY,
                       Strings_lang.RATE_CONVERT_ERROR_MESSAGE
                   );
                return BadRequest(ModelState);
            }
        }


    }
}
