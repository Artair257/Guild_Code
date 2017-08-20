using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace Tourmaline_Dashboard
{
    public partial class membership_totals : Form
    {
        public membership_totals()
        {
            InitializeComponent();


        }


        private void membership_totals_Load_1(object sender, EventArgs e)
        {
            SqlConnection myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11; MultipleActiveResultSets=true");
            myConnection.Open();
            membership_total_chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            membership_total_chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                try
                {
                    myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11");
                    myConnection.Open();
                    SqlDataReader reader;
                    membership_total_chart.Series[0].IsXValueIndexed = true;
                    membership_total_chart.Series[0].XValueType = ChartValueType.DateTime;
                    string query = "select date, member_count from web.dbo.tbl_member_totals order by date asc";
                    SqlCommand cmd = new SqlCommand(query, myConnection);
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        var date = reader.GetDateTime(0);
                        var count = reader.GetInt32(1);

                        membership_total_chart.Series[0].Points.AddXY(date, count);
                    }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error Caught: " + ex.Message);
                }



                try
                {
                    myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11");
                    myConnection.Open();
                    SqlDataReader  classReader;
                    //class_chart.Series[0].IsXValueIndexed = true;
                    class_chart.ChartAreas[0].AxisX.LabelStyle.Angle = -50;
                    class_chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                    class_chart.Series[0].XValueType = ChartValueType.String;
                    string classQuery = "select class_name, count(class_name) from web.dbo.tbl_members_v group by class_name order by class_name asc";
                    SqlCommand classCmd = new SqlCommand(classQuery, myConnection);
                    classReader = classCmd.ExecuteReader();


                    while (classReader.Read())
                    {
                        var className = classReader.GetString(0);
                        var count = classReader.GetInt32(1);

                        class_chart.Series[0].Points.AddXY(className, count);
                    }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error Caught: " + ex.Message);
                }
            }
        }

        private void membership_total_chart_Click(object sender, EventArgs e)
        {

        }
    }
}
