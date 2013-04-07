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
        private string dbConnectionString;
        private string currentTableName;
        private long accountId;

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
            this.generate_button.Enabled = false;
            try
            {
                dbConnectionString = @"Data Source=" + dbfilelocatioin_textBox.Text + @";pooling=False;read only=False;cache size=8000;new=False";
                accountId = Convert.ToInt64(accountId_textBox.Text);

                if (!File.Exists(dbfilelocatioin_textBox.Text))
                {
                    MessageBox.Show("DB file can not be NULL!");
                    return;
                }

                this.progressBar1.Value = 0;
                switch (currentTableName)
                {
                    case "tblConvStatsFacebookAdGroup":
                        StartWork(new ConvStatsFacebookAdGroup(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblConvStatsFacebookCampaign":
                        StartWork(new ConvStatsFacebookCampaign(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblConvFacebookAccount_daily":
                        StartWork(new ConvFacebookAccount(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblConvFacebookAdGroup_daily":
                        StartWork(new ConvFacebookAdGroup(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblConvFacebookCampaign_daily":
                        StartWork(new ConvFacebookCampaign(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblPerfFacebookAccount_daily":
                        StartWork(new PerfFacebookAccount(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblPerfFacebookAdGroup_daily":
                        StartWork(new PerfFacebookAdGroup(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    case "tblPerfFacebookCampaign_daily":
                        StartWork(new PerfFacebookCampaign(dbConnectionString, int.Parse(randomStart_textBox.Text), int.Parse(randomEnd_textBox.Text)));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.generate_button.Enabled = true;
            }
        }

        private void StartWork(DBProvider provider)
        {
            provider.ReportProgress += new EventHandler(dbProvider_ReportProgress);
            provider.Completed += new EventHandler(dbProvider_Completed);

            Thread workingThread = new Thread(() =>
            {
                provider.CreateData(this.startDate_dateTimePicker.Value, this.endDate_dateTimePicker.Value, accountId);
            });
            workingThread.Start();
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
