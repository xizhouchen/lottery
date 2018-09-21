using CommonFunction;
using Lottery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lottery.Login
{
    public partial class Form1 : Form
    {
        private string _cookie;

        private string _loginCookie;
        public Form1()
        {
            InitializeComponent();
            //LogHelper.InfoLog("hahahxxx");
            //LogHelper.ErrorLog("dd", null);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            
            //3. post 登录数据

            var postLoginUrl = "https://www.blgj02.com/login";
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
            if (!string.IsNullOrEmpty(_cookie))
            {
                request.Headers.Add("Cookie", _cookie);
            }
            //resp = (HttpWebResponse)request.GetResponse();
            //resp.GetResponseStream();
            var validCode = this.txtvalidCode.Text;
            var postStr = "user_name="+txtUserName.Text+"&password="+txtPassword.Text+"&valid_code="+ validCode + "&g_code=&wechat_web=web&wechat_app=app&u_g=0&command=login";
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
            _loginCookie = rsp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
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
            Res_LoginResult rlt = new Res_LoginResult();
            try
            {
                rlt = JsonConvert.DeserializeObject<Res_LoginResult>(rspStr);
            }
            catch (Exception ex) {

                MessageBox.Show("登录失败： " + rspStr + " 异常：" + ex.Message);
            }
           
            if (rlt.success == 1)
            {
                lblStatus.Text = "登录成功";
                lblStatus.ForeColor = Color.Green;
                lblBalance.Text = rlt.data.money.ToString();
                lblBalance.ForeColor = Color.Orange;
                lblUserId.Text = rlt.data.user_id;
            }
            else {
                lblStatus.Text = "登录失败";
                lblStatus.ForeColor = Color.Red;
            }
            //return rspStr;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //1. 获取登录页面
            Random dom = new Random();
            var dd = (double)dom.Next() / (double)100;

            string loginGetUrl = "https://www.blgj02.com/valid_code?t=" + dd + "&user_name=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loginGetUrl);
            req.Method = "GET";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
            req.AllowAutoRedirect = false;//服务端重定向。一般设置false
            req.ContentType = "application/x-www-form-urlencoded";//数据一般设置这个值，除非是文件上传
            req.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
            //Stream postDataStream = req.GetRequestStream();
            //postDataStream.Write(postBytes, 0, postBytes.Length);
            //postDataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            string cookie = resp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            var resStream = resp.GetResponseStream();

            System.Drawing.Image result = System.Drawing.Image.FromStream(resStream);

            Bitmap bit = new Bitmap(result);
            //2. 获取验证码
            this.pictureBox1.Image = bit;
            _cookie = cookie;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
