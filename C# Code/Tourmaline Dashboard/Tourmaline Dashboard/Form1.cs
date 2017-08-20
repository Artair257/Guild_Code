using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace Tourmaline_Dashboard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            this.Close(); //exit program
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            ind_chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            ind_chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            attendance_chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            attendance_chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;

            SqlConnection myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11; MultipleActiveResultSets=true");
            myConnection.Open();
            SqlDataReader reader;
            List<string> nameList = new List<string>();

            Console.WriteLine("Connection Established");
            string query = "select name from web.dbo.tbl_dashboard_v where is_raider = 1";
            SqlCommand cmd = new SqlCommand(query, myConnection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                nameList.Add(name);
            } //add all raiding member names to list
            raider_listbox.DataSource = nameList;

            attendance_chart.Series[0].XValueType = ChartValueType.DateTime;
            string dateQuery = "select att_date, count(att_member) from web.dbo.tbl_attendance group by att_date order by att_date asc";
            SqlCommand getDates = new SqlCommand(dateQuery, myConnection);
            SqlDataReader dateReader;
            dateReader = getDates.ExecuteReader();
            //int i = 0;
            attendance_chart.Series[0].IsXValueIndexed = true;

            while (dateReader.Read())
            {
                var date = dateReader.GetDateTime(0);
                var count = dateReader.GetInt32(1);

                attendance_chart.Series[0].Points.AddXY(date, count);
                
            }


            string gearQuery = "select sum(gear_ilvl)/count(gear_ilvl), sum(case when cloak_enchant = 'Missing Enchant' then 1 else 0 end) " +  
  ",sum(case when ring_1_enchant = 'Missing Enchant' then 1 else 0 end), sum(case when ring_2_enchant = 'Missing Enchant' then 1 else 0 end) " +
  ",sum(case when neck_enchant = 'Missing Enchant' then 1 else 0 end), sum(case when shoulder_enchant = 'Missing Enchant' then 1 else 0 end) " +
  ",sum(case when gloves_enchant = 'Missing Enchant' then 1 else 0 end), sum(missing_gems) from web.dbo.tbl_dashboard_v where is_raider = 1";

            SqlCommand getGear = new SqlCommand(gearQuery, myConnection);
            SqlDataReader gearReader = getGear.ExecuteReader();
            while (gearReader.Read())
            {
                try
                {
                    avg_ilvl_lbl.Text = gearReader.GetInt32(0).ToString();
                    tot_cloak_lbl.Text = gearReader.GetInt32(1).ToString();
                    tot_ring1_lbl.Text = gearReader.GetInt32(2).ToString();
                    tot_ring2_lbl.Text = gearReader.GetInt32(3).ToString();
                    tot_neck_lbl.Text = gearReader.GetInt32(4).ToString();
                    tot_shoulder_lbl.Text = gearReader.GetInt32(5).ToString();
                    tot_gloves_lbl.Text = gearReader.GetInt32(6).ToString();
                    tot_gems_lbl.Text = gearReader.GetInt32(7).ToString();
                }
                catch  { }
            }


        }

        private void raider_listbox_SelectedIndexChanged(object sender, EventArgs e) //on selecting new name
        {
            ind_chart.Series[0].Points.Clear();
            string name = raider_listbox.SelectedItem.ToString();
            string indDateQuery = "select att_date ,case when (select att_member from web.dbo.tbl_attendance where att_member = '" + name + "' and att.att_date = att_date) is not null then 1 else 0 end from web.dbo.tbl_attendance [att] group by att_date order by att_date asc";
            SqlConnection myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11; MultipleActiveResultSets=true");
            myConnection.Open();
            SqlCommand getIndData = new SqlCommand(indDateQuery, myConnection);
            SqlDataReader indDateReader = getIndData.ExecuteReader();
            ind_chart.Series[0].IsXValueIndexed = true;
            while (indDateReader.Read())
            {
                var date = indDateReader.GetDateTime(0);
                var count = indDateReader.GetInt32(1);
                ind_chart.Series[0].Points.AddXY(date, count);
            }

            string gearQuery = "select rank_name, class_name, race_name, server, gear_ilvl, cloak_enchant, neck_enchant, ring_1_enchant, ring_2_enchant, " + 
                "shoulder_enchant, gloves_enchant, missing_gems, specialization, role from web.dbo.tbl_dashboard_v where name = '" + name + "'";
            SqlCommand getGearData = new SqlCommand(gearQuery, myConnection);
            SqlDataReader gearReader = getGearData.ExecuteReader();
            while (gearReader.Read())
            {
                ind_rank_lbl.Text = "Rank: " + gearReader.GetString(0);
               // ind_class_lbl.Text = gearReader.GetString(1);
                ind_race_lbl.Text = gearReader.GetString(2) + ", " + gearReader.GetString(1);
                ind_spec_lbl.Text = gearReader.GetString(12); // + ", " + gearReader.GetString(13);
               // var server = gearReader.GetString(3);
                ind_ilvl_lbl.Text = gearReader.GetInt32(4).ToString();
                ind_cloak_lbl.Text = gearReader.GetString(5);
                ind_neck_lbl.Text = gearReader.GetString(6);
                ind_ring1_lbl.Text = gearReader.GetString(7);
                ind_ring2_lbl.Text = gearReader.GetString(8);
                ind_shoulder_lbl.Text = gearReader.GetString(9);
                ind_gloves_lbl.Text = gearReader.GetString(10);
                ind_gems_lbl.Text = gearReader.GetInt32(11).ToString();

                if (ind_cloak_lbl.Text == "Missing Enchant")
                {
                    ind_cloak_lbl.BackColor = Color.Red;
                }
                else { ind_cloak_lbl.BackColor = Control.DefaultBackColor; }
                if (ind_ring1_lbl.Text == "Missing Enchant")
                {
                    ind_ring1_lbl.BackColor = Color.Red;
                }
                else { ind_ring1_lbl.BackColor = Control.DefaultBackColor; }
                if (ind_ring2_lbl.Text == "Missing Enchant")
                {
                    ind_ring2_lbl.BackColor = Color.Red;
                }
                else { ind_ring2_lbl.BackColor = Control.DefaultBackColor; }
                if (ind_neck_lbl.Text == "Missing Enchant")
                {
                    ind_neck_lbl.BackColor = Color.Red;
                }
                else { ind_neck_lbl.BackColor = Control.DefaultBackColor; }
                if (Convert.ToInt32(ind_gems_lbl.Text) > 0)
                {
                    ind_gems_lbl.BackColor = Color.Red;
                }
                else { ind_gems_lbl.BackColor = Control.DefaultBackColor; }
                if (Convert.ToInt32(avg_ilvl_lbl.Text) > Convert.ToInt32(ind_ilvl_lbl.Text))
                {
                    ind_ilvl_lbl.BackColor = Color.Red;
                }
                else { ind_ilvl_lbl.BackColor = Control.DefaultBackColor; }
            }

            b1_n.Text = "N/A%"; b2_n.Text = "N/A%"; b3_n.Text = "N/A%"; b4_n.Text = "N/A%"; b5_n.Text = "N/A%"; b6_n.Text = "N/A%"; b7_n.Text = "N/A%"; b8_n.Text = "N/A%"; b9_n.Text = "N/A%"; b10_n.Text = "N/A%";
            b1_h.Text = "N/A%"; b2_h.Text = "N/A%"; b3_h.Text = "N/A%"; b4_h.Text = "N/A%"; b5_h.Text = "N/A%"; b6_h.Text = "N/A%"; b7_h.Text = "N/A%"; b8_h.Text = "N/A%"; b9_h.Text = "N/A%"; b10_h.Text = "N/A%";
            b1_m.Text = "N/A%"; b2_m.Text = "N/A%"; b3_m.Text = "N/A%"; b4_m.Text = "N/A%"; b5_m.Text = "N/A%"; b6_m.Text = "N/A%"; b7_m.Text = "N/A%"; b8_m.Text = "N/A%"; b9_m.Text = "N/A%"; b10_m.Text = "N/A%";

            string logQuery = "select * from web.dbo.tbl_warcraft_logs where name = '" + name + "'";
            SqlCommand getLogs = new SqlCommand(logQuery, myConnection);
            SqlDataReader logReader = getLogs.ExecuteReader();

            while (logReader.Read())
            {
                b1_n.Text = logReader.GetString(2) + "%";
                b2_n.Text = logReader.GetString(3) + "%";
                b3_n.Text = logReader.GetString(4) + "%";
                b4_n.Text = logReader.GetString(5) + "%";
                b5_n.Text = logReader.GetString(6) + "%";
                b6_n.Text = logReader.GetString(7) + "%";
                b7_n.Text = logReader.GetString(8) + "%";
                b8_n.Text = logReader.GetString(9) + "%";
                b9_n.Text = logReader.GetString(10) + "%";
                b10_n.Text = logReader.GetString(11) + "%";
                b1_h.Text = logReader.GetString(12) + "%";
                b2_h.Text = logReader.GetString(13) + "%";
                b3_h.Text = logReader.GetString(14) + "%";
                b4_h.Text = logReader.GetString(15) + "%";
                b5_h.Text = logReader.GetString(16) + "%";
                b6_h.Text = logReader.GetString(17) + "%";
                b7_h.Text = logReader.GetString(18) + "%";
                b8_h.Text = logReader.GetString(19) + "%";
                b9_h.Text = logReader.GetString(20) + "%";
                b10_h.Text = logReader.GetString(21) + "%";
                b1_m.Text = logReader.GetString(22) + "%";
                b2_m.Text = logReader.GetString(23) + "%";
                b3_m.Text = logReader.GetString(24) + "%";
                b4_m.Text = logReader.GetString(25) + "%";
                b5_m.Text = logReader.GetString(26) + "%";
                b6_m.Text = logReader.GetString(27) + "%";
                b7_m.Text = logReader.GetString(28) + "%";
                b8_m.Text = logReader.GetString(29) + "%";
                b9_m.Text = logReader.GetString(30) + "%";
                b10_m.Text = logReader.GetString(31) + "%";
               }

            
        }

        private void import_btn_Click(object sender, EventArgs e)
        {
            importForm attendanceImportForm = new importForm();
            DialogResult dr = attendanceImportForm.ShowDialog();
            if (dr != DialogResult.OK)
            {
                attendance_chart.Series[0].Points.Clear();
                SqlConnection myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11; MultipleActiveResultSets=true");
                myConnection.Open();
                SqlDataReader reader;
                List<string> nameList = new List<string>();
                Console.WriteLine("Connection Established");
                string query = "select name from web.dbo.tbl_dashboard_v where is_raider = 1";
                SqlCommand cmd = new SqlCommand(query, myConnection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    nameList.Add(name);
                } //add all raiding member names to list
                raider_listbox.DataSource = nameList;

                attendance_chart.Series[0].XValueType = ChartValueType.DateTime;
                string dateQuery = "select att_date, count(att_member) from web.dbo.tbl_attendance group by att_date order by att_date asc";
                SqlCommand getDates = new SqlCommand(dateQuery, myConnection);
                SqlDataReader dateReader;
                dateReader = getDates.ExecuteReader();
                //int i = 0;
                attendance_chart.Series[0].IsXValueIndexed = true;

                while (dateReader.Read())
                {
                    var date = dateReader.GetDateTime(0);
                    var count = dateReader.GetInt32(1);

                    attendance_chart.Series[0].Points.AddXY(date, count);

                }
            }
        }

        private void btn_member_total_Click(object sender, EventArgs e)
        {
            membership_totals membershipForm = new membership_totals();
            DialogResult mf = membershipForm.ShowDialog();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
