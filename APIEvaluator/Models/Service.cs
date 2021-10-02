using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIEvaluator.Models
{
    public class ServiceData
    {
        public string services { get; set; } 

        [JsonProperty("services")]
        public List<Service> ServiceList { get; set; } 
    }
    public class Service
    {
        [JsonProperty("baseURL")]
        public string baseURL { get; set; }

        [JsonProperty("datatype")]
        public string datatype { get; set; }

        [JsonProperty("enabled")]
        public bool enabled { get; set; }

        [JsonProperty("endpoints")]
        public List<EndPoint> EndPointList { get; set; }

        [JsonProperty("identifiers")]
        public List<Identifier> IdentifierList { get; set; }
    }

    public class EndPoint
    {
        [JsonProperty("enabled")]
        public bool enabled { get; set; }

        [JsonProperty("resource")]
        public string resource { get; set; }

        [JsonProperty("response")]
        public List<Response> ResponseList { get; set; }
    }

    public class Response
    {
        [JsonProperty("element")]
        public string element { get; set; }

        [JsonProperty("regex")]
        public string regex { get; set; }

        [JsonProperty("identifier")]
        public string identifier { get; set; }
    }

    public class Identifier
    {
        [JsonProperty("key")]
        public string key { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }
    }
}
