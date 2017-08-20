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

namespace gearUpdater
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
                string deleteQuery = "truncate table web.dbo.tbl_gear";

                SqlCommand delete = new SqlCommand(deleteQuery, myConnection);
                try
                {
                    delete.ExecuteNonQuery();
                    Console.WriteLine("Truncate Successful");
                }
                catch (SqlException ex) { Console.WriteLine(ex.Message); }

                foreach (MemberList member in memberList)
                {
                
                    string url = "https://us.api.battle.net/wow/character/" + member.memberServer + "/" + member.memberName + "?fields=items,statistics,progression,audit&apikey=qg3vzf559ggr958xmz5z7gr8dznykqcm";
                    using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        var rawJson = wc.DownloadString(url);
                        dynamic stuff = JsonConvert.DeserializeObject(rawJson);
                        string name = stuff.name;
                        int ilvl = stuff.items.averageItemLevelEquipped;
                        int gems; try { gems = stuff.audit.emptySockets; } catch { gems = 0; }
                        int neckEnch; try { neckEnch = stuff.items.neck.tooltipParams.enchant; } catch { neckEnch = 0; }
                        int cloakEnch; try { cloakEnch = stuff.items.back.tooltipParams.enchant; } catch { cloakEnch = 0; }
                        int ring1Ench; try { ring1Ench = stuff.items.finger1.tooltipParams.enchant; } catch { ring1Ench = 0; }
                        int ring2Ench; try { ring2Ench = stuff.items.finger2.tooltipParams.enchant; } catch { ring2Ench = 0; }
                        int glovesEnch; try { glovesEnch = stuff.items.hands.tooltipParams.enchant; } catch { glovesEnch = 0; }
                        int shouldersEnch; try { shouldersEnch = stuff.items.shoulder.tooltipParams.enchant; } catch { shouldersEnch = 0; }

                        string importQuery = "insert into web.dbo.tbl_gear(gear_member, gear_ilvl, gear_neck, " +
                            "gear_ring_1, gear_ring_2, gear_cloak, gear_gloves, gear_shoulders, gear_missing_sockets) values('" +
                            name + "'," + ilvl + ", " + neckEnch + ", " + ring1Ench + "," + ring2Ench + ", " + cloakEnch + ", " +
                            glovesEnch + ", " + shouldersEnch + ", " + gems + ")";
                        SqlCommand sqlcom = new SqlCommand(importQuery, myConnection);
                        try
                        {
                            sqlcom.ExecuteNonQuery();
                            Console.WriteLine("Insert Successful");
                        }
                        catch (SqlException ex) { Console.WriteLine(ex.Message); }
                    }
                }
            }
            catch { }
            }

        }
}


        