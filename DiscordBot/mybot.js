//Version 2.3
//Updated 2/23/2017

var Discord = require("discord.js");
var bot = new Discord.Client();

var sql = require("mssql");

var request = require("request");
var tokenurl = "https://wowtoken.info/wowtoken.json";

var config = {
    server: 'localhost\\SQLEXPRESS01',
    database: 'web',
    driver: 'msnodesqlv8',
    options: { trustedConnection: true }
};


const blizzard = require("blizzard.js").initialize({ apikey: 'qg3vzf559ggr958xmz5z7gr8dznykqcm' });

var currentTimestamp;
var newsFeedOn = false;
//var channelId = "184469791409438720"; //criticallimit test general text channel
var channelId = "220948566380642304";   //tourmaline production general text channel

var savage = ["What a god!"
, "What a scrub."
, "Please consider getting good."
, "/gkick for the love of god."
, "Maybe Hello Kitty Island Adventure would be a better time investment."
, "I'm a bot and even I find this appalling."
, "Guess that application to Big Crits is getting denied."
, "Yep, they seem like Blessed Ignorance material."
, "How about you actually use that health potion?"
, "Maybe roll Shadow so you have an excuse."];

bot.on("message", msg => {
	//if (bot.memberHasRole(msg.author, msg.server.roles.get("name", "Officer"))) {
	//	bot.sendMessage("pong");
	//}

	
    if (msg.content.startsWith("!halp")) { //list commands and parameters
        msg.channel.sendMessage("Hi, I'm rip-bot! My command syntax is listed below.\n!rip <player_name> <server_name> : Displays player deaths.\n!legendary <legendary_item_name> : Display Legendary Item Effect.\n" + 
		"!news <on/off> :Turns on guild news feed.\n!status : Shows module status.\n!token : Shows current WoW Token Pricing.\n" + 
		"!member <name> : Displays info about guild member.");
    } //end !help

	if (msg.content.startsWith("!rules") && msg.author.bot === false) {
		var sendUser = msg.author;
		sendUser.sendMessage("");
		console.log("Rules sent to " + msg.author.username);
	}
	
	if (msg.content.startsWith("!updatelog")) {
		msg.channel.sendMessage("Updating warcraft log table, this may take a few minutes.");
		var exec = require('child_process').exec;
			console.log("Starting up logLoader.exe");
			var child = exec("\"C:\\Users\\administrator.LOCAL\\Documents\\Visual Studio 2015\\Projects\\logLoader\\logLoader\\bin\\Debug\\logLoader.exe\"", function(error, stdout, stderr)
			{
				if (error != null)
				{
					console.log(stderr);
					msg.channel.sendMessage("Error with log updating.");
				}
			});
	}
		if (msg.content.startsWith("!updatemember")) {
		msg.channel.sendMessage("Updating members table.");
		var exec = require('child_process').exec;
			console.log("Starting up guildMemberLoader.exe");
			var child = exec("\"C:\\Users\\administrator.LOCAL\\Documents\\Visual Studio 2015\\Projects\\guildMemberLoader\\guildMemberLoader\\bin\\Debug\\guildMemberLoader.exe\"", function(error, stdout, stderr)
			{
				if (error != null)
				{
					console.log(stderr);
					msg.channel.sendMessage("Error with log updating.");
				}
			});
	}
		if (msg.content.startsWith("!updategear")) {
		msg.channel.sendMessage("Updating gear table.");
		var exec = require('child_process').exec;
			console.log("Starting up gearUpdater.exe");
			var child = exec("\"C:\\Users\\administrator.LOCAL\\Documents\\Visual Studio 2015\\Projects\\gearUpdater\\gearUpdater\\bin\\Debug\\gearUpdater.exe\"", function(error, stdout, stderr)
			{
				if (error != null)
				{
					console.log(stderr);
					msg.channel.sendMessage("Error with log updating.");
				}
			});
	}
	
	
	
    if (msg.content.startsWith("!status")) {
        msg.channel.sendMessage("Bot is currently Active.");
        if (newsFeedOn == true) {
            msg.channel.sendMessage("Guild News Feed is On.");
        }
        else {
            msg.channel.sendMessage("Guild News Feed is Off.");
        }
    }
	
	if (msg.content.startsWith("!member")) {
		var inName = msg.content;
		inName = inName.replace("!member ", "");
		inName = inName.replace("'","''");
		console.log("Searching for member " + inName);
		var connectionString = "select top 1 name, server, class_name, race_name, level, achievement_points, rank_name from web.dbo.tbl_members_v where name like '%" + inName + "%'";
		var conn = new sql.Connection(config);
		conn.connect().then(function () {
			var req = new sql.Request(conn);
			req.query(connectionString).then(function (recordset) {
				try {
					var outName = recordset[0].name
					var outServer = recordset[0].server
					var outClass = recordset[0].class_name
					var outRace = recordset[0].race_name
					var outLevel = recordset[0].level
					var outAch = recordset[0].achievement_points
					var outRank = recordset[0].rank_name
					if (outServer != undefined) {
						msg.channel.sendMessage("Information for member: **" + outName + "**\n" +
						"Server: " + outServer + "\n" +
						"Guild Rank: " + outRank + "\n" +
						"Class: " + outClass + "\n" + 
						"Race: " + outRace + "\n" +
						"Level: " + outLevel + "\n" + 
						"Achievement Points: " + outAch);
					}
					else {
						Console.log("Could not locate member: " + inName);
						msg.channel.sendMessage("Could not locate member " + inName + "in stored records.");
					}
				}
				catch (err) {
					
				}
				conn.close();
			});
		}).catch(NotFoundError, function (e) {
			reply(e);
		});
	}


    if (msg.content.startsWith("!news")) {
        var cmd = msg.content.split(" ");
        //cmd[0] = !news
        //cmd[1] = on/off
        console.log("News " + cmd[1]);
        if (cmd[1] == "on") {
            newsFeedOn = true;
            msg.channel.sendMessage("Turning Blessed Ignorance Guild News Feed On.");
            blizzard.wow.guild(['news'], { origin: 'us', realm: 'Zul\'Jin', name: 'Blessed Ignorance' })
			.then(response => {
			    currentTimestamp = response.data.news[5].timestamp;
			    console.log("Initial Timestamp set to: " + currentTimestamp);
			})
            myVar = setInterval(getNews, 10000); //30 second refresh
          //  msg.channel.sendMessage("News Feed is On. Type !news off to disable. Currently reporting Legendary Drops and some achievements.");
        } //end on
        else if (cmd[1] == "off") {
            newsFeedOn = false;
            msg.channel.sendMessage("Turning Blessed Ignorance Guild News Feed Off. Type '!news on' to re-enable.");
            clearInterval(getNews); //turn off the setInterval for getNews function
        } //end off
        else {
            msg.channel.sendMessage("Command not recognized, should be either '!news on' or '!news off'.");
        } //end else

    } // end !news

    if (msg.content.startsWith("!legendary")) {
        var legName = msg.content;

        legName = legName.replace("!legendary ", "");
		legName = legName.replace("'", "''");
        console.log("Searching for " + legName);
        var connectionString = "select top 1 item_name, item_effect from web.dbo.tbl_legendary where item_name like '%" + legName + "%'";
        var conn = new sql.Connection(config);
        conn.connect().then(function () {
            var req = new sql.Request(conn);
            req.query(connectionString).then(function (recordset) {

                try {
                    var itemName = recordset[0].item_name;
                    var itemEffect = recordset[0].item_effect;



                    if (itemName != undefined) {
                        msg.channel.sendMessage("Effect for Legendary: " + itemName + "\n" + itemEffect);

                    }
                    else {
                        console.log("Could not locate legendary in database");
						msg.channel.sendMessage("Could not locate legendary in database.");
                    }
                }
                catch (err) {

                }
                conn.close();
            });
        }).catch(NotFoundError, function (e) {
            reply(e);
        });

    }

	
	if (msg.content.startsWith("!token")) {
		request({
			url: tokenurl,
			json: true
			}, function (error, response, body) {
				
				if (!error && response.statusCode === 200) {
					msg.channel.sendMessage("NA Token: " + body.update.NA.formatted.buy + "\n" + 
					"Europe Token: " + body.update.EU.formatted.buy + "\n" +
					"China Token: " + body.update.CN.formatted.buy + "\n" +
					"Taiwan Token: " + body.update.TW.formatted.buy + "\n" +
					"Korea Token: " + body.update.KR.formatted.buy);
					//console.log(body.update.NA.formatted.buy);
				}
			}
		)			
	}
	
    if (msg.content.startsWith("!rip")) { //if user starts message with '!rip' then begin api calls

        var cmd = msg.content.split(" ");
        //cmd[0] = '!rip'
        //cmd[1] = name
        //cmd[2] = realm
        console.log(cmd[1]);
        console.log(cmd[2]);
        if (cmd[3] != null) //a realm with a space was entered, add the extra word to cmd[2]
        {
            cmd[2] = cmd[2] + " " + cmd[3];
        }

        else if (cmd[2] == null) //missing parameter, throw error message to chat
        {
            msg.channel.sendMessage("Looks like you're missing a parameter.");
        }

        else {
            blizzard.wow.character(['statistics'], { origin: 'us', realm: cmd[2], name: cmd[1] })
			.then(response => {
			    var qty = response.data.statistics.subCategories[3].statistics[0].quantity;
			    console.log(qty);
			    if (qty <= 0) {
			        var ran = 0;
			    }
			    else {
			        var ran = Math.floor(Math.random() * 9) + 1;
			    }
			    msg.channel.sendMessage(cmd[1] + " has died " + qty + " times. " + "\n" + savage[ran]);
			    console.log("Done");
			})

			.catch(function (e) {

			    var errorMessage = e.response.data.reason;
			    console.log(errorMessage);

			    msg.channel.sendMessage(errorMessage);
			}) //end catch
        }; //end if
    }; //end !rip
})  //end bot.on


function isLegendary(id, playerName) {
    //console.log("Entering isLegendary function");

    var connectionString = "select item_name, item_effect from web.dbo.tbl_legendary where id = " + id;
    var conn = new sql.Connection(config);
    conn.connect().then(function () {
        var req = new sql.Request(conn);
        req.query(connectionString).then(function (recordset) {

            try {
                var itemName = recordset[0].item_name;
                var itemEffect = recordset[0].item_effect;
				console.log(id);
				console.log(itemName);

                if (itemName != undefined) {
                    console.log("Legendary drop!");
                    bot.channels.get(channelId).sendMessage("Woah! " + itemName + " dropped for " + playerName + "!\n" + "Effect: " + itemEffect);
                }
                else {
                    console.log("undefined item");
                }
            }
            catch (err) {

            }
            conn.close();
        });
    }).catch(NotFoundError, function (e) {
        reply(e);
    });
} //end isLegendary

function NotFoundError(e) {
    return e.statusCode === 404;
}

function getNews() {
    //console.log("Checking News Feed");
    //console.log("Current TimeStamp is: " + currentTimestamp);
    //check for new news

    blizzard.wow.guild(['news'], { origin: 'us', realm: 'Zul\'Jin', name: 'Blessed Ignorance' })
	.then(response => {
	    var newsLength = response.data.news.length - 1;
	    for (i = newsLength; i >= 0; i--) {
	        if (response.data.news[i].timestamp > currentTimestamp) { 
				currentTimestamp = response.data.news[i].timestamp;
				console.log("Timestamp Updated: " + currentTimestamp);
				if (response.data.news[i].type =="itemLoot") {    //display legendary item 
					var lootId = response.data.news[i].itemId; //get loot ID number to determine if item is legendary
					var playerName = response.data.news[i].character;
					isLegendary(lootId, playerName); //legendary lookup
				} //end if
		
				if (response.data.news[i].type == "playerAchievement") {
					var playerName = response.data.news[i].character;
					var achName = response.data.news[i].achievement.title;
					//var achDesc = response.data.news[i].achievement.description;
					/*
					if (response.data.news[i].achievement.id == 10671) {
						bot.channels.get(channelId).sendMessage(playerName + " has reached level 110!");
					}
					*/ //deprecated, use switch statement
					switch (response.data.news[i].achievement.id) {
						//general achievements
						case 10671:
							bot.channels.get(channelId).sendMessage(playerName + " has reached level 110!");
							break;
						
						//mount achievments
						case 4626:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Mimiron's Head!");
							break;
						case 885:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Ashes of Al'ar!");
							break;
						case 729:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Deathcharger's Reins!");
							break;
						case 882:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Fiery Warhorse's Reins!");
							break;
						case 2081:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Grand Black War Mammoth!");
							break;
						case 4625:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Invincible's Reins!");
							break;
						case 883:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Reins of the Raven Lord!");
							break;
						case 884:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Swift White Hawkstrider!");
							break;
						case 424:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained a Red Qiraji Resonating Crystal!");
							break;
						//legendary achievements
						case 5839:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Dragonwrath, Tarecgosa's Rest!");
							break;
						case 6181:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Fangs of the Father!");
							break;
						case 4623:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Shadowmourne!");
							break;
						case 429:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Sulfuras, Hand of Ragnaros!");
							break;
						case 725:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Thori'dal, the Stars' Fury!");
							break;
						case 428:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Thunderfury, Blessed Blade of the Windseeker!");
							break;
						case 3142:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained Val'anyr, Hammer of Ancient Kings!");
							break;
						case 426:
							bot.channels.get(channelId).sendMessage(playerName + " has obtained the Warglaives of Azzinoth!");
							break;
						default: 
							console.log("Not a tracked achievement");
					}
				}
				
				if (response.data.news[i].type == "guildAchievement") {
					console.log("Guild Achievement Spotted!");
					var achName = response.data.news[i].achievement.title;
					bot.channels.get(channelId).sendMessage("Guild has earned the achievement: " + achName);
				}

			} //end for
		} //end then
	})
}//    end setInterval

bot.on('ready', () => {
    console.log('I am ready!');
	



})

bot.login("MjUzOTI4MzYyOTE1OTIxOTIw.CyHrQQ.92RQzW5pbexLvkXVfMmjgSZXADI");