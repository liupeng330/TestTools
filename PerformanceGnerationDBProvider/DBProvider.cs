using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.RandomData;
using System.Data.SQLite;
using System.Data;

namespace PerformanceGnerationDBProvider
{
    public abstract class DBProvider
    {
        public bool IsBusy { get; protected set; }

        protected SQLiteConnection conn;
        protected int randomStartValue;
        protected int randomEndValue;
        private RandomData randomData = new RandomData();

        protected const long ConnectionId = 115372962383;
        protected const string ConverionId = "476960acefcc3c4ad9e891e463c3baab";
        private const string adgroupObjectTableName = "tblFacebookAdGroup";
        private const string campaignObjectTableName = "tblFacebookCampaign";

        public event EventHandler Completed;
        public event EventHandler ReportProgress;

        protected void SettingReportProgress(int progress)
        {
            if (ReportProgress != null)
            {
                ReportProgress(this.ReportProgress, new ReportProgressEventArgs(progress));
            }
        }

        protected void CompletedProgress()
        {
            if (this.Completed != null)
            {
                this.Completed(this, new EventArgs());
            }
        }

        protected long RandomPerfStats()
        {
            return this.randomData.NextInt64(randomStartValue, randomEndValue);
        }
        protected double RandomNewsFeed()
        {
            return this.randomData.NextDouble(0, 5);
        }

        public abstract void CreateData(DateTime start, DateTime end, long accountId);

        protected IEnumerable<string> GetAllObjectIds(ObjectTableType table)
        {
            string selectCommand;
            if (table == ObjectTableType.AdGroup)
            {
                selectCommand = "select Id from tblFacebookAdGroup";
            }
            else if (table == ObjectTableType.Campaign)
            {
                selectCommand = "select Id from tblFacebookCampaign";
            }
            else
            {
                return null;
            }


            this.conn.Open();
            try
            {
                SQLiteCommand commandObject = this.conn.CreateCommand();
                commandObject.CommandText = selectCommand;
                IList<string> objectIds = new List<string>();

                using (SQLiteDataReader reader = commandObject.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objectIds.Add(reader.GetInt64(0).ToString());
                        }
                    }
                }

                return objectIds;
            }
            finally
            {
                this.conn.Close();
            }
        }

        protected IEnumerable<string> GetAllAdGroupObjectIds()
        {
            return GetAllObjectIds(ObjectTableType.AdGroup);
        }

        protected IEnumerable<string> GetAllCampaignObjectIds()
        {
            return GetAllObjectIds(ObjectTableType.Campaign);
        }

        protected long GetCampaignObjectId(long adgroupId)
        {
            SQLiteCommand commandObject = this.conn.CreateCommand();
            commandObject.CommandText = "select ParentId from tblFacebookAdGroup where Id = " + adgroupId;

            using (SQLiteDataReader reader = commandObject.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return reader.GetInt64(0);
                    }
                }
            }
            return -1;
        }
    }
}
