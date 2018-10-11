using CommonFunction;
using Lottery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.PlatForm
{
    public abstract class IPlatForm
    {
       

        public string PayUrl { get; set; }

        public string SecurityCodeUrl = "http://mj.gud5100.com/api/utils/login-security-code?";

        public string PostLoginUrl = "http://mj.gud5100.com/api/web-login";

        public string BalanceUrl = "http://mj.gud5100.com/api/web-ajax/loop-page";

        public string OrderUrl = "http://mj.gud5100.com/api/game-lottery/add-order";

        public string Cookie { get; set; }

        public string LoginedCookie { get; set; }


        public Image VerImage { get; set; }

        /// <summary>
        /// 获取登录页面及验证码
        /// </summary>
        /// <returns></returns>
        public Bitmap GetLogin() {
            Random dom = new Random();
            var dd = (double)dom.Next() / (double)100;

            string loginGetUrl = SecurityCodeUrl + dd;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loginGetUrl);
            req.Method = "GET";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
            req.AllowAutoRedirect = false;//服务端重定向。一般设置false
            req.ContentType = "application/x-www-form-urlencoded";//数据一般设置这个值，除非是文件上传
            req.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
            //Stream postDataStream = req.GetRequestStream();
            //postDataStream.Write(postBytes, 0, postBytes.Length);
            //postDataStream.Close();
            req.Timeout = 5000;
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            string cookie = resp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            var resStream = resp.GetResponseStream();

            System.Drawing.Image result = System.Drawing.Image.FromStream(resStream);

            Bitmap bit = new Bitmap(result);
            //2. 获取验证码
            VerImage = bit;
            Cookie = cookie;
            return bit;
        }

        public string LoginReqeust(string username, string password, string verCode) {

            var postLoginUrl = this.PostLoginUrl;

            HttpWebRequest request = HttpWebRequest.CreateHttp(postLoginUrl);
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
            if (!string.IsNullOrEmpty(Cookie))
            {
                request.Headers.Add("Cookie", Cookie);
            }

            //var postStr = "user_name=" + txtUserName.Text + "&password=" + txtPassword.Text + "&valid_code=" + validCode + "&g_code=&wechat_web=web&wechat_app=app&u_g=0&command=login";
            var postStr = "username=" + username + "&password=" + password + "&securityCode=" + verCode + "";
            if (postStr != null)
            {
                using (var reqc = request.GetRequestStream())
                {
                    var bs = Encoding.UTF8.GetBytes(postStr);
                    reqc.Write(bs, 0, bs.Length);
                }
            }

            var rsp = (HttpWebResponse)request.GetResponse();

            var stream = rsp.GetResponseStream();
            LoginedCookie = rsp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            if (string.IsNullOrEmpty(LoginedCookie))
            {
                LoginedCookie = Cookie;
            }
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

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public virtual bool Login(string username, string password, string verCode) {
            var rspStr = this.LoginReqeust(username, password, verCode);
            if (rspStr.Contains("请求成功"))
            {
               
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <returns></returns>
        public virtual double GetBalance() {
            double balance = 0;
            try
            {
                var baUrl = this.BalanceUrl;
                var baRes = Util.getURLResponseStr(baUrl, LoginedCookie);
                var baRlt = JsonConvert.DeserializeObject<Res_Minjue_BalanceResult>(baRes);
                balance = baRlt.data.lotteryBalance;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("获取余额失败：" + ex.Message, ex);
                return balance;
            }
            return balance;
        }

        public virtual bool MakeOrder(string model = "li", int multiple = 1, string content = "0,1,2,3,4,5,6,7")
        {
            bool isSuccess = false;
            var data = System.Web.HttpUtility.UrlEncode("[{\"lottery\":\"qqmin\",\"issue\":\"\",\"method\":\"sxzuxzlh\",\"content\":\"" + content + "\",\"model\":\"" + model + "\",\"multiple\":" + multiple + ",\"code\":1976,\"compress\":false}]");
            var postParam = @"text=%5B%7B%22lottery%22%3A%22qqmin%22%2C%22issue%22%3A%22%22%2C%22method%22%3A%22sxzuxzlh%22%2C%22content%22%3A%220%2C1%2C2%2C3%2C4%2C5%2C6%2C7%22%2C%22model%22%3A%22li%22%2C%22multiple%22%3A1%2C%22code%22%3A1976%2C%22compress%22%3Afalse%7D%5D";
            postParam = "text=" + data;
            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- " + postParam);

            var res = Util.getURLResponseStr(OrderUrl, LoginedCookie, postParam);
            if (res.Contains("请求成功"))
            {
                isSuccess = true;
            }


            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- " + res);
            return isSuccess;
        }
    }
}
