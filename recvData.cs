using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace triton1
{
    static class recvData
    {
        public static string reqId;
        public static long host_time;


        public static void AsyncParseData(string json)
        {


            try
            {
                RootObject p = JsonConvert.DeserializeObject<RootObject>(json);
                reqId = p.response.requestId;
                host_time = p.response.data.ts;
                if (p.response.data.events.Count > 0)/*если нам пришел ответ с массивом данных*/
                {
                    if (p.response.data.events[0].eventData.tail != null)/*если есть какое то сообщение*/
                    {
                        if (p.response.data.events[0].eventData.tail.messages[0].outgoing != true)/*это сообщение не подтверждение успешной доставки*/
                        {
                            Logger.saveLog(json);
                            Logger.saveLog("----------------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.Green;
                          
                           
                            
                                Console.WriteLine(p.response.data.events[0].eventData.tail.messages[0].text);/*вытаскиваем месседж*/
                            
                            

                            if (p.response.data.events[0].eventData.tail.messages[0].text == "проверка связи")
                            {
                                sendData.sendMessage("На связи", "672880971");

                            }

                            if (p.response.data.events[0].eventData.tail.messages[0].text == "sfile")
                            {
                                //Console.WriteLine("введите путь к файлу");
                                //string path = Console.ReadLine();
                                sendFile.SendFile(@"C:\eclipse\notice.html");



                                /*отправляем файл*/

                            }

                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    }
                    else if (p.response.data.events[0].type == "typing")/*если тип сообщения typing значит нам печатают*/
                    {

                        Console.WriteLine("typing");
                    }
                    else if (p.response.data.events[0].type == "presence")/*тип presence  - обновление статуса контактов*/
                    {
                        Console.Write("проверен статус " + p.response.data.events[0].eventData.friendly + " ");
                        Console.WriteLine(" - " + p.response.data.events[0].eventData.state);

                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " " + e.StackTrace);

            }
        }
    }
}
