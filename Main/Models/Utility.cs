using System;
using System.Collections.Generic;
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

        public void QueueNewBuild(int id)
        {
            var JSON = @"
{ 
        'definition': {
            'id': {id}
        }
    }
";

            JSON = JSON.Replace("{id}", id.ToString());
            var ret = AzureDevOpsHttpPost<string>(JSON, new Uri(
                "https://dev.azure.com/twlab/DevOpsFight2019/_apis/build/builds?api-version=5.0"));
        }


     public  T AzureDevOpsHttpPost<T>(string JsonBody, Uri Endpoint)
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
                    var ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(retJSON);
                return ReturnObject;
            }
            catch (WebException ex)
            {
                //抓取detasils
                string WebException = GetWebException(ex);
                //重新丟exception
                throw new Exception($"\n AzureDevOpsHttpPost Exception:{ex.Message} \nResponse:{ WebException} \nEndpoint:{Endpoint} \nJSON:{JsonBody}", ex);
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
    }
}