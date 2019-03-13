using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace SharepointProxy.Controllers
{
    public class testSHController : ApiController
    {
        public class RequestParams
        {
            public String url { get; set; }
            public String body { get; set; }
        }
        WebClient client = new WebClient();


        private string RequestFormDigest()
        {
            var endpointUri = new Uri(WebUri, "_api/contextinfo");
            var result = client.UploadString(endpointUri, "POST");
            JToken t = JToken.Parse(result);
            return t["d"]["GetContextWebInformation"]["FormDigestValue"].ToString();
        }

        public Uri WebUri = new Uri("http://net-sp/");



        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage GetRequest([FromBody]RequestParams rparams)
        {
            //  WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("spadmin", "Nsr!dm$n!Sp", "nasr2");
            //client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            //client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            //client.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
            //var myUrl = "/_api/web/lists(guid'25A95124-E0C6-4681-AF69-69E76DB9B865')/items";
            //var contactEntry = new
            //{
            //    __metadata = new { type = "SP.Data.List3ListItem" },
            //    Title = "John Doe3"
            //};
            var formDigestValue = RequestFormDigest();
            client.Headers.Add("X-RequestDigest", formDigestValue);
            var endpointUri = new Uri(WebUri, rparams.url);
           //var payloadString2 = JsonConvert.SerializeObject(contactEntry);
            var payloadString = (rparams.body);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            var json = client.UploadString(endpointUri, payloadString);

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            return response;
        }

    }
}

