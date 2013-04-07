using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace PerformanceGnerationDBProvider
{
    public abstract class PerfFacebook : DBProvider
    {
        private const string accountTableName = "tblPerfFacebookAccount_daily";
        private const string adGroupTableName = "tblPerfFacebookAdGroup_daily";
        private const string campaignTableName = "tblPerfFacebookCampaign_daily";

        protected void CreatePerfDaily(DateTime start, DateTime end, PerformanceTableType table, long accountId)
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
                    deleteCommand = string.Format(deleteCommand, adGroupTableName);
                    insertCommandForAdGroup = string.Format(insertCommandForAdGroup, adGroupTableName);
                }
                else if (table == PerformanceTableType.tblPerfFacebookCampaign_daily)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, campaignTableName);
                    insertCommandForCampaign = string.Format(insertCommandForCampaign, campaignTableName);
                }
                else if (table == PerformanceTableType.tblPerfFacebookAccount_daily)
                {
                    deleteCommand = string.Format(deleteCommand, accountTableName);
                    insertCommandForAccount = string.Format(insertCommandForAccount, accountTableName);
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
                        SettingReportProgress(progress);

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
                            SettingReportProgress(progress);

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
                CompletedProgress();
                this.conn.Close();
                this.IsBusy = false;
            }
        }
    }
}
