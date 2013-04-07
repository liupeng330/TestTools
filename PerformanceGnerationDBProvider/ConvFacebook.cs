using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace PerformanceGnerationDBProvider
{
    public abstract class ConvFacebook : DBProvider
    {
        private const string accountTableName = "tblConvFacebookAccount_daily";
        private const string adGroupTableName = "tblConvFacebookAdGroup_daily";
        private const string campaignTableName = "tblConvFacebookCampaign_daily";

        protected void CreateConvFacebookDaily(DateTime start, DateTime end, PerformanceTableType table, long accountId)
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
                    deleteCommand = string.Format(deleteCommand, adGroupTableName);
                    insertCommandForAdGroup = string.Format(insertCommandForAdGroup, adGroupTableName);
                }
                else if (table == PerformanceTableType.tblConvFacebookCampaign_daily)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, campaignTableName);
                    insertCommandForCampaign = string.Format(insertCommandForCampaign, campaignTableName);
                }
                else if (table == PerformanceTableType.tblConvFacebookAccount_daily)
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
                        SettingReportProgress(progress);

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
                            SettingReportProgress(progress);

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
                CompletedProgress();
                this.conn.Close();
                this.IsBusy = false;
            }
        }
    }
}
