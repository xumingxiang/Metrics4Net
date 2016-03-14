using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Metrics.Utils
{
    public static class HttpHelper
    {

        public static string HttpGet(string Url)
        {
            System.IO.Stream respStream = null;
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            catch (WebException web_ex)
            {
                if (web_ex.Response != null)
                {
                    respStream = web_ex.Response.GetResponseStream();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    return web_ex.ToString();
                }
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
            finally
            {
                if (respStream != null)
                {
                    respStream.Close();
                    respStream.Dispose();
                }
            }
        }
    }
}
