using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

namespace SharepointProxy.Controllers
{
    public class SharepointController : ApiController
    {
       Uri baseUrl = new Uri("http://net-sp");
      //  Uri baseUrl = new Uri("http://31.47.33.146:90");
        NetworkCredential credential = new NetworkCredential("spadmin", "Nsr!dm$n!Sp", "nasr2");

        [AcceptVerbs("GET", "POST")]
        public async Task<HttpResponseMessage> RequestHandler(string url)
        {
            WebClient wc = new WebClient();
            wc.Credentials = credential;
            HashSet<string> HDR = new HashSet<string>() { "Accept", "X-RequestDigest" };
            foreach (var headerStr in HDR)
            {
                IEnumerable<string> val = new List<string>();
                if  (Request.Headers.TryGetValues(headerStr,out val )){
                    wc.Headers.Add(headerStr, val.FirstOrDefault());
                }
            }
           
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose; charset=UTF-8");
            //wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
           
            String json;
            if (this.Request.Method == HttpMethod.Get)
            {
                json = wc.DownloadString(new Uri(baseUrl, url+Request.RequestUri.Query));
            }
            else
            {
                json = wc.UploadString(new Uri(baseUrl, url+Request.RequestUri.Query), await this.Request.Content.ReadAsStringAsync());
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            return response;
        }
    }

}
