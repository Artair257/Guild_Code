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


namespace Tourmaline_Dashboard
{
    public partial class importForm: Form
    {
        public importForm()
        {
            InitializeComponent();
        }


        private void button_submit_Click(object sender, EventArgs e)
        {
            SqlConnection myConnection;
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string query;
                string attendanceList = attendance_list_text.Text;
                string[] members = attendanceList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); //add an array entry for each line
                int memberLength = members.Length;
                int i = 0;
                foreach (string x in members) //remove -realm from line
                {
                    int index = members[i].IndexOf("-");
                    members[i] = members[i].Split('-').First();
                    i++;
                }
                i = 0;
                string date = date_picker.Value.ToString(); //get date from picker and convert it to string
                string raid = "";
                string difficulty = "";
                bool isRaid = radioButton1.Checked;
                if (isRaid)
                {
                    raid = radioButton1.Text;
                }
                isRaid = radioButton2.Checked;
                if (isRaid)
                {
                    raid = radioButton2.Text;
                }
                isRaid = radioButton3.Checked;
                if (isRaid)
                {
                    raid = radioButton3.Text;
                }
                isRaid = radioButton4.Checked;
                if (isRaid)
                {
                    raid = radioButton4.Text;
                }
                bool isDifficulty = radioButton5.Checked;
                if (isDifficulty)
                {
                    difficulty = radioButton5.Text;
                }
                isDifficulty = radioButton6.Checked;
                if (isDifficulty)
                {
                    difficulty = radioButton6.Text;
                }
                isDifficulty = radioButton7.Checked;
                if (isDifficulty)
                {
                    difficulty = radioButton7.Text;
                }

                if (raid == "" || difficulty == "")
                {
                    MessageBox.Show("Please select an option for raid and difficulty.");
                }
                else
                {
                    try
                    {
                        myConnection = new SqlConnection("server=criticallimit.win,55000; database=web; User Id=balthizar; password=Tourmaline11");
                        myConnection.Open();
                        Console.WriteLine("Connection Established.");
                        foreach (string y in members)
                        {

                            query = "insert into web.dbo.tbl_attendance(att_member, att_date, att_raid, att_difficulty) " +
                                "values('" + members[i] + "','" + date + "','" + raid + "', '" + difficulty + "')";
                            SqlCommand sqlcom = new SqlCommand(query, myConnection);
                            try
                            {
                                sqlcom.ExecuteNonQuery();
                               
                            }
                            catch (SqlException ex)
                            {
                                MessageBox.Show("Error while inesrting: " + ex);
                            }

                            i++;

                        }
                        MessageBox.Show("Insert Successful");
                    }
                    catch
                    {
                        Console.WriteLine("Error Caught");
                    }
                }
                
            }
            
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }
    }
}
