using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PerformanceGnerationDBProvider;
using System.Threading;

namespace PerformanceGenerationTool
{
    public partial class Form1 : Form
    {
        private string dbFullPath;
        private string currentTableName;
        private long accountId;
        private DBProvider dbProvider;

        public Form1()
        {
            InitializeComponent();
        }

        private void DBTableSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTableName = DBTableSelection.SelectedItem.ToString();
        }

        private void generate_button_Click(object sender, EventArgs e)
        {
            dbFullPath = dbfilelocatioin_textBox.Text;
            accountId = Convert.ToInt64(accountId_textBox.Text);

            if (!File.Exists(dbFullPath))
            {
                MessageBox.Show("DB file can not be NULL!");
                return;
            }
            if (this.dbProvider == null)
            {
                this.dbProvider = new DBProvider(@"Data Source=" + dbFullPath + @";pooling=False;read only=False;cache size=8000;new=False",
                    int.Parse(randomStart_textBox.Text),
                    int.Parse(randomEnd_textBox.Text));
                this.dbProvider.ReportProgress += new EventHandler(dbProvider_ReportProgress);
                this.dbProvider.Completed += new EventHandler(dbProvider_Completed);
            }

            if (!this.dbProvider.IsBusy)
            {
                this.progressBar1.Value = 0;
                Thread workingThread;
                switch (currentTableName)
                {
                    case "tblConvStatsFacebookAdGroup":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreateConvStatsAdGroup_daily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value);
                        });
                        workingThread.Start();
                        break;
                    case "tblConvStatsFacebookCampaign":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreateConvStatsCampaign_daily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value);
                        });
                        workingThread.Start();
                        break;
                    case "tblConvFacebookAccount_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreateConvFacebookAccountDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    case "tblConvFacebookAdGroup_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreateConvFacebookAdGroupDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    case "tblConvFacebookCampaign_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreateConvFacebookCampaignDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    case "tblPerfFacebookAccount_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreatePerfAccountDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    case "tblPerfFacebookAdGroup_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreatePerfAdGroupDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    case "tblPerfFacebookCampaign_daily":
                        workingThread = new Thread(() =>
                        {
                            this.dbProvider.CreatePerfCampaignDaily(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
                        });
                        workingThread.Start();
                        break;
                    default:
                        break;
                }
            }
        }

        private void dbProvider_Completed(object sender, EventArgs e)
        {
            MessageBox.Show("Completed");
        }

        private void dbProvider_ReportProgress(object sender, EventArgs e)
        {
            ReportProgressEventArgs eventArg = e as ReportProgressEventArgs;
            if (eventArg != null)
            {
                this.Invoke(new Action(() =>
                {
                    this.progressBar1.Value = eventArg.Progress;
                }));
            }
        }
    }
}
