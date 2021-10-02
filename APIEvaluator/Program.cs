using APIEvaluator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace APIEvaluator
{
    class Program
    {
        private static string conclusion = "The response from the API is what we are expecting!";
        static void Main(string[] args)
        {
            //string json = File.ReadAllText(@"C:\Assessments\Bitventure\basic_endpoints.json", Encoding.UTF8);
            string json = File.ReadAllText(@"C:\Assessments\Bitventure\bonus_endpoints.json", Encoding.UTF8);
            if (IsValidJson(json))
            {
                ProcessFile(json);
            }
            else
                Console.WriteLine("Invalid JSON file!");
        }

        private static void ProcessFile(string JSONString)
        {
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(JSONString);

                foreach (var service in obj.services)
                {
                    Service objservice = (Service)JsonConvert.DeserializeObject(service.ToString(), typeof(Service));
                    if (objservice.enabled)
                    {
                        foreach (EndPoint endPoint in objservice.EndPointList)
                        {
                            string URL = objservice.baseURL + endPoint.resource;
                            conclusion = "The response from the endpoint " + URL + " is what we are expecting!";
                            if (endPoint.enabled)
                            {
                                string apiResponse = CallAPI(URL, objservice.datatype);

                                if (objservice.datatype.ToLower() == "xml")
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(apiResponse);

                                    apiResponse = JsonConvert.SerializeXmlNode(doc);
                                }

                                var responseObject = JsonConvert.DeserializeObject<JObject>(apiResponse);

                                JToken token = JToken.Parse(apiResponse);
                                foreach (Response response in endPoint.ResponseList)
                                {
                                    if (objservice.IdentifierList != null && objservice.IdentifierList.Where(i => i.key == response.identifier).Any())
                                        response.regex = response.regex == null ? objservice.IdentifierList.Where(i => i.key == response.identifier).FirstOrDefault().value : response.regex;

                                    if (IsElementAvailable(token, response.element, response.regex))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine(response.element + " : " + response.regex + ". Element found");
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine(response.element + " : " + response.regex + ". Element not found");
                                        conclusion = "The response from the endpoint " + URL + " is NOT what we are expecting!";
                                    }
                                }

                                PrintResults(conclusion);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private static bool IsElementAvailable(JToken token, string key, string value)
        {
            try
            {
                if (token is JProperty && token.First is JValue)
                {
                    if (((JProperty)token).Name == key && ((JProperty)token).Value.ToString() == value)
                    {
                        return true;
                    }
                }

                foreach (JToken token2 in token.Children())
                {
                    if (IsElementAvailable(token2, key, value))
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || 
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) 
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static string CallAPI(string URL, string dataType)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/" + dataType.ToLower()));
                HttpResponseMessage response = client.GetAsync("").Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void PrintResults(string conclusion)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("CONCLUSION");
            Console.WriteLine(conclusion);
            Console.WriteLine("==========================================================================================");
            Console.WriteLine();
        }
    }
}
