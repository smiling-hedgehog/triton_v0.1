using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace triton1
{
    /*
     step 1  - открываем файл /GET 
     step 2  - отправляем первый запрос (чтобы получить путь на который надо uploadить файл)
     step 3  - uploadим файл
         
         
         */
    public static class sendFile
    {
        private static string file_name;
        private static string file_path;
        private static long full_file_size;
        private static string upload_host;
        private static string upload_url;
        private static string complete_url;
        private static string url = "https://u.icq.net/files/init?";
        private static long time_offset;
        private static long session_id;
        private static int size_already_sent_ = 0;//сколько байт уже отправлено
        private const int max_block_size = 1048576;
        private static List<string> custom_headers = new List<string>();
        private static string full_path_file;
        private static byte[] buffer;
        private static string uUrl;
        /// <summary>
        ///Начальная проверка и получение данных из файла для upload
        /// </summary>
        private static bool openFile(string path)
        {

            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
            {
                full_file_size = file.Length;
                file_name = file.Name;
                Console.WriteLine("file_name " + file_name);
                file_path = file.DirectoryName + "/";
                Console.WriteLine("dir_name " + file_path);
                full_path_file = path;

                if (full_file_size < max_block_size)
                {
                    buffer = new byte[full_file_size];
                }
                else
                {
                    buffer = new byte[max_block_size];
                }
            }
            else {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Главная функция,передаём путь и кайфуем
        /// </summary>
        public static void SendFile(string path)
        {
            session_id = System.Convert.ToInt64(http_sender.getCurrentTimeToUnixFormat());

            if (openFile(path))
            {

                get_road_map();
                upload_file();

            }
        }
        /// <summary>
        /// Данная функция формирует запрос на сервер по результатам которого из request получаем куда надо upload-ить файл и 
        /// как закончить передачу файла
        /// </summary>
        private static void get_road_map()
        {
            string response;
            time_offset = System.Convert.ToInt64(http_sender.getCurrentTimeToUnixFormat()) - Convert.ToInt64(recvData.host_time);
            StartSession ss = new StartSession();
            string sha256 = "";
            ss.set_post_parameter("a", StartSession.token);
            ss.set_post_parameter("f", "json");
            ss.set_post_parameter("k", ss.dev_id);
            long ts = System.Convert.ToInt64(http_sender.getCurrentTimeToUnixFormat()) - time_offset;
            ss.set_post_parameter("ts", ts.ToString());
            ss.set_post_parameter("size", full_file_size.ToString());
            ss.set_post_parameter("filename", file_name);
            ss.set_post_parameter("client", http_sender.EscapeSymbol("Mail.ru Windows ICQ"));
            ss.set_post_parameter("r", ss.GUID);
            ss.set_post_parameter("language", "ru-ru");
            sha256 = http_sender.hmac_base64(StartSession.sessionKey, http_sender.EscapeSymbol(ss.getStrStartSession(url, false)));/*auto sha256 = escape_symbols(get_url_sign(ss_host.str(), params, params_, false));*/
            ss.set_post_parameter("sig_sha256", sha256);
            response = http_sender.GetAsync(url + ss.getStrPack(), true);

            Console.WriteLine(response);
            dynamic json = JObject.Parse(response);
            upload_host = (string)json["data"]["host"];
            upload_url = (string)json["data"]["url"];
            complete_url = (string)json["data"]["complete_url"];

            Console.WriteLine("upload host " + upload_host);
            Console.WriteLine("upload url " + upload_url);
            Console.WriteLine("complete_url " + complete_url);

        }
        private static void upload_file()
        {
            StartSession st = new StartSession();
            string sha256 = "";
            st.set_post_parameter("a", StartSession.token);
            st.set_post_parameter("f", "json");
            st.set_post_parameter("k", st.dev_id);
            long ts = System.Convert.ToInt64(http_sender.getCurrentTimeToUnixFormat()) - time_offset;
            st.set_post_parameter("ts", ts.ToString());
            st.set_post_parameter("r", st.GUID);
            sha256 = http_sender.hmac_base64(StartSession.sessionKey, http_sender.EscapeSymbol(st.getStrStartSession(url, true)));/*auto sha256 = escape_symbols(get_url_sign(ss_host.str(), params, params_, false));*/
            st.set_post_parameter("sig_sha256", sha256);

            //   read_file();
            send_data_to_server(st.getStrPack());
          


        }
        private static void read_file()
        {


            using (System.IO.FileStream inStream = File.Open(full_path_file,
                   FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                size_already_sent_ = inStream.Read(buffer, size_already_sent_, (((int)full_file_size < max_block_size) ? (int)full_file_size : max_block_size));
            }

        }

        private static void send_data_to_server(string data)
        {
            string resp;
            try
            {
                string url;

                url = "https://" + upload_host + upload_url + "?" + data;
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Accept = "*/*";
                wr.ContentType = "application/octet-stream";
                wr.Method = "POST";
                wr.KeepAlive = true;
      
                wr.Headers.Add("Session-ID", session_id.ToString());
                wr.Headers.Add("Content-Disposition", " attachment; filename=" + "\"" + file_name + "\"");
                string ss_range = "bytes " + size_already_sent_.ToString() + "-" + (size_already_sent_ + full_file_size - 1).ToString() + "/" + full_file_size.ToString();
                wr.Headers.Add("Content-Range", ss_range);
                wr.UserAgent = " ICQ Desktop 742343553 ic1nmMjqg7Yu-0hL 10.0.0(1999) Windows_Server_2008_R2_Service_Pack_1 PC";

                wr.ServicePoint.Expect100Continue = true;
                wr.ContentLength = full_file_size;
                Console.WriteLine("-------------------------");
                Console.WriteLine(wr.Headers);
                Console.WriteLine("-------------------------");
                using (Stream rs = wr.GetRequestStream())
                {
                    using (System.IO.FileStream inStream = File.Open(full_path_file,
                           FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        size_already_sent_ = inStream.Read(buffer, 0, buffer.Length);
                        rs.Write(buffer, 0, size_already_sent_);
                       
                    }
                }
                WebResponse response = wr.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        resp = reader.ReadToEnd();
                        Console.WriteLine(resp);
                        dynamic json = JObject.Parse(resp);
                        if ((int)json["status"] == 200)
                        {
                            sendData.sendMessage((string)json["data"]["static_url"], "672880971");

                        }
                        else {
                            Console.WriteLine("Ошибка при загрузке файла на сервер");
                        }
                    }
                }
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine(e.Message);
                Console.WriteLine("-------------------------");
            }
        }        
    }
}


