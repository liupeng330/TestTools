using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace PerformanceGnerationDBProvider
{
    public abstract class ConvStatsFacebook : DBProvider
    {
        protected const string adGroupTableName = "tblConvStatsFacebookAdGroup";
        protected const string campaignTableName = "tblConvStatsFacebookCampaign";

        protected void CreateConvStatsDaily(DateTime start, DateTime end, PerformanceTableType table)
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
                    deleteCommand = string.Format(deleteCommand, adGroupTableName);
                    insertCommand = string.Format(insertCommand, adGroupTableName);
                }
                else if (table == PerformanceTableType.tblConvStatsFacebookCampaign)
                {
                    objectIds = GetAllCampaignObjectIds().ToList();
                    deleteCommand = string.Format(deleteCommand, campaignTableName);
                    insertCommand = string.Format(insertCommand, campaignTableName);
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
                        SettingReportProgress(progress);

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
                this.CompletedProgress();
                this.conn.Close();
                this.IsBusy = false;
            }
        }
    }
}
