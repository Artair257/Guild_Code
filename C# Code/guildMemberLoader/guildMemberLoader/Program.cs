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


namespace guildMemberLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://us.api.battle.net/wow/guild/zul'jin/blessed%20ignorance?fields=members&locale=en_US&apikey=qg3vzf559ggr958xmz5z7gr8dznykqcm";
            SqlConnection myConnection;
            using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8})
            {
                var rawJson = wc.DownloadString(url);
                dynamic stuff = JsonConvert.DeserializeObject(rawJson);
            //    
                int i = 1;
                int x = 0;


                try
                {
                    myConnection = new SqlConnection("server=localhost\\SQLEXPRESS01; Trusted_Connection=true;Database=web");
                    myConnection.Open();
                    Console.WriteLine("Connection Established");

                    do
                    {
                        string spec = "";
                        string role = "";
                        string name = stuff.members[x].character.name;
                        string realm = stuff.members[x].character.realm;
                        int charClass = stuff.members[x].character.@class;
                        int race = stuff.members[x].character.race;
                        int level = stuff.members[x].character.level;
                        int achievementPoints = stuff.members[x].character.achievementPoints;
                        int rank = stuff.members[x].rank;
                        try
                        {
                            spec = stuff.members[x].character.spec.name;
                            role = stuff.members[x].character.spec.role;
                        }
                        catch { Console.WriteLine("No Spec Data Present."); }

                        name = name.Replace("'", "''");
                        realm = realm.Replace("'", "''");
                        string query = "create table #temp(temp_name nchar(100), temp_server varchar(50), temp_class int, temp_race int, temp_level int, temp_achPoints int, temp_rank int, temp_spec varchar(50), temp_role varchar(15)) " +
                         "insert into #temp(temp_name, temp_server, temp_class, temp_race, temp_level, temp_achPoints, temp_rank, temp_spec, temp_role) " +
                         "select '" + name + "', '" + realm + "', " + charClass + ", " + race + ", " + level + ", " + achievementPoints + ", " + rank + ", '" + spec + "', '" + role +
                         "' merge web.dbo.tbl_members as tgt using #temp as src on temp_name = name when not matched by target then insert " +
                         "(name, server, class, race, level, achievement_points, guild_rank,specialization, role, created_date, updated_date, leave_date) " +
                         "values ('" + name + "', '" + realm + "', " + charClass + ", " + race + ", " + level + ", " + achievementPoints + ", " + rank + ", '" + spec + "', '" + role + "', getDate(), getDate(), null) " +
                         "when matched then update set name = '" + name + "', server = '" + realm + "', class = " + charClass + ", race = " + race + ", level = " + level + ", achievement_points = " + achievementPoints + ", guild_rank = " + rank + ", specialization = '" + spec + "', role = '" + role + "', updated_date = getDate(), leave_date = null; " +
                         "drop table #temp";
                      //   "when not matched by source then update updated_date = getdate(), leave_date = get_date()";
                       
                       
                        SqlCommand sqlcom = new SqlCommand(query, myConnection);
                        try
                        {
                            sqlcom.ExecuteNonQuery();
                            Console.WriteLine("Insert Successful");
                        }
                        catch(SqlException ex) { Console.WriteLine(ex.Message); }




                        //Console.WriteLine(query);
                        Console.WriteLine(name + " " + realm + " " + charClass + " " + rank);
                        x++;
                    } while (i < 2);



                }
                catch
                {
                    Console.WriteLine("Error Caught");
                    Console.WriteLine(x);
                   
                }

                myConnection = new SqlConnection("server=localhost\\SQLEXPRESS01; Trusted_Connection=true;Database=web");
                myConnection.Open();
                string leave = "update web.dbo.tbl_members set leave_date = updated_date where updated_date <> convert(date, getdate()) and leave_date is null";
                SqlCommand sqlleave = new SqlCommand(leave, myConnection);
                try
                {
                    sqlleave.ExecuteNonQuery();
                    Console.WriteLine("Created Date Set");
                }
                catch (SqlException ex) { Console.WriteLine(ex.Message); Console.WriteLine("Update Error"); }

                string memberCountQuery = "insert into web.dbo.tbl_member_totals (date, member_count) values(convert(date, getdate()), (select count(name) from web.dbo.tbl_members where leave_date is null))";
                SqlCommand memberCount = new SqlCommand(memberCountQuery, myConnection);
                try
                {
                    memberCount.ExecuteNonQuery();
                    Console.WriteLine("Member Count Table Updated");
                }
                catch (SqlException ex) { Console.WriteLine(ex.Message); Console.WriteLine("Update Error"); }

            }
        }
    }
}
