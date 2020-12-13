using ExchangeRateCaseSolution.Localization;
using System.Collections.Generic;


namespace ExchangeRateCase.Models
{
    public static class ResponseData
    {
        static public Dictionary<string, object> ok(object data) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", data);
            dict.Add("result", Strings_lang.RESULT_SUCCESS);

            return dict;
        }
    }
}
