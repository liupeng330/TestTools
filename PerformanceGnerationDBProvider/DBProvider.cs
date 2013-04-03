using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.RandomData;
using System.Data.SQLite;
using System.Data;

namespace PerformanceGnerationDBProvider
{
    public class DBProvider
    {
        public DBProvider(string connectionString, int randomStart, int randomEnd)
        {
            this.conn = new SQLiteConnection(connectionString);
            this.randomStartValue = randomStart;
            this.randomEndValue = randomEnd;
            this.IsBusy = false;
        }

        public bool IsBusy { get; private set; }

        private SQLiteConnection conn;
        private int randomStartValue;
        private int randomEndValue;
        private RandomData randomData = new RandomData();

        private const long ConnectionId = 115372962383;
        private const string ConverionId = "476960acefcc3c4ad9e891e463c3baab";
        private const string adgroupObjectTableName = "tblFacebookAdGroup";
        private const string campaignObjectTableName = "tblFacebookCampaign";
        private const string conversionStatsAdGroupTableName = "tblConvStatsFacebookAdGroup";
        private const string conversionStatsCampaignTableName = "tblConvStatsFacebookCampaign";
        private const string conversionAccountTableName = "tblConvFacebookAccount_daily";
        private const string conversionAdGroupTableName = "tblConvFacebookAdGroup_daily";
        private const string conversionCampaignTableName = "tblConvFacebookCampaign_daily";
        private const string perfAccountTableName = "tblPerfFacebookAccount_daily";
        private const string perfAdGroupTableName = "tblPerfFacebookAdGroup_daily";
        private const string perfCampaignTableName = "tblPerfFacebookCampaign_daily";

        public event EventHandler Completed;
        public event EventHandler ReportProgress;

        private long RandomPerfStats()
        {
            return this.randomData.NextInt64(randomStartValue, randomEndValue);
        }

        private double RandomNewsFeed()
        {
            return this.randomData.NextDouble(0, 5);
        }

        private void CreateConvStatsDaily(DateTime start, DateTime end, PerformanceTableType table)
        {
            try
            {
                List<string> objectIds = new List<string>();
                string deleteCommand = "delete from {0} where ObjectId = ";
                string insertCommand =
                      "insert into {0} "
                      + "(ObjectId, ActionTypeId, ConnectionId, PostClick1Day, PostClick7Day, "
                      + "PostClick28Day, PostImpression1Day, PostImpression7Day, PostImpression28Day, "
                      + "StartDate, EndDate) " + "values "
                      + "(@ObjectId, @ActionTypeId, @ConnectionId, @PostClick1Day, @PostClick7Day, "
                      + "@PostClick28Day, @PostImpression1Day, @PostImpression7Day, @PostImpression28Day, "
                      + "@StartDate, @EndDate) ";

                if (table == PerformanceTableType.tblConvStatsFacebookAdGroup)
                {
                    objectIds = GetAllAdGroupObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, conversionStatsAdGroupTableName);
                    insertCommand = string.Format(insertCommand, conversionStatsAdGroupTableName);
                }
                else if (table == PerformanceTableType.tblConvStatsFacebookCampaign)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, conversionStatsCampaignTableName);
                    insertCommand = string.Format(insertCommand, conversionStatsCampaignTableName);
                }
                else
                {
                    return;
                }

                this.IsBusy = true;
                this.conn.Open();
                SQLiteCommand commandObject = this.conn.CreateCommand();
                int progress = 0;

                using (var ts = this.conn.BeginTransaction())
                {
                    commandObject.Transaction = ts;

                    //cleanup all related conv stats
                    foreach (var id in objectIds)
                    {
                        commandObject.CommandText = deleteCommand + id;
                        commandObject.ExecuteNonQuery();

                    }

                    //create conv stat and insert
                    commandObject.CommandText = insertCommand;

                    DateTime old_start = new DateTime(start.Year, start.Month, start.Day);
                    DateTime old_end = new DateTime(end.Year, end.Month, end.Day);
                    for (int i = 0; i < objectIds.Count; i++)
                    {
                        start = old_start;
                        end = old_end;
                        commandObject.Parameters.Clear();

                        progress = (int)((i + 1.0) / objectIds.Count * 100);
                        if (ReportProgress != null)
                        {
                            ReportProgress(this, new ReportProgressEventArgs(progress));
                        }

                        while (start <= end)
                        {
                            commandObject.Parameters.Add(new SQLiteParameter("@ObjectId", DbType.Int64) { Value = objectIds[i] });
                            commandObject.Parameters.Add(new SQLiteParameter("@StartDate", DbType.DateTime) { Value = start });
                            commandObject.Parameters.Add(new SQLiteParameter("@EndDate", DbType.DateTime) { Value = start.AddDays(1) });
                            commandObject.Parameters.Add(new SQLiteParameter("@ActionTypeId", DbType.Int32) { Value = 1 });
                            commandObject.Parameters.Add(new SQLiteParameter("@ConnectionId", DbType.Int64) { Value = ConnectionId });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostClick1Day", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostClick7Day", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostClick28Day", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostImpression1Day", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostImpression7Day", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@PostImpression28Day", DbType.Int64) { Value = RandomPerfStats() });

                            commandObject.ExecuteNonQuery();

                            start = start.AddDays(1);
                        }
                    }
                    ts.Commit();
                }
            }
            finally
            {
                if (this.Completed != null)
                {
                    this.Completed(this, new EventArgs());
                }
                this.conn.Close();
                this.IsBusy = false;
            }
        }

        public void CreateConvStatsAdGroup_daily(DateTime start, DateTime end)
        {
            CreateConvStatsDaily(start, end, PerformanceTableType.tblConvStatsFacebookAdGroup);
        }

        public void CreateConvStatsCampaign_daily(DateTime start, DateTime end)
        {
            CreateConvStatsDaily(start, end, PerformanceTableType.tblConvStatsFacebookCampaign);
        }

        private IEnumerable<string> GetAllObjectIds(ObjectTableType table)
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

        public IEnumerable<string> GetAllAdGroupObjectIds()
        {
            return GetAllObjectIds(ObjectTableType.AdGroup);
        }

        public IEnumerable<string> GetAllCampaignObjectIds()
        {
            return GetAllObjectIds(ObjectTableType.Campaign);
        }

        public long GetCampaignObjectId(long adgroupId)
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

        public void CreateConvFacebookAccountDaily(DateTime start, DateTime end, long accountId)
        {
            CreateConvFacebookDaily(start, end, PerformanceTableType.tblConvFacebookAccount_daily, accountId);
        }

        public void CreateConvFacebookAdGroupDaily(DateTime start, DateTime end, long accountId)
        {
            CreateConvFacebookDaily(start, end, PerformanceTableType.tblConvFacebookAdGroup_daily, accountId);
        }

        public void CreateConvFacebookCampaignDaily(DateTime start, DateTime end, long accountId)
        {
            CreateConvFacebookDaily(start, end, PerformanceTableType.tblConvFacebookCampaign_daily, accountId);
        }

        private void CreateConvFacebookDaily(DateTime start, DateTime end, PerformanceTableType table, long accountId)
        {
            try
            {
                List<string> objectIds = new List<string>();
                string deleteCommand = "delete from {0} where ObjectId = ";
                string insertCommandForAccount = "insert into {0} "
                      + "(Date, ConversionId, ObjectId, Conversions, ConversionRate, CostPerConversion, Revenue, ROI) values"
                      + "(@Date, @ConversionId, @ObjectId, @Conversions, @ConversionRate, @CostPerConversion, @Revenue, @ROI)";
                string insertCommandForAdGroup = "insert into {0} "
                      + "(Date, ConversionId, ObjectId, AccountId, CampaignId, Conversions, ConversionRate, CostPerConversion, Revenue, ROI) values"
                      + "(@Date, @ConversionId, @ObjectId, @AccountId, @CampaignId, @Conversions, @ConversionRate, @CostPerConversion, @Revenue, @ROI)";
                string insertCommandForCampaign = "insert into {0} "
                      + "(Date, ConversionId, ObjectId, AccountId, Conversions, ConversionRate, CostPerConversion, Revenue, ROI) values"
                      + "(@Date, @ConversionId, @ObjectId, @AccountId, @Conversions, @ConversionRate, @CostPerConversion, @Revenue, @ROI)";
                if (table == PerformanceTableType.tblConvFacebookAdGroup_daily)
                {
                    objectIds = GetAllAdGroupObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, conversionAdGroupTableName);
                    insertCommandForAdGroup = string.Format(insertCommandForAdGroup, conversionAdGroupTableName);
                }
                else if (table == PerformanceTableType.tblConvFacebookCampaign_daily)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, conversionCampaignTableName);
                    insertCommandForCampaign = string.Format(insertCommandForCampaign, conversionCampaignTableName);
                }
                else if (table == PerformanceTableType.tblConvFacebookAccount_daily)
                {
                    deleteCommand = string.Format(deleteCommand, conversionAccountTableName);
                    insertCommandForAccount = string.Format(insertCommandForAccount, conversionAccountTableName);
                }
                else
                {
                    return;
                }

                this.IsBusy = true;
                this.conn.Open();
                SQLiteCommand commandObject = this.conn.CreateCommand();
                int progress = 0;

                using (var ts = this.conn.BeginTransaction())
                {
                    commandObject.Transaction = ts;

                    //cleanup all related conv stats
                    foreach (var id in objectIds)
                    {
                        commandObject.CommandText = deleteCommand + id;
                        commandObject.ExecuteNonQuery();
                    }
                    if (!objectIds.Any() && table == PerformanceTableType.tblConvFacebookAccount_daily)
                    {
                        commandObject.CommandText = deleteCommand + accountId;
                        commandObject.ExecuteNonQuery();
                    }

                    //create conv stat and insert
                    if (table == PerformanceTableType.tblConvFacebookAdGroup_daily)
                    {
                        commandObject.CommandText = insertCommandForAdGroup;

                    }
                    else if (table == PerformanceTableType.tblConvFacebookCampaign_daily)
                    {
                        commandObject.CommandText = insertCommandForCampaign;
                    }
                    else
                    {
                        commandObject.CommandText = insertCommandForAccount;
                    }

                    DateTime old_start = new DateTime(start.Year, start.Month, start.Day);
                    DateTime old_end = new DateTime(end.Year, end.Month, end.Day);
                    for (int i = 0; i < objectIds.Count; i++)
                    {
                        start = old_start;
                        end = old_end;
                        commandObject.Parameters.Clear();

                        progress = (int)((i + 1.0) / objectIds.Count * 100);
                        if (ReportProgress != null)
                        {
                            ReportProgress(this, new ReportProgressEventArgs(progress));
                        }

                        while (start <= end)
                        {
                            commandObject.Parameters.Add(new SQLiteParameter("@Date", DbType.DateTime) { Value = start });
                            commandObject.Parameters.Add(new SQLiteParameter("@ConversionId", DbType.String) { Value = ConverionId });
                            commandObject.Parameters.Add(new SQLiteParameter("@ObjectId", DbType.Int64) { Value = objectIds[i] });
                            commandObject.Parameters.Add(new SQLiteParameter("@AccountId", DbType.Int64) { Value = accountId });
                            if (table == PerformanceTableType.tblConvFacebookAdGroup_daily)
                            {
                                commandObject.Parameters.Add(new SQLiteParameter("@CampaignId", DbType.Int64) { Value = GetCampaignObjectId(Convert.ToInt64(objectIds[i])) });
                            }
                            commandObject.Parameters.Add(new SQLiteParameter("@Conversions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@ConversionRate", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@CostPerConversion", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@Revenue", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@ROI", DbType.Double) { Value = null });
                            commandObject.ExecuteNonQuery();

                            start = start.AddDays(1);
                        }
                    }
                    if (table == PerformanceTableType.tblConvFacebookAccount_daily && !objectIds.Any())
                    {
                        start = old_start;
                        end = old_end;
                        commandObject.Parameters.Clear();
                        int total = (end - start).Days;

                        while (start <= end)
                        {
                            progress = (int)(((start - old_start).Days + 0.0) / total * 100);
                            if (ReportProgress != null)
                            {
                                ReportProgress(this, new ReportProgressEventArgs(progress));
                            }
                            commandObject.Parameters.Add(new SQLiteParameter("@Date", DbType.DateTime) { Value = start });
                            commandObject.Parameters.Add(new SQLiteParameter("@ConversionId", DbType.String) { Value = ConverionId });
                            commandObject.Parameters.Add(new SQLiteParameter("@ObjectId", DbType.Int64) { Value = accountId });
                            commandObject.Parameters.Add(new SQLiteParameter("@Conversions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@ConversionRate", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@CostPerConversion", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@Revenue", DbType.Double) { Value = null });
                            commandObject.Parameters.Add(new SQLiteParameter("@ROI", DbType.Double) { Value = null });
                            commandObject.ExecuteNonQuery();
                            start = start.AddDays(1);
                        }
                    }
                    ts.Commit();
                }
            }
            finally
            {
                if (this.Completed != null)
                {
                    this.Completed(this, new EventArgs());
                }
                this.conn.Close();
                this.IsBusy = false;
            }

        }

        public void CreatePerfAdGroupDaily(DateTime start, DateTime end, long accountId)
        {
            CreatePerfDaily(start, end, PerformanceTableType.tblPerfFacebookAdGroup_daily, accountId);
        }

        public void CreatePerfCampaignDaily(DateTime start, DateTime end, long accountId)
        {
            CreatePerfDaily(start, end, PerformanceTableType.tblPerfFacebookCampaign_daily, accountId);
        }

        public void CreatePerfAccountDaily(DateTime start, DateTime end, long accountId)
        {
            CreatePerfDaily(start, end, PerformanceTableType.tblPerfFacebookAccount_daily, accountId);
        }

        private void CreatePerfDaily(DateTime start, DateTime end, PerformanceTableType table, long accountId)
        {
            try
            {
                List<string> objectIds = new List<string>();
                string deleteCommand = "delete from {0} where ID = ";

                string insertCommandForAdGroup = "insert into {0} "
                      + "(AccountID, ID, StartDate, EndDate, Clicks, Impressions, Spent, Social_Clicks, Social_Impressions, Social_Spent, Actions, UniqueImpressions, SocialUniqueImpressions, UniqueClicks, SocialUniqueClicks, NewsFeed_Impressions, NewsFeed_Clicks, NewsFeed_AveragePosition) values"
                      + "(@AccountID, @ID, @StartDate, @EndDate, @Clicks, @Impressions, @Spent, @Social_Clicks, @Social_Impressions, @Social_Spent, @Actions, @UniqueImpressions, @SocialUniqueImpressions, @UniqueClicks, @SocialUniqueClicks, @NewsFeed_Impressions, @NewsFeed_Clicks, @NewsFeed_AveragePosition)";
                string insertCommandForCampaign = "insert into {0} "
                      + "(AccountID, ID, StartDate, EndDate, Clicks, Impressions, Spent, Social_Clicks, Social_Impressions, Social_Spent, Actions, UniqueImpressions, SocialUniqueImpressions, UniqueClicks, SocialUniqueClicks) values"
                      + "(@AccountID, @ID, @StartDate, @EndDate, @Clicks, @Impressions, @Spent, @Social_Clicks, @Social_Impressions, @Social_Spent, @Actions, @UniqueImpressions, @SocialUniqueImpressions, @UniqueClicks, @SocialUniqueClicks)";
                string insertCommandForAccount = insertCommandForCampaign;

                if (table == PerformanceTableType.tblPerfFacebookAdGroup_daily)
                {
                    objectIds = GetAllAdGroupObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, perfAdGroupTableName);
                    insertCommandForAdGroup = string.Format(insertCommandForAdGroup, perfAdGroupTableName);
                }
                else if (table == PerformanceTableType.tblPerfFacebookCampaign_daily)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, perfCampaignTableName);
                    insertCommandForCampaign = string.Format(insertCommandForCampaign, perfCampaignTableName);
                }
                else if (table == PerformanceTableType.tblPerfFacebookAccount_daily)
                {
                    deleteCommand = string.Format(deleteCommand, perfAccountTableName);
                    insertCommandForAccount = string.Format(insertCommandForAccount, perfAccountTableName);
                }
                else
                {
                    return;
                }

                this.IsBusy = true;
                this.conn.Open();
                SQLiteCommand commandObject = this.conn.CreateCommand();
                int progress = 0;

                using (var ts = this.conn.BeginTransaction())
                {
                    commandObject.Transaction = ts;

                    //cleanup all related conv stats
                    foreach (var id in objectIds)
                    {
                        commandObject.CommandText = deleteCommand + id;
                        commandObject.ExecuteNonQuery();
                    }
                    if (!objectIds.Any() && table == PerformanceTableType.tblPerfFacebookAccount_daily)
                    {
                        commandObject.CommandText = deleteCommand + accountId;
                        commandObject.ExecuteNonQuery();
                    }

                    //create conv stat and insert
                    if (table == PerformanceTableType.tblPerfFacebookAdGroup_daily)
                    {
                        commandObject.CommandText = insertCommandForAdGroup;
                    }
                    else if (table == PerformanceTableType.tblPerfFacebookCampaign_daily)
                    {
                        commandObject.CommandText = insertCommandForCampaign;
                    }
                    else
                    {
                        commandObject.CommandText = insertCommandForAccount;
                    }

                    DateTime old_start = new DateTime(start.Year, start.Month, start.Day);
                    DateTime old_end = new DateTime(end.Year, end.Month, end.Day);
                    for (int i = 0; i < objectIds.Count; i++)
                    {
                        start = old_start;
                        end = old_end;
                        commandObject.Parameters.Clear();

                        progress = (int)((i + 1.0) / objectIds.Count * 100);
                        if (ReportProgress != null)
                        {
                            ReportProgress(this, new ReportProgressEventArgs(progress));
                        }

                        while (start <= end)
                        {
                            commandObject.Parameters.Add(new SQLiteParameter("@AccountID", DbType.Int64) { Value = accountId });
                            if (table == PerformanceTableType.tblPerfFacebookAccount_daily)
                            {
                                commandObject.Parameters.Add(new SQLiteParameter("@ID", DbType.Int64) { Value = accountId });
                            }
                            else
                            {
                                commandObject.Parameters.Add(new SQLiteParameter("@ID", DbType.Int64) { Value = objectIds[i] });
                            }
                            commandObject.Parameters.Add(new SQLiteParameter("@StartDate", DbType.DateTime) { Value = start });
                            commandObject.Parameters.Add(new SQLiteParameter("@EndDate", DbType.DateTime) { Value = start.AddDays(1) });
                            commandObject.Parameters.Add(new SQLiteParameter("@Clicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Impressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Spent", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Clicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Impressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Spent", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Actions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@UniqueImpressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@SocialUniqueImpressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@UniqueClicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@SocialUniqueClicks", DbType.Int64) { Value = RandomPerfStats() });

                            if (table == PerformanceTableType.tblPerfFacebookAdGroup_daily)
                            {
                                commandObject.Parameters.Add(new SQLiteParameter("@NewsFeed_Impressions", DbType.Int32) { Value = RandomPerfStats() });
                                commandObject.Parameters.Add(new SQLiteParameter("@NewsFeed_Clicks", DbType.Int32) { Value = RandomPerfStats() });
                                commandObject.Parameters.Add(new SQLiteParameter("@NewsFeed_AveragePosition", DbType.Decimal) { Value = RandomNewsFeed() });
                            }

                            commandObject.ExecuteNonQuery();
                            start = start.AddDays(1);
                        }
                    }
                    if (table == PerformanceTableType.tblPerfFacebookAccount_daily && !objectIds.Any())
                    {
                        start = old_start;
                        end = old_end;
                        commandObject.Parameters.Clear();
                        int total = (end - start).Days;

                        while (start <= end)
                        {
                            progress = (int)(((start - old_start).Days + 0.0) / total * 100);
                            if (ReportProgress != null)
                            {
                                ReportProgress(this, new ReportProgressEventArgs(progress));
                            }
                            commandObject.Parameters.Add(new SQLiteParameter("@AccountID", DbType.Int64) { Value = accountId });
                            commandObject.Parameters.Add(new SQLiteParameter("@ID", DbType.Int64) { Value = accountId });
                            commandObject.Parameters.Add(new SQLiteParameter("@StartDate", DbType.DateTime) { Value = start });
                            commandObject.Parameters.Add(new SQLiteParameter("@EndDate", DbType.DateTime) { Value = start.AddDays(1) });
                            commandObject.Parameters.Add(new SQLiteParameter("@Clicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Impressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Spent", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Clicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Impressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Social_Spent", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@Actions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@UniqueImpressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@SocialUniqueImpressions", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@UniqueClicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.Parameters.Add(new SQLiteParameter("@SocialUniqueClicks", DbType.Int64) { Value = RandomPerfStats() });
                            commandObject.ExecuteNonQuery();
                            start = start.AddDays(1);
                        }
                    }
                    ts.Commit();
                }
            }
            finally
            {
                if (this.Completed != null)
                {
                    this.Completed(this, new EventArgs());
                }
                this.conn.Close();
                this.IsBusy = false;
            }
        }
    }
}
