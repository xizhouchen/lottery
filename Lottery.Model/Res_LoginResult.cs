using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
 

    public class LoginData
    {
        /// <summary>
        /// 
        /// </summary>
        public string reg_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reg_ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string money { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string has_proxy_bonus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string parent_qq { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string prev_login_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string has_proxy_wage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string money_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string prev_login_ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string login_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string login_ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string u_num { get; set; }
    }

    public class Res_LoginResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string x_hit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LoginData data { get; set; }
    }
}
