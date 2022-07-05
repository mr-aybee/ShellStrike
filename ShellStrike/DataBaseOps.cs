using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ShellStrike
{
    public static class DataBaseOps
    {
        public static string dBColdStrikeConnectionString { get; set; }//;// = Properties.Settings.Default.dBColdStrikeConnectionString;

        public async static Task<DataSet> GetDataSetAsync(string Query, Hashtable hT)
        {
            return await Task.Run(() =>
            {
                var sqlDS = new SqlDataSource
                {
                    ConnectionString = dBColdStrikeConnectionString
                };
                sqlDS.Selecting += (object sender, SqlDataSourceSelectingEventArgs e) => e.Command.CommandTimeout = 120;
                sqlDS.SelectCommand = Query;
                if (!(hT == null))
                {
                    foreach (string x in hT.Keys)
                        sqlDS.SelectParameters.Add(x, hT[x].ToString());
                }
                sqlDS.CancelSelectOnNullParameter = false;
                DataView dV = null;
                int i = 0;
            TryAgain:
                try
                {
                    dV = (DataView)sqlDS.Select(System.Web.UI.DataSourceSelectArguments.Empty);
                }
                catch (Exception t)
                {
                    if (t.Message.Contains("was deadlocked on lock resources with another process") && (i <= 3))
                    {
                        Logger.Error($"Tried Again due to Deadlock on Query: " + Query);
                        i++;
                        goto TryAgain;
                    }
                }
                if (!(dV == null))
                    return dV.Table.DataSet;
                else
                    return null;
            });
        }


        public static DataSet GetDataSet(string Query, Hashtable hT = null)
        {

            var sqlDS = new SqlDataSource
            {
                ConnectionString = dBColdStrikeConnectionString
            };
            sqlDS.Selecting += (object sender, SqlDataSourceSelectingEventArgs e) => e.Command.CommandTimeout = 120;
            sqlDS.SelectCommand = Query;
            if (!(hT == null))
            {
                foreach (string x in hT.Keys)
                    sqlDS.SelectParameters.Add(x, hT[x].ToString());
            }
            sqlDS.CancelSelectOnNullParameter = false;
            DataView dV = (DataView)sqlDS.Select(System.Web.UI.DataSourceSelectArguments.Empty);
            if (!(dV == null))
                return dV.Table.DataSet;
            else
                return null;

        }


    }
}
