using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Text;

namespace BuildMonitor.Domain
{
    class PtrackClient
    {
        static private HttpStatusCode DoRequest(string method, string uri, string body, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.Timeout = 10 * 60 * 1000; // 10 minutes

            if (body != null)
            {
                request.ContentType = contentType;
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(body);
                }
            }

            //string base64Credentials = GetEncodedCredentials();
            //request.Headers.Add("Authorization", "Basic " + base64Credentials);

            request.Accept = "*/*";

            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                string result = string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                return response.StatusCode;
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                // can use ex.Response.Status, .StatusDescription
                if (response.ContentLength != 0)
                {
                    string result = string.Empty;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }

                    return response.StatusCode;
                }
                else
                {
                    return response.StatusCode;
                }
            }
        }

        public static void SendJsonDocument(string path, string json)
        {
            var ptserver = System.Environment.GetEnvironmentVariable("PROJECTTRACKSERVER");
            var ptport = System.Environment.GetEnvironmentVariable("PROJECTTRACKSERVERPORT");
            var ptid = System.Environment.GetEnvironmentVariable("PROJECTTRACKSERVERID");

            if (ptserver == "" || ptport == "" || ptid == "")
                return;

            string uri = String.Format("http://{0}:{1}/api/{2}/{3}", ptserver, ptport, ptid, path);

            var sb = new StringBuilder();
            sb.Append("docs=[");
            sb.Append(json);
            sb.Append("]");

            try
            {
                DoRequest("POST", uri, sb.ToString(), "application/x-www-form-urlencoded");
            }
            catch(Exception)
            {
            }
        }
    }

    public class BuildRepository : IBuildRepository
    {
        private readonly JsonSerializer serializer;

        public BuildRepository(string pathToDb)
        {
            if(string.IsNullOrEmpty(pathToDb))
                throw new ArgumentNullException("pathToDb");

            Source = pathToDb;
            serializer = new JsonSerializer {Formatting = Formatting.Indented};
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
        }

        public string Source { get; private set; }

        public void Save(IPersistable build)
        {
            if (build as ISolutionBuild != null) // Only solution builds
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, build.Data());

                    }

                    PtrackClient.SendJsonDocument("visualStudioReport", sw.ToString());
                }
            }

            
            //using (var fileStream = new FileStream(Source, FileMode.Open, FileAccess.ReadWrite))
            //{
            //    if (fileStream.Length > 0) //remove ] if we have content in file
            //    {
            //        fileStream.Seek(-1, SeekOrigin.End);
            //    }
            //    using (var sw = new StreamWriter(fileStream)) //add item to array
            //    {
            //        using (JsonWriter writer = new JsonTextWriter(sw))
            //        {
            //            if (sw.BaseStream.Position == 0) //begin array if this is first item
            //            {
            //                writer.WriteRawValue("[");
            //            }
            //            else //we are adding item to existing array
            //            {
            //                writer.WriteRawValue(",");
            //            }

            //            serializer.Serialize(writer, build.Data());

            //            writer.WriteRawValue("]");
            //        }
            //    }
            //}
        }

        public void Save(string data)
        {
            var d = JsonConvert.DeserializeObject<IEnumerable<object>>(data);
            var jsonSerializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());
            var s = JsonConvert.SerializeObject(d, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(Source, s);
        }

        public string GetRawData()
        {
            return File.ReadAllText(Source);
        }
    }
}