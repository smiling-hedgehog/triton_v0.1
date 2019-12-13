using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace triton1
{
    public static class Fetch
    {
        public static string aimsid;
        private static string FetchTile = "&f=json&r=1&timeout=1&peek=0&hidden=1";
        private static string FetchBaseUrl;
        private static int r = 1;
        public static bool isWork = true;
        private static string resp;

        public static void getContactList(string response)
        {
            /*При инициализации, fetchBaseUrl отличается присутствием дополнительного параметра first=1.
             При наличии которого сервер отдаёт список контактов*/
            Logger.saveLog(response);
            dynamic json = JObject.Parse(response);
            aimsid = (string)json["response"]["data"]["aimsid"];
            FetchBaseUrl = (string)json["response"]["data"]["fetchBaseURL"];
            /*пример //bos.icq.net/bos-d013e/aim/fetchEvents?aimsid=026.0663845947.1633452841:742343553&first=1&rnd=1553887966.752189
             далее можем GET запросом получить список контактов
  */
          //  triton1_contactlist.RootObject p = JsonConvert.DeserializeObject<triton1_contactlist.RootObject>(http_sender.GetAsync(FetchBaseUrl));
         
         /*   for (int i = 0;i< p.response.data.events[2].eventData.groups[0].buddies.Count; i++)
            {
                if (p.response.data.events[2].eventData.groups[0].buddies[i].state == "online")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(p.response.data.events[2].eventData.groups[0].buddies[i].friendly + " ");
                    Console.WriteLine(p.response.data.events[2].eventData.groups[0].buddies[i].state);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(p.response.data.events[2].eventData.groups[0].buddies[i].friendly + " ");
                    Console.WriteLine(p.response.data.events[2].eventData.groups[0].buddies[i].state);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }*/
           
        }
        public static void FetchEvent()
        {  
            string fetchUrl;
            r++;
            FetchTile = "&f=json&r=" + r + "&timeout=1&peek=0";
            fetchUrl = FetchBaseUrl + FetchTile;
              http_sender.GetAsync(fetchUrl);

            if (get_response_code(http_sender.response) == 200)
            {
                dynamic json = JObject.Parse(http_sender.response);
                FetchBaseUrl = (string)json["response"]["data"]["fetchBaseURL"];
              
                fetchUrl = FetchBaseUrl + FetchTile;
                Logger.saveLog(resp, "updateUIN.txt");
                recvData.AsyncParseData(http_sender.response);
               
            }
            else
            {
                Console.WriteLine("Error code " + get_response_code(http_sender.response));
            }
        
        }
        private static int get_response_code(string response)
        {
            int Status;
            try
            {
                Logger.saveLog(response, "log141119.txt");
                dynamic json = JObject.Parse(response);
                Status = (int)json["response"]["statusCode"];
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " " + e.HResult);
                return -200;
            }
            return Status;

        }

        public static async void AsyncUpdateState()
        {
            await Task.Run(() =>
            {
                while (isWork)
                {
                    
                    Fetch.FetchEvent();
                   
                }


            });

        }
    }
}
