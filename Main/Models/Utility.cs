 
using AzureDevOps.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Main
{
    public class AzureDevOpsRestApiClient
    {
        public string UserName { get; set; }
        public string UserPAT { get; set; }
        public string OrganizationName { get; set; }
        public string ProjectName { get; set; }

        private string BasicAuthToken { get; set; }

        public AzureDevOpsRestApiClient(string UserName, string UserPAT, string OrganizationName, string ProjectName)
        {
            this.UserName = UserName;
            this.UserPAT = UserPAT;
            this.OrganizationName = OrganizationName;
            this.ProjectName = ProjectName;

            byte[] bytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes($"{UserName}:{UserPAT}");
            this.BasicAuthToken = Convert.ToBase64String(bytes);
        }

        public QueueBuildResult QueueNewBuild(int id)
        {
            var JSON = @"
{ 
        'definition': {
            'id': {id}
        }
    }
";

            JSON = JSON.Replace("{id}", id.ToString());
            var ret = AzureDevOpsHttpPost<QueueBuildResult>(JSON, new Uri(
                $"https://dev.azure.com/{OrganizationName}/{ProjectName}/_apis/build/builds?api-version=5.0"));

            return ret;
        }

        public GetDefinitionsResult GetDefinitions()
        {
            var ret = AzureDevOpsHttpGet<GetDefinitionsResult>(new Uri(
                $"https://dev.azure.com/{OrganizationName}/{ProjectName}/_apis/build/definitions?api-version=5.0"));

            return ret;
        }


        public MakeApprove.MakeApproveResult MakeApprove(int approvalId, string comments)
        {
            var JSON = @"
{
  ""status"": ""approved"",
  ""comments"": ""Good to go!""
}
";

            //JSON = JSON.Replace("{id}", id.ToString());
            var ret = AzureDevOpsHttpPatch<MakeApprove.MakeApproveResult>(JSON, new Uri(
                $"https://vsrm.dev.azure.com/{OrganizationName}/{ProjectName}/_apis/release/approvals/{approvalId}?api-version=5.1"));

            return ret;
        }


        public Approves.GetApproversResult GetApprovers()
        {
            var ret = AzureDevOpsHttpGet<Approves.GetApproversResult>(new Uri(
                $"https://vsrm.dev.azure.com/{OrganizationName}/{ProjectName}/_apis/release/approvals?api-version=5.1"));

            return ret;
        }

        #region "http utility"
        public T AzureDevOpsHttpPatch<T>(string JsonBody, Uri Endpoint)
        {
            try
            {
                //Call API
                WebClient wc = new WebClient();
                wc.Headers.Clear();
                wc.Headers.Add("Content-Type", "application/json");
                wc.Headers.Add("Authorization", "Basic " + BasicAuthToken);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(JsonBody);
                byte[] result = wc.UploadData(Endpoint, "PATCH", byteArray);
                var retJSON = System.Text.Encoding.UTF8.GetString(result);
                if (typeof(T).Equals(typeof(string)))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, retJSON);
                }
                else
                {
                    var ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(retJSON);
                    return ReturnObject;
                }
            }
            catch (WebException ex)
            {
                //抓取detasils
                string WebException = GetWebException(ex);
                //重新丟exception
                throw new Exception($"\n AzureDevOpsHttpPost Exception:{ex.Message} \nResponse:{ WebException} \nEndpoint:{Endpoint} \nJSON:{JsonBody}", ex);
            }
        }

        public T AzureDevOpsHttpPost<T>(string JsonBody, Uri Endpoint)
        {
            try
            {
                //Call API
                WebClient wc = new WebClient();
                wc.Headers.Clear();
                wc.Headers.Add("Content-Type", "application/json");
                wc.Headers.Add("Authorization", "Basic " + BasicAuthToken);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(JsonBody);
                byte[] result = wc.UploadData(Endpoint, byteArray);
                var retJSON = System.Text.Encoding.UTF8.GetString(result);
                if (typeof(T).Equals(typeof(string)))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, retJSON);
                }
                else
                {
                    var ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(retJSON);
                    return ReturnObject;
                }
            }
            catch (WebException ex)
            {
                //抓取detasils
                string WebException = GetWebException(ex);
                //重新丟exception
                throw new Exception($"\n AzureDevOpsHttpPost Exception:{ex.Message} \nResponse:{ WebException} \nEndpoint:{Endpoint} \nJSON:{JsonBody}", ex);
            }
        }
        public T AzureDevOpsHttpGet<T>( Uri Endpoint)
        {
            try
            {
                //Call API
                WebClient wc = new WebClient();
                wc.Headers.Clear();
                wc.Headers.Add("Content-Type", "application/json");
                wc.Headers.Add("Authorization", "Basic " + BasicAuthToken);
                byte[] result = wc.DownloadData(Endpoint);
                var retJSON = System.Text.Encoding.UTF8.GetString(result);
          
                if (typeof(T).Equals(typeof(string)))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, retJSON);
                }
                else
                {
                    var ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(retJSON);
                    return ReturnObject;
                }
            }
            catch (WebException ex)
            {
                //抓取detasils
                string WebException = GetWebException(ex);
                //重新丟exception
                throw new Exception($"\n AzureDevOpsHttpGet Exception:{ex.Message} \nResponse:{ WebException} \nEndpoint:{Endpoint}", ex);
            }
        }

        private static string GetWebException(WebException ex)
        {
            try
            {
                string responseString;
                using (Stream stream = ex.Response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
                return responseString;
            }
            catch (Exception innerEx)
            {
                throw new Exception("GetWebException", innerEx);
            }
        }
        #endregion
    }
}