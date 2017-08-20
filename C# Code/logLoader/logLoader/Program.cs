using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;


namespace logLoader
{
    public class MemberList
    {
        public string memberName { get; set; }
        public string memberServer { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection myConnection;
            MemberList[] memberList = null;
            try
            {
                myConnection = new SqlConnection("server=localhost\\SQLEXPRESS01; Trusted_Connection=true;Database=web");


                string query = "select name, server from web.dbo.tbl_members where level = 110 and leave_date is null";
                using (var command = new SqlCommand(query, myConnection))
                {
                    myConnection.Open();
                    Console.WriteLine("Connection Established");
                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<MemberList>();
                        while (reader.Read())
                        {
                            list.Add(new MemberList { memberName = reader.GetString(0), memberServer = reader.GetString(1) });
                        }
                        memberList = list.ToArray();
                    }
                    Console.WriteLine("List Obtained");
                }
                string deleteQuery = "truncate table web.dbo.tbl_warcraft_logs";

                SqlCommand delete = new SqlCommand(deleteQuery, myConnection);
                try
                {
                    delete.ExecuteNonQuery();
                    Console.WriteLine("Truncate Successful");
                }
                catch (SqlException ex) { Console.WriteLine(ex.Message); }




                foreach (MemberList member in memberList)
                {
                    try {
                        Console.WriteLine("Pulling log for: " + member.memberName);
                        string url = "https://www.warcraftlogs.com:443/v1/parses/character/" + member.memberName + "/zuljin/us?metric=dps&api_key=6c7790b1d9878e0010a2c3e3df11c142";
                        using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                        {
                            var rawJson = wc.DownloadString(url);
                            dynamic stuff = JsonConvert.DeserializeObject(rawJson);

                            Console.WriteLine("JSON pulled");
                            try
                            {
                                string importQuery = "";
                                int jsonLength = stuff.Count;
                                Console.WriteLine(jsonLength);
                                Console.WriteLine(stuff[0].name);

                                string nameQuery = "insert into web.dbo.tbl_warcraft_logs (name) values('" + member.memberName + "')";

                                SqlCommand namecom = new SqlCommand(nameQuery, myConnection);
                                Console.WriteLine("Inserting Name.");
                                try
                                {
                                    namecom.ExecuteNonQuery();
                                    Console.WriteLine("Name Insert Successful.");
                                }
                                catch (SqlException ex) { Console.WriteLine("Exception: " + ex); }

                                for (int i = 0; i < jsonLength; i++)
                                {
                                    if (jsonLength <= 0)
                                    {
                                        break;
                                    }
                                    if (stuff[i].name == "Skorpyron" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_1 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Skorpyron" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_1 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Skorpyron" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_1 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Chronomatic Anomaly" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_2 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Chronomatic Anomaly" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_2 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Chronomatic Anomaly" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_2 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Trilliax" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_3 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Trilliax" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_3 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Trilliax" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_3 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Spellblade Aluriel" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_4 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Spellblade Aluriel" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_4 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Spellblade Aluriel" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_4 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Tichondrius" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_5 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Tichondrius" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_5 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                             "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Tichondrius" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_5 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Star Augur Etraeus" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_6 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Star Augur Etraeus" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_6 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Star Augur Etraeus" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_6= '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "High Botanist Tel'arn" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_7 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "High Botanist Tel'arn" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_7= '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "High Botanist Tel'arn" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_7 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Krosus" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_8 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Krosus" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_8 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Krosus" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_8 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Grand Magistrix Elisande" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_9 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Grand Magistrix Elisande" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_9 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Grand Magistrix Elisande" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_9 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Gul'dan" && stuff[i].difficulty == 3)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_normal_10 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Gul'dan" && stuff[i].difficulty == 4)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_heroic_10 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }
                                    if (stuff[i].name == "Gul'dan" && stuff[i].difficulty == 5)
                                    {
                                        importQuery = "update web.dbo.tbl_warcraft_logs set updated_date = '" + DateTime.Today + "', parse_mythic_10 = '" + stuff[i].specs[0].best_historical_percent + "' " +
                                            "where name = '" + member.memberName + "'";
                                    }



                                    SqlCommand sqlcom = new SqlCommand(importQuery, myConnection);
                                    Console.WriteLine("Beginning Insert.");
                                    try
                                    {
                                        sqlcom.ExecuteNonQuery();
                                        Console.WriteLine("Insert Successful.");
                                    }
                                    catch (SqlException ex) { Console.WriteLine("Exception: " + ex); }

                                  
                                }
                            }
                            catch (Exception ex) { Console.WriteLine("Exception: " + ex); }

                        }
                    }
                    catch (Exception ex) { Console.WriteLine("Exception: " + ex); }


                }
                String updateQuery = "update web.dbo.tbl_warcraft_logs set " +
                  "parse_normal_1 = isnull(parse_normal_1, 'N/A')," +
                  "parse_normal_2 = isnull(parse_normal_2, 'N/A')," +
                  "parse_normal_3 = isnull(parse_normal_3, 'N/A')," +
                  "parse_normal_4 = isnull(parse_normal_4, 'N/A')," +
                  "parse_normal_5 = isnull(parse_normal_5, 'N/A')," +
                  "parse_normal_6 = isnull(parse_normal_6, 'N/A')," +
                  "parse_normal_7 = isnull(parse_normal_7, 'N/A')," +
                  "parse_normal_8 = isnull(parse_normal_8, 'N/A')," +
                  "parse_normal_9 = isnull(parse_normal_9, 'N/A')," +
                  "parse_normal_10 = isnull(parse_normal_10, 'N/A')," +
                  "parse_heroic_1 = isnull(parse_heroic_1, 'N/A')," +
                  "parse_heroic_2 = isnull(parse_heroic_2, 'N/A')," +
                  "parse_heroic_3 = isnull(parse_heroic_3, 'N/A')," +
                  "parse_heroic_4 = isnull(parse_heroic_4, 'N/A')," +
                  "parse_heroic_5 = isnull(parse_heroic_5, 'N/A')," +
                  "parse_heroic_6 = isnull(parse_heroic_6, 'N/A')," +
                  "parse_heroic_7 = isnull(parse_heroic_7, 'N/A')," +
                  "parse_heroic_8 = isnull(parse_heroic_8, 'N/A')," +
                  "parse_heroic_9 = isnull(parse_heroic_9, 'N/A')," +
                  "parse_heroic_10 = isnull(parse_heroic_10, 'N/A')," +
                  "parse_mythic_1 = isnull(parse_mythic_1, 'N/A')," +
                  "parse_mythic_2 = isnull(parse_mythic_2, 'N/A')," +
                  "parse_mythic_3 = isnull(parse_mythic_3, 'N/A')," +
                  "parse_mythic_4 = isnull(parse_mythic_4, 'N/A')," +
                  "parse_mythic_5 = isnull(parse_mythic_5, 'N/A')," +
                  "parse_mythic_6 = isnull(parse_mythic_6, 'N/A')," +
                  "parse_mythic_7 = isnull(parse_mythic_7, 'N/A')," +
                  "parse_mythic_8 = isnull(parse_mythic_8, 'N/A')," +
                  "parse_mythic_9 = isnull(parse_mythic_9, 'N/A')," +
                  "parse_mythic_10 = isnull(parse_mythic_10, 'N/A')";
                SqlCommand update = new SqlCommand(updateQuery, myConnection);
                try
                {
                    update.ExecuteNonQuery();
                    Console.WriteLine("Updated NULLS.");
                }
                catch (SqlException sqlex) { Console.WriteLine("Exception: " + sqlex); }

            }
            catch { }
        }
            
    }
}


