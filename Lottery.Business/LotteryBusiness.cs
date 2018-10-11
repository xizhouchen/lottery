using CommonFunction;
using Lottery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Business
{
    public class LotteryBusiness
    {
        private int _maxBatchId = 14;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId">期号</param>
        /// <returns></returns>
        public int InsertLottery(string zjhm,int BatchId,string lotteryId,string issueId,string rewardate) {
            int rlt = 0;

            var sql = "insert into[Lottery].[dbo].[PredictRecord] values('" + issueId + "','" +
                "-" + "','" + "-" + "','" + zjhm + "','" + "-" + "','" + rewardate + "',getdate()," + BatchId + "," + lotteryId + ")";
            rlt = SQLHelper.ExecNonQuery(sql, null);
            return rlt;
        }

        public int InsertAutoRecord(int paycount,double paytotal,int paywintime,int payfailedtime,double paypoint,DateTime startDate,
            DateTime endDate,double lowestBalance,int isUp,string userId,double startB,double endB)
        {
            int rlt = 0;

            var sql = @"insert into [Lottery].[dbo].[AutoRecord] values("+paycount+","+paytotal+","+paywintime+","+payfailedtime+","+paypoint+",'"+startDate.ToString("yyyy/MM/dd HH:mm:ss")+"','"+endDate.ToString("yyyy/MM/dd HH:mm:ss") +"',"+lowestBalance+","+isUp+",'"+userId+"',"+startB+","+endB+")";
            rlt = SQLHelper.ExecNonQuery(sql, null);
            return rlt;
        }

        public bool IsExsit(string lotteryId, string issueId)
        {
            var isExsit = true;
            var sql = "select * from [Lottery].[dbo].[PredictRecord] where issueId = '" + issueId + "' and LotteryId = '"+lotteryId+"'";
            var dataTable = SQLHelper.GetDataTable(sql, null);
            if (dataTable.Rows.Count == 0)
            {
                isExsit = false;
            }
            return isExsit;
        }

        private List<DB_PredictRecord> ConvertToList(System.Data.DataTable table) {
            List<DB_PredictRecord> col = new List<DB_PredictRecord>();

            if (table.Rows.Count > 0)
            {
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    DB_PredictRecord rd = new DB_PredictRecord();
                    rd.GoodNo = table.Rows[i]["GoodNo"].ToString();
                    rd.IssueId = table.Rows[i]["IssueId"].ToString();
                    rd.RewardDate = table.Rows[i]["RewardDate"].ToString();
                    col.Add(rd);
                }
            }

            return col;
        }
        
       /// <summary>
       /// 获取开奖记录根据最近的多少期
       /// </summary>
       /// <param name="lastQS">最近的多少期</param>
       /// <returns></returns>
        public List<DB_PredictRecord> GetLoRecordsByLast(int lastQS) {
            List<DB_PredictRecord> records = new List<DB_PredictRecord>();

            var sql = "select top "+lastQS+" * from [Lottery].[dbo].[PredictRecord]"
                        +" where batchId = "+ _maxBatchId + ""
                        +" order by IssueId desc";
            var table = SQLHelper.GetDataTable(sql, null);
            records = this.ConvertToList(table);
            return records;
        }


        /// <summary>
        /// 获取开奖记录根据,时间
        /// </summary>
        /// <param name="lastQS">最近的多少期</param>
        /// <returns></returns>
        public List<DB_PredictRecord> GetLoRecordsByDate(DateTime from,DateTime to)
        {
            //to = to.AddDays(1);
            var sql = "select * from [Lottery].[dbo].[PredictRecord] where batchid = "+ _maxBatchId + " and "+
                "CreatedOn >=  '"+from.ToString("yyyy-MM-dd HH:mm:ss") + "' and CreatedOn <= '"+to.ToString("yyyy-MM-dd HH:mm:ss") +"'";
            var table = SQLHelper.GetDataTable(sql, null);
            return this.ConvertToList(table);
           
        }

        public List<DB_PredictRecord> GetRecordsByPage(int pageIndex = 0, int pageSize = 1000) {

            var sql = @"select top "+ pageSize + " * from [dbo].[PredictRecord] "+
                "where BatchId = "+ _maxBatchId + " and IssueId not in (select top "+pageIndex*1000+" IssueId from [dbo].[PredictRecord] where BatchId = 10 order by IssueId)"
                +"order by IssueId";
            var table = SQLHelper.GetDataTable(sql, null);
            return this.ConvertToList(table);

        }

        public List<DB_PredictRecord> GetLastRecordsByPage(int pageIndex = 0, int pageSize = 1440) {
            var sql = @"select top " + pageSize + " * from [dbo].[PredictRecord] " +
              "where BatchId = " + _maxBatchId + " and IssueId not in (select top " + pageIndex * 1000 + " IssueId from [dbo].[PredictRecord] where BatchId = "+_maxBatchId+" order by IssueId desc)"
              + "order by IssueId desc";
            var table = SQLHelper.GetDataTable(sql, null);
            return this.ConvertToList(table);
        }




    }
}
