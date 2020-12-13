using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ExchangeRateCase.Services
{
    public  class ApiCall:IApiCall
    {
        public async Task<string> getData(string url)
        {
            try
            {
                string responseString;
              
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (var response1 = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response1.GetResponseStream()))
                    {
                       responseString = await reader.ReadToEndAsync();
                    }
                }
                return  responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
