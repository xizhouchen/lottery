using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
    public class CanBePayPoint
    {
        public int Id { get; set; }
        public string PayIssueId { get; set; }

        public string StartDate { get; set; }

        public string PayDate { get; set; }

        public int RestPayCount { get; set; }


        public string StartIssueId { get; set; }

        public int PassCount { get; set; }

        public int WinCount { get; set; }

        public double WinRate { get; set; }

        public DateTime StartDateTime { get; set; }

        public double Balance { get; set; }

        public string IsWin { get; set; }

        public DateTime EndDateTime { get; set; }

        public double MaxBalance { get; set; }

        /// <summary>
        /// 能被下单的数量
        /// </summary>
       

        public string StopIssueId
        {
            get
            {
                return null;
                var temp = this.PayIssueId.Split(new char[] { '-' });
                int first = int.Parse(temp[1]);
                int last = first + this.RestPayCount;
                if (last > 1440)
                {
                    DateTime dt = DateTime.Parse(temp[0].Substring(0, 4) + "-" + temp[0].Substring(4, 2) + "-" + temp[0].Substring(6, 2));
                    dt = dt.AddDays(1);
                    var value = last - 1440;
                    if (value < 10)
                    {
                        return dt.ToString("yyyyMMdd") + "-000" + value;
                    }
                    else if (value < 100)
                    {
                        return dt.ToString("yyyyMMdd") + "-00" + value;
                    }
                    else if (value < 1000)
                    {
                        return dt.ToString("yyyyMMdd") + "-0" + value;
                    }
                    return dt.ToString("yyyyMMdd") + "-" + value.ToString();
                }
                else {
                    if (last < 10)
                    {
                        return temp[0] + "-000" + last;
                    }
                    else if (last < 100)
                    {
                        return temp[0] + "-00" + last;
                    }
                    else if (last < 1000) {
                        return temp[0] + "-0" + last;
                    }
                    return temp[0] + "-" + last;
                }
                
            }
           
        }
    }
}
