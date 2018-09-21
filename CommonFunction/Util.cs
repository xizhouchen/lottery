using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace CommonFunction
{
    public class Util
    {
        public static string getURLResponseStr(string url, string cookie = null, string postStr = null)
        {

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Timeout = 5000;
            // request.Headers["accept"] = "*/*";
            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            // request.Headers["connection"] = "Keep-Alive";
            //  request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Method = "POST";
            //request.Headers["user-agent"] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)";
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }
            //var resp = request.GetResponse();
            //resp.GetResponseStream();

            if (postStr != null)
            {
                using (var req = request.GetRequestStream())
                {
                    var bs = Encoding.UTF8.GetBytes(postStr);
                    req.Write(bs, 0, bs.Length);
                }
            }

            var rsp = request.GetResponse();

            var stream = rsp.GetResponseStream();

            List<byte> bytes = new List<byte>();
            int temp = stream.ReadByte();
            while (temp != -1)
            {
                bytes.Add((byte)temp);
                temp = stream.ReadByte();
            }
            var byteFile = bytes.ToArray();



            //stream.Write(bytes, 0, (int)stream.Length);
            string rspStr = Encoding.GetEncoding("UTF-8").GetString(byteFile);
            return rspStr;
        }

        public static List<string> PredictResult(List<LISTItem> lottery) {
            List<string> rlt = new List<string>();

            //List<Dictionary<int, string>> NumExistCount = new List<Dictionary<int, string>>();

            Dictionary<int, int> ne = new Dictionary<int, int>();

            //获取五星杀号
            var fiveMaxNum = 0;
            var fiveMaxValue = 0;

            for (var i = 0; i <= 9; i++) {
                var count = 0;
                foreach (var item in lottery) {
                    if (item.ZJHM.ToString().Contains(i.ToString()))
                    {
                        ne.Add(i, count);
                        if (fiveMaxValue < count) {
                            fiveMaxValue = count;
                            fiveMaxNum = i;
                        }
                        break;
                    }
                    else {
                        count++;
                        
                    }
                }
            }

            Dictionary<int, int> threeNe = new Dictionary<int, int>();

            //获取后三杀号
            var threeMaxNum = 0;
            var threeMaxValue = 0;

            for (var i = 0; i <= 9; i++)
            {
                var count = 0;
                foreach (var item in lottery)
                {
                    if (item.ZJHM.ToString().Substring(3).Contains(i.ToString()))
                    {
                        threeNe.Add(i, count);
                        if (threeMaxValue < count)
                        {
                            threeMaxValue = count;
                            threeMaxNum = i;
                        }
                        break;
                    }
                    else
                    {
                        count++;

                    }
                }
            }

            rlt.Add(fiveMaxNum.ToString());
            rlt.Add(threeMaxNum.ToString());

            return rlt;

        }
    }
}
