using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace PerformanceGnerationDBProvider
{
    public class ConvFacebookAccount : ConvFacebook
    {
        public ConvFacebookAccount(string connectionString, int randomStart, int randomEnd)
        {
            this.conn = new SQLiteConnection(connectionString);
            this.randomStartValue = randomStart;
            this.randomEndValue = randomEnd;
            this.IsBusy = false;
        }

        public override void CreateData(DateTime start, DateTime end, long accountId)
        {
            CreateConvFacebookDaily(start, end, PerformanceTableType.tblConvFacebookAccount_daily, accountId);
        }
    }
}
