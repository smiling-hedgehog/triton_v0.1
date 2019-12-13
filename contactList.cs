using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triton1_contactlist
{
    public class Urls
    {
        public string profile { get; set; }
        public string icon { get; set; }
        public string settings { get; set; }
        public string association { get; set; }
        public string icon10 { get; set; }
        public string compose { get; set; }
        public string signup { get; set; }
        public string inbox { get; set; }
    }

    public class ServiceURL
    {
        public string name { get; set; }
        public Urls urls { get; set; }
    }

    public class ServiceConfig
    {
        public string name { get; set; }
        public string friendlyName { get; set; }
        public int associated { get; set; }
    }

    public class Group
    {
        public string name { get; set; }
        public int id { get; set; }
        public List<User> buddies { get; set; }
    }
    public class User {
        public string aimId { get; set; }
        public string displayId { get; set; }

        public string friendly { get; set; }
        public string state { get; set; }
        public string userType { get; set; }
        public int official{ get; set; }
        public long lastseen { get; set; }
    }

    public class ServicePromo
    {
        public int enabled { get; set; }
        public string promoId { get; set; }
        public string service { get; set; }
    }

    public class EventData
    {
        public List<ServiceURL> serviceURLs { get; set; }
        public List<ServiceConfig> serviceConfigs { get; set; }
        public List<Group> groups { get; set; }
        public string aimId { get; set; }
        public string displayId { get; set; }
        public string friendly { get; set; }
        public string state { get; set; }
        public string userType { get; set; }
        public List<string> capabilities { get; set; }
        public string attachedPhoneNumber { get; set; }
        public int? globalFlags { get; set; }
        public string pdMode { get; set; }
        public List<string> blocks { get; set; }
        public List<string> ignores { get; set; }
        public ServicePromo servicePromo { get; set; }
        public long? lastMsgId { get; set; }
    }

    public class Event
    {
        public string type { get; set; }
        public EventData eventData { get; set; }
        public int seqNum { get; set; }
    }

    public class Data
    {
        public int pollTime { get; set; }
        public int ts { get; set; }
        public string fetchBaseURL { get; set; }
        public int fetchTimeout { get; set; }
        public int timeToNextFetch { get; set; }
        public List<Event> events { get; set; }
    }

    public class Response
    {
        public int statusCode { get; set; }
        public string statusText { get; set; }
        public Data data { get; set; }
    }

    public class RootObject
    {
        public Response response { get; set; }
    }
}
