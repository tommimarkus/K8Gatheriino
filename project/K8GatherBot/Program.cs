﻿using Discore;
using Discore.Http;
using Discore.WebSocket;
using K8GatherBot;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace K8GatherBot
{


    public class Program
    {

        public static class ProgHelpers
        {
            //things needed for obvious reasons
            public static IConfigurationRoot Configuration { get; set; }
            public static PersistedData persistedData { get; set; }

            public static List<string> queue = new List<string>(); //names
            public static List<string> queueids = new List<string>(); //discord ids
            public static List<string> readycheckids = new List<string>(); //readycheck
            public static List<string> readycheck = new List<string>(); //readychecknames
            public static List<string> cptrandomids = new List<string>(); //captainrandoming
            public static List<string> cptrandom = new List<string>(); //captainrandomingnames

            public static string captain1 = "";
            public static string captain1id = "";
            public static List<string> team1 = new List<string>();
            public static List<string> team1ids = new List<string>();

            public static string captain2 = "";
            public static string captain2id = "";
            public static List<string> team2 = new List<string>();
            public static List<string> team2ids = new List<string>();

            //Workaround to keep initial list numbering for the whole draft, show names from another list.
            public static List<string> draftchatnames = new List<string>();
            public static List<string> draftchatids = new List<string>();

            public static string pickturn = ""; //initial "", team1 is cap1id, team2 is cap2id

            //Timer lenght
            public static int _counter = 0; //initial value

            public static int counterlimit = 0; //maxvalue
                                                //queuesize and channel to listen
            public static int qcount = 0; //minimum count is 4
            public static string userChannel = "";
            public static string bottoken = "";
            public static string txtversion = "";
            public static string language = ""; //en-fi

            public static Dictionary<string, Dictionary<string, string>> locales = new Dictionary<string, Dictionary<string, string>>();

            public static Dictionary<string, string> locale;
        }

        public static void Main(string[] args)
        {
            //Get settings and GO
            Console.WriteLine("Reading settings from appsettings.json");

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            ProgHelpers.Configuration = builder.Build();
            ProgHelpers.persistedData = new PersistedData();

            Console.WriteLine("START SETTINGS-----------------------------");
            ProgHelpers.qcount = Convert.ToInt32(ProgHelpers.Configuration["Settings:Queuesize"]);
            Console.WriteLine("qcount:" + Convert.ToInt32(ProgHelpers.Configuration["Settings:Queuesize"]));
            ProgHelpers.counterlimit = Convert.ToInt32(ProgHelpers.Configuration["Settings:Readytimer"]);
            Console.WriteLine("counterlimit:" + Convert.ToInt32(ProgHelpers.Configuration["Settings:Readytimer"]));
            ProgHelpers.language = ProgHelpers.Configuration["Settings:Language"];
            Console.WriteLine("language:" + ProgHelpers.Configuration["Settings:Language"]);
            ProgHelpers.userChannel = ProgHelpers.Configuration["Settings:AllowedChannel"];
            Console.WriteLine("usechannel:" + ProgHelpers.Configuration["Settings:AllowedChannel"]);
            ProgHelpers.bottoken = ProgHelpers.Configuration["Settings:BotToken"];
            Console.WriteLine("token:" + ProgHelpers.Configuration["Settings:BotToken"]);
            ProgHelpers.txtversion = ProgHelpers.Configuration["Settings:Version"];
            Console.WriteLine("txtversion:" + ProgHelpers.Configuration["Settings:Version"]);
            Console.WriteLine("END SETTINGS-----------------------------");

            initLocalizations();
            ProgHelpers.locale = ProgHelpers.locales[ProgHelpers.language];

            Program program = new Program();
            program.Run().Wait();
        }

        public static void initLocalizations()
        {
            Dictionary<string, string> fi = new Dictionary<string, string>();
            Dictionary<string, string> en = new Dictionary<string, string>();
            ProgHelpers.locales.Add("fi", fi);
            ProgHelpers.locales.Add("en", en);

            fi.Add("pickPhase.alreadyInProcess", "Odota kunnes edellinen jono on käsitelty.");
            fi.Add("queuePhase.added", "Lisätty!");
            fi.Add("readyPhase.started", "Jono on nyt täynnä, merkitse itsesi valmiiksi käyttäen ***!ready*** komentoa. \n Aikaa 60 sekuntia!");
            fi.Add("queuePhase.alreadyInQueue", "Olet jo jonossa!");
            fi.Add("pickPhase.cannotRemove", "Liian myöhäistä peruuttaa, odota jonon käsittelyn valmistumista.");
            fi.Add("queuePhase.removed", "Poistettu!");
            fi.Add("queuePhase.notInQueue", "Et ole juuri nyt jonossa");
            fi.Add("queuePhase.notReadyYet", "Jono ei ole vielä valmis!");
            fi.Add("readyPhase.ready", "Valmiina!");
            fi.Add("pickPhase.started", "Readycheck valmis, aloitetaan poimintavaihe! Ensimmäinen poiminta: Team 1 \n Team1:n kapteeni:");
            fi.Add("pickPhase.team2Captain", "Team 2:n kapteeni:");
            fi.Add("pickPhase.instructions", "Poimi pelaajia käyttäen ***!pick NUMERO***");
            //fi.Add("txt13",  "Näyttäisi siltä ettet ole jonossa.");
            fi.Add("pickPhase.team2Turn", "Pelaaja lisätty Team 1:n! \n Team 2:n vuoro poimia!");
            fi.Add("pickPhase.team1Turn", "Pelaaja lisätty Team 2:n! \n Team 1:n vuoro poimia!");
            fi.Add("pickPhase.unpicked", "***Poimimatta:***");
            fi.Add("pickPhase.alreadyPicked", "Pelaaja on jo joukkueessa!");
            fi.Add("pickPhase.unknownIndex", "Numerolla ei löytynyt pelaajaa!");
            fi.Add("pickPhase.notYourTurn", "Ei ole vuorosi poimia!");
            fi.Add("pickPhase.notCaptain", "Vain kapteenit voivat poimia pelaajia!");
            fi.Add("queuePhase.emptyQueue", "Jono on tyhjä! Käytä ***!add*** aloittaaksesi jonon!");
            fi.Add("admin.resetSuccessful", "Kaikki listat tyhjennetty onnistuneesti!");
            fi.Add("status.pickedTeams", "Valitut joukkueet");
            fi.Add("status.queueStatus", "jonon tilanne");
            fi.Add("info.purposeAnswer", "Saada pelaajia keräytymään pelien äärelle!");
            fi.Add("info.funFactAnswer", "Gathut aiheuttavat paljon meemejä :thinking:");
            fi.Add("info.developer", "Kehittäjä");
            fi.Add("info.purpose", "Tarkoitus");
            fi.Add("info.funFact", "Tiesitkö");
            fi.Add("info.commands", "Komennot");
            fi.Add("status.queuePlayers", "Pelaajat");
            fi.Add("status.notReady", "EI VALMIINA");
            //fi.Add("txt32",  "Poistettu koska käyttäjä on ***OFFLINE***.");
            //fi.Add("txt33",  "Aloitetaan readycheck. Teillä on 60 sekuntia aikaa käyttää ***!ready*** komentoa.");
            fi.Add("readyPhase.timeout", "Kaikki pelaajat eivät olleet valmiita readycheckin aikana. Palataan jonoon valmiina olleiden pelaajien kanssa.");
            fi.Add("readyPhase.alreadyMarkedReady", "Olet jo merkinnyt itsesi valmiiksi!");
            fi.Add("readyPhase.cannotAdd", "Odota poimintavaiheen päättymistä!");
            fi.Add("fatKid.header", "Viimeiseksi valittu");
            fi.Add("fatKid.top10", "Top10 viimeisenä valitut");
            fi.Add("fatKid.statusSingle", "{0} on valittu viimeisenä {1} kertaa ({2}/{3})");
            fi.Add("highScores.header", "Gathuja pelattu");
            fi.Add("highScores.top10", "Top10 gathuLEGENDAT");
            fi.Add("highScores.statusSingle", "{0} on pelannut {1} gathua ({2}/{3})");
            fi.Add("thinKid.header", "1. varaus");
            fi.Add("thinKid.top10", "Top10 ensimmäisenä valitut");
            fi.Add("thinKid.statusSingle", "{0} on valittu ensimmäisenä {1} kertaa ({2}/{3})");
            fi.Add("captain.header", "Kapteeni");
            fi.Add("captain.top10", "Top10 kapteenit");
            fi.Add("captain.statusSingle", "{0} on valittu kapteeniksi {1} kertaa ({2}/{3})");
            fi.Add("player.stats", "pelaajan tiedot");
            fi.Add("noCap.text", "Sinua ei arvota enää kapteeniksi!");
            fi.Add("yesCap.text", "Sinut voidaan nyt arpoa kapteeniksi!");

            en.Add("pickPhase.alreadyInProcess", "Please wait until the previous queue is handled.");
            en.Add("queuePhase.added", "Added!");
            en.Add("readyPhase.started", "Queue is now full, proceed to mark yourself ready with ***!ready*** \n You have 60 seconds!");
            en.Add("queuePhase.alreadyInQueue", "You're already in the queue!");
            en.Add("pickPhase.cannotRemove", "Too late to back down now! Wait until the queue is handled.");
            en.Add("queuePhase.removed", "Removed!");
            en.Add("queuePhase.notInQueue", "You are not in the queue right now.");
            en.Add("queuePhase.notReadyYet", "Queue is not finished yet!");
            en.Add("readyPhase.ready", "Ready!");
            en.Add("pickPhase.started", "Readycheck complete, starting picking phase! First picking turn: Team 1 \n Team 1 Captain:");
            en.Add("pickPhase.team2Captain", "Team 2 Captain:");
            en.Add("pickPhase.instructions", "Pick players using ***!pick NUMBER***");
            //en.Add("txt13",  "It seems you're not in the queue.");
            en.Add("pickPhase.team2Turn", "Player added to Team 1!\n Team 2 Turn to pick!");
            en.Add("pickPhase.team1Turn", "Player added to Team 2! \n Team 1 Turn to pick!");
            en.Add("pickPhase.unpicked", "***Remaining players:***");
            en.Add("pickPhase.alreadyPicked", "That player is already in a team!");
            en.Add("pickPhase.unknownIndex", "Couldn't place a player with that index");
            en.Add("pickPhase.notYourTurn", "Not your turn to pick right now!");
            en.Add("pickPhase.notCaptain", "You are not the captain of either teams. Picking is restricted to captains.");
            en.Add("queuePhase.emptyQueue", "Nobody in the queue! use ***!add***  to start queue!");
            en.Add("admin.resetSuccessful", "All lists emptied successfully.");
            en.Add("status.pickedTeams", "Selected teams");
            en.Add("status.queueStatus", "current queue");
            en.Add("info.purposeAnswer", "Get people to gather and play together");
            en.Add("info.funFactAnswer", "Only a droplet of coffee was used to develop this bot. :thinking:");
            en.Add("info.developer", "Developer");
            en.Add("info.purpose", "Purpose");
            en.Add("info.funFact", "Fun fact");
            en.Add("info.commands", "Commands");
            en.Add("status.queuePlayers", "Players");
            en.Add("status.notReady", "NOT READY YET:");
            //en.Add("txt32", "Removed because status is ***Offline***");
            //en.Add("txt33", "Starting Readycheck timer, you have 60 seconds to ***!ready*** yourself.");
            en.Add("readyPhase.timeout", "Not all players were ready during the readycheck. Returning to queue with players that were ready.");
            en.Add("readyPhase.cannotAdd", "Wait until the picking phase is over.");
            en.Add("readyPhase.alreadyMarkedReady", "You have already readied!");
            en.Add("fatKid.header", "Fat Kid");
            en.Add("fatKid.top10", "Top 10 Fat Kids");
            en.Add("fatKid.statusSingle", "{0} has been the fat kid {1} times ({2}/{3})");
            en.Add("highScores.header", "Games played");
            en.Add("highScores.top10", "Top 10 Gathering LEGENDS");
            en.Add("highScores.statusSingle", "{0} has played {1} games ({2}/{3})");
            en.Add("thinKid.header", "Thin kid");
            en.Add("thinKid.top10", "Top10 Thin Kids");
            en.Add("thinKid.statusSingle", "{0} has been the thin kid {1} times ({2}/{3}");
            en.Add("captain.header", "Captain");
            en.Add("captain.top10", "Top10 Captains");
            en.Add("captain.statusSingle", "{0} has been selected captain {1} times ({2}/{3}");
            en.Add("player.stats", "Player statistics");
            en.Add("noCap.text", "You will no longer be randomed as the captain!");
            en.Add("yesCap.text", "You may now be randomed to be the captain!");
        }

        public async Task Run()
        {

            // Create authenticator using a bot user token.
            DiscordBotUserToken token = new DiscordBotUserToken(ProgHelpers.bottoken); //token
                                                                                       // Create a WebSocket application.
            DiscordWebSocketApplication app = new DiscordWebSocketApplication(token);
            // Create and start a single shard.
            Shard shard = app.ShardManager.CreateSingleShard();
            await shard.StartAsync(CancellationToken.None);
            // Subscribe to the message creation event.
            shard.Gateway.OnMessageCreated += Gateway_OnMessageCreated;

            Console.WriteLine(DateTime.Now + $" -- kitsun8's GatherBot Started \n -------------------------------------------");
            // Wait for the shard to end before closing the program.
            while (shard.IsRunning)
                await Task.Delay(1000);
        }

        public async Task RunNotRdyannounce()
        {
            DiscordBotUserToken token = new DiscordBotUserToken(ProgHelpers.bottoken); //token
            DiscordWebSocketApplication app = new DiscordWebSocketApplication(token);
            Shard shard = app.ShardManager.CreateSingleShard();
            await shard.StartAsync(CancellationToken.None);
            Console.WriteLine(DateTime.Now + $" -- Started new shard for announce -------------------------------------------");
            Snowflake xx = new Snowflake();
            ulong xxid = (ulong)Convert.ToInt64(ProgHelpers.userChannel);
            xx.Id = xxid;
            ITextChannel textChannel = (ITextChannel)shard.Application.HttpApi.Channels.CreateMessage(xx, ProgHelpers.locale["readyPhase.timeout"]);
            await shard.StopAsync();
        }

        //------------------------------------------------------------------------Timer for ready check
        public static Timer _tm = null;
        public static AutoResetEvent _autoEvent = null;

        public static void StartTimer()
        {
            //timer init

            _autoEvent = new AutoResetEvent(false);
            _tm = new Timer(Checkrdys, _autoEvent, 1000, 1000);

            Console.WriteLine("RDYCHECK ACTIVATED --- " + DateTime.Now.ToString());
            //Console.Read();
        }

        public static void Checkrdys(Object stateInfo)
        {
            if (ProgHelpers._counter < ProgHelpers.counterlimit)
            {
                ProgHelpers._counter++;
                return;
            }
            //Timer is up, check who have not readied.
            List<string> notinlist = ProgHelpers.queueids.Except(ProgHelpers.readycheckids).ToList();
            if (notinlist.Count > 0)
            {

                foreach (string item in notinlist)
                {
                    var fndr1 = ProgHelpers.queueids.IndexOf(item); //Get index because discord name can change, id can not

                    ProgHelpers.queue.RemoveAt(fndr1);
                    ProgHelpers.queueids.Remove(item);
                    Console.WriteLine("RDYCHECK-REMOVED --- " + item);
                }
                ProgHelpers.readycheckids.Clear();
                ProgHelpers.readycheck.Clear();

                Program rdyprog = new Program();
                rdyprog.RunNotRdyannounce();

            }

            //dispose of the currenttimer
            _tm.Dispose();
            ProgHelpers._counter = 0;

            Console.WriteLine("RDYCHECK EXPIRED --- " + DateTime.Now.ToString());
        }

        //------------------------------------------------------------------------Gateway messages parsing
        private async void Gateway_OnMessageCreated(object sender, MessageEventArgs e)
        {
            Shard shard = e.Shard;
            DiscordMessage message = e.Message;

            //Open helper functions
            KGHelper kgh = new KGHelper();

            if (message.Author == shard.User)
                // Ignore messages created by our bot.
                return;

            if (message.ChannelId.Id.ToString() != ProgHelpers.userChannel)//Prevent DM abuse, only react to messages sent on a set channel.
            {
                return;
            }

            string msgBody = message.Content.ToLower().Split(' ')[0];

            switch (msgBody)
            {
                case "!abb": //haHAA!
                case "! add": //haHAA!
                case "!dab": //haHAA!
                case "!abd": //haHAA!
                case "!asd": //haHAA!
                case "!das": //haHAA!
                case "!a": //haHAA!
                case "!add":
                    await CmdAdd(shard, message);
                    break;
                case "!remove":
                case "!rm":
                    await CmdRemove(shard, message);
                    break;
                case "!ready":
                case "!r":
                    await CmdReady(shard, message);
                    break;
                case "!pick":
                case "!p":
                    await CmdPick(shard, message);
                    break;
                case "!fakeriino": 
                    //CmdFakeriino(shard, message);
                    break;
                case "!pstats":
                    await CmdPlayerStats(shard, message);
                    break;
                case "!fatkid":
                    await CmdFatKid(shard, message);
                    break;
                case "!f10":
                case "!fat10":
                    await CmdFatTopTen(shard, message);
                    break;
                case "!highscore":
                case "!hs":
                    await CmdHighScore(shard, message);
                    break;
                case "!topten":
                case "!top10":
                    await CmdTopTen(shard, message);
                    break;  
                case "!thinkid":
                    await CmdThinKid(shard, message);
                    break;
                case "!tk10":
                    await CmdThinTopTen(shard, message);
                    break;
                case "!captain":
                    await CmdCaptain(shard, message);
                    break;
                case "!c10":
                    await CmdCaptainTopTen(shard, message);
                    break;
                case "!gstatus":
                case "!gs":
                    await CmdGStatus(shard, message);
                    break;
                case "!resetbot":
                    await CmdResetBot(shard, message);
                    break;
                case "!gatherinfo":
                case "!gi":
                    await CmdGatherInfo(shard, message);
                    break;
                case "!yescap":
                    await CmdYesCap(shard, message);
                    break;
                case "!nocap":
                    await CmdNoCap(shard, message);
                    break;
            }

        }

        private async Task CmdPlayerStats(Shard shard, DiscordMessage message)
        { 
            try 
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                Tuple<string, string> idUsername = ParseIdAndUsername(message);
                Console.WriteLine("user for checking " + idUsername);
                string highScoreStats = ProgHelpers.persistedData.GetHighScoreInfo(idUsername, ProgHelpers.locale["highScores.statusSingle"]);
                string fatkidStats = ProgHelpers.persistedData.GetFatKidInfo(idUsername, ProgHelpers.locale["fatKid.statusSingle"]);
                string captainStats = ProgHelpers.persistedData.GetCaptainInfo(idUsername, ProgHelpers.locale["captain.statusSingle"]);
                string thinkidStats = ProgHelpers.persistedData.GetThinKidInfo(idUsername, ProgHelpers.locale["thinKid.statusSingle"]);

                textChannel.CreateMessage(new DiscordMessageDetails()
                    .SetEmbed(new DiscordEmbedBuilder()
                              .SetTitle($"kitsun8's GatherBot, " + ProgHelpers.locale["player.stats"] + ": " + idUsername.Item2)
                              .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                              .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                              .AddField(ProgHelpers.locale["highScores.header"], highScoreStats, true)
                              .AddField(ProgHelpers.locale["captain.header"], captainStats, true)
                              .AddField(ProgHelpers.locale["thinKid.header"], thinkidStats, true)
                              .AddField(ProgHelpers.locale["fatKid.header"], fatkidStats, true)));
                Console.WriteLine("oi");

            } catch (Exception e)
            {
                Console.WriteLine($"!gatherinfo - EX -" + message.Author.Username + "-" + message.Author.Id + " --- " + DateTime.Now);
                Console.WriteLine(e.ToString());
            }
        }

        private async Task CmdGatherInfo(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                // Reply to the user who posted "!info".
                textChannel.CreateMessage(new DiscordMessageDetails()
                    .SetEmbed(new DiscordEmbedBuilder()
                    .SetTitle($"kitsun8's GatherBot")
                    .SetFooter("Discore (.NET Core), C# , " + ProgHelpers.txtversion)
                    .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                    .AddField(ProgHelpers.locale["info.developer"] + " ", "kitsun8 & pirate_patch", false)
                    .AddField(ProgHelpers.locale["info.purpose"] + " ", ProgHelpers.locale["info.purposeAnswer"], false)
                    .AddField(ProgHelpers.locale["info.funFact"] + " ", ProgHelpers.locale["info.funFactAnswer"], false)
                    .AddField(ProgHelpers.locale["info.commands"] + " ", "!add, !remove/rm, !ready/r, !pick/p, !gatherinfo/gi, !gstatus/gs, !resetbot, !f10/fat10, !fatkid, !top10/topten, !hs/highscore, !tk10, !thinkid, !c10, !captain, !nocap, !yescap", false)
                ));

                Console.WriteLine($"!gatherinfo - " + message.Author.Username + "-" + message.Author.Id + " --- " + DateTime.Now);
            }
            catch (Exception)
            {
                Console.WriteLine($"!gatherinfo - EX -" + message.Author.Username + "-" + message.Author.Id + " --- " + DateTime.Now);
            }
        }

        private async Task CmdResetBot(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                ResetQueue();
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["admin.resetSuccessful"]);
                Console.WriteLine("!resetbot" + " --- " + DateTime.Now);
            }
            catch (Exception)
            {
                Console.WriteLine("EX-!resetbot" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdGStatus(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);

                if (ProgHelpers.queue.Count != 0)
                {
                    if (ProgHelpers.queue.Count == ProgHelpers.qcount)
                    {

                        //compare readycheck list to queue, print out those who are not ready
                        List<string> notinlist = ProgHelpers.queue.Except(ProgHelpers.readycheck).ToList();

                        //full queue, ready phase
                        textChannel.CreateMessage(new DiscordMessageDetails()
                        .SetEmbed(new DiscordEmbedBuilder()
                        .SetTitle($"kitsun8's GatherBot, readycheck  " + "(" + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + ")")
                        .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                        .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                        .AddField(ProgHelpers.locale["status.queuePlayers"] + " ", string.Join("\n", notinlist.Cast<string>().ToArray()), false)
                        ));

                    }
                    else
                    {
                        if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count > 0)
                        {
                            //picking phase
                            textChannel.CreateMessage(new DiscordMessageDetails()
                            .SetEmbed(new DiscordEmbedBuilder()
                            .SetTitle($"kitsun8's GatherBot, " + ProgHelpers.locale["status.pickedTeams"])
                            .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                            .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                            .AddField("Team1: ", string.Join("\n", ProgHelpers.team1.Cast<string>().ToArray()), true)
                            .AddField("Team2: ", string.Join("\n", ProgHelpers.team2.Cast<string>().ToArray()), true)
                            ));
                        }
                        else
                        {
                            //queue phase
                            textChannel.CreateMessage(new DiscordMessageDetails()
                            .SetEmbed(new DiscordEmbedBuilder()
                            .SetTitle($"kitsun8's GatherBot, " + ProgHelpers.locale["status.queueStatus"] + " " + "(" + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + ")")
                            .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                            .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                            .AddField(ProgHelpers.locale["status.queueStatus"] + " ", string.Join("\n", ProgHelpers.queue.Cast<string>().ToArray()), false)
                            ));
                        }

                    }

                    Console.WriteLine("!status");
                }
                else
                {
                    textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.emptyQueue"]);
                    Console.WriteLine("!status" + " --- " + DateTime.Now);
                }

            }
            catch (Exception)
            {
                Console.WriteLine("EX-!status" + " --- " + DateTime.Now);
            }
        }

        private async Task sendTop10Message(ITextChannel textChannel, string textKey, string top10List) {
            textChannel.CreateMessage(new DiscordMessageDetails()
                            .SetEmbed(new DiscordEmbedBuilder()
                                      .SetTitle($"kitsun8's Gatheriino")
                                      .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                                      .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                      .AddField(ProgHelpers.locale[textKey], top10List)));
        }

        private async Task CmdTopTen(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                string highScoreTop10 = ProgHelpers.persistedData.GetHighScoreTop10();
                await sendTop10Message(textChannel, "highScores.top10", highScoreTop10);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX-!top10 --- " + DateTime.Now);
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task CmdFatTopTen(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                string fatKidTop10 = ProgHelpers.persistedData.GetFatKidTop10();
                await sendTop10Message(textChannel, "fatKid.top10", fatKidTop10);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX-!f10 --- " + DateTime.Now);
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task CmdCaptainTopTen(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                string captainTop10 = ProgHelpers.persistedData.GetCaptainTop10();
                await sendTop10Message(textChannel, "captain.top10", captainTop10);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX-!c10 --- " + DateTime.Now);
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task CmdCaptain(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                Tuple<string, string> idUsername = ParseIdAndUsername(message);
                Console.WriteLine("captain name split resulted in " + idUsername);
                string captainInfo = ProgHelpers.persistedData.GetCaptainInfo(idUsername, ProgHelpers.locale["captain.statusSingle"]);
                textChannel.CreateMessage($"<@{message.Author.Id}> " + captainInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine("EX-!captain" + " --- " + DateTime.Now);
                Console.WriteLine(e.ToString());
            }
        }

        private async Task CmdHighScore(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                Tuple<string, string> idUsername = ParseIdAndUsername(message);
                Console.WriteLine("fatkid name split resulted in " + idUsername);
                string hsInfo = ProgHelpers.persistedData.GetHighScoreInfo(idUsername, ProgHelpers.locale["highScores.statusSingle"]);
                textChannel.CreateMessage($"<@{message.Author.Id}> " + hsInfo);
            }
            catch
            {
                Console.WriteLine("EX-!hs" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdFatKid(Shard shard, DiscordMessage message) 
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                Tuple<string, string> idUsername = ParseIdAndUsername(message);
                Console.WriteLine("fatkid name split resulted in " + idUsername);
                string fatKidInfo = ProgHelpers.persistedData.GetFatKidInfo(idUsername, ProgHelpers.locale["fatKid.statusSingle"]);
                textChannel.CreateMessage($"<@{message.Author.Id}> " + fatKidInfo);
            }
            catch
            {
                Console.WriteLine("EX-!fatkid" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdThinTopTen(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                string thinKidTop10 = ProgHelpers.persistedData.GetThinKidTop10();
                textChannel.CreateMessage(new DiscordMessageDetails()
                            .SetEmbed(new DiscordEmbedBuilder()
                                      .SetTitle($"kitsun8's Gatheriino")
                                      .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                                      .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                      .AddField(ProgHelpers.locale["thinKid.top10"], thinKidTop10)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX-!tk10 --- " + DateTime.Now);
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task CmdThinKid(Shard shard, DiscordMessage message)
        {
            try
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                string msg = message.Content;
                Tuple<string, string> idUsername = ParseIdAndUsername(message);
                Console.WriteLine("thinkid name split resulted in " + idUsername);
                string thinKidInfo = ProgHelpers.persistedData.GetFatKidInfo(idUsername, ProgHelpers.locale["thinKid.statusSingle"]);

                textChannel.CreateMessage($"<@{message.Author.Id}> " + thinKidInfo);
            }
            catch
            {
                Console.WriteLine("EX-!thinkid" + " --- " + DateTime.Now);
            }
        }

        private void CmdFakeriino(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            HandleAdd(message, textChannel, "240432007908294656", "pirate_patch");
            Thread.Sleep(100);
            HandleAdd(message, textChannel, "111", "kerpo");
            Thread.Sleep(100);
            HandleAdd(message, textChannel, "222", "jonne");
            Thread.Sleep(100);
            HandleAdd(message, textChannel, "333", "spede");
            Thread.Sleep(100);
            HandleReady(message, textChannel, "240432007908294656", "pirate_patch");
            Thread.Sleep(100);
            HandleReady(message, textChannel, "111", "kerpo");
            Thread.Sleep(100);
            HandleReady(message, textChannel, "222", "jonne");
            Thread.Sleep(100);
            HandleReady(message, textChannel, "333", "spede");
            Thread.Sleep(100);
        }

        private bool PickTeamMember(ITextChannel textChannel, DiscordUser author, string msg, List<string> teamIds, List<string> team, string nextCaptain)
        {
            string[] msgsp = msg.Split(null);
            int selectedIndex = 0;
            var selectedPlayerId = "";
            var selectedPlayerName = "";
            if(!Int32.TryParse(msgsp[1].Trim(), out selectedIndex))
            {
                // ignore since it was garbage   
            }
            if (ProgHelpers.queueids.ElementAtOrDefault(selectedIndex) == null)
            {
                textChannel.CreateMessage($"<@{author.Id}> " + ProgHelpers.locale["pickPhase.unknownIndex"]);
                return false;
            }

            selectedPlayerId = ProgHelpers.queueids.ElementAtOrDefault(selectedIndex);
            selectedPlayerName = ProgHelpers.queue.ElementAtOrDefault(selectedIndex);

            if (ProgHelpers.team1ids.IndexOf(selectedPlayerId) > -1 || ProgHelpers.team2ids.IndexOf(selectedPlayerId) > -1)
            {
                textChannel.CreateMessage($"<@{author.Id}> " + ProgHelpers.locale["pickPhase.alreadyPicked"]);
                return false;
            }

            teamIds.Add(selectedPlayerId);
            team.Add(selectedPlayerName);

            int finderremover = ProgHelpers.draftchatids.IndexOf(selectedPlayerId);
            ProgHelpers.draftchatnames.RemoveAt(finderremover);
            ProgHelpers.draftchatids.RemoveAt(finderremover);

            Console.WriteLine(ProgHelpers.draftchatnames.Cast<string>().ToArray());

            // add thin kid (the first pick)
            if (ProgHelpers.team1.Count + ProgHelpers.team2.Count == 3)
            {
                ProgHelpers.persistedData.AddThinKid(selectedPlayerId, selectedPlayerName);
            }

            ProgHelpers.pickturn = nextCaptain;

            return true;
        }

        private void PickFatKid(List<string> teamids, List<string> teamNames) 
        {
            var remainingPlayer = ProgHelpers.draftchatids.FirstOrDefault();
            var remainingPlayerIndex = ProgHelpers.queueids.IndexOf(remainingPlayer);
            var remainingPlayername = ProgHelpers.queue.ElementAtOrDefault(remainingPlayerIndex);

            //Put remaining player in picking team (pickturn has already gotten a new value above)
            teamNames.Add(remainingPlayername);
            teamids.Add(remainingPlayer);
            ProgHelpers.persistedData.AddFatKid(remainingPlayer, remainingPlayername);

            //Clear draftchat names (you could say this is redundant but..)
            ProgHelpers.draftchatnames.Clear();
            ProgHelpers.draftchatids.Clear();
        }

        private async Task CmdPick(Shard shard, DiscordMessage message) 
        {
            try 
            {
                ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                DiscordUser author = message.Author;
                string messageAuthorId = author.Id.Id.ToString();

                // verify message sender
                if (messageAuthorId != ProgHelpers.pickturn)
                {
                    if (messageAuthorId == ProgHelpers.captain2id || messageAuthorId == ProgHelpers.captain1id)
                    {
                        textChannel.CreateMessage($"<@{author.Id}> " + ProgHelpers.locale["pickPhase.notYourTurn"]);
                    }
                    else
                    {
                        textChannel.CreateMessage($"<@{author.Id}> " + ProgHelpers.locale["pickPhase.notCaptain"]);
                    }
                    return;
                }

                // execute team pick
                string nextTeam = null;
                bool pickSuccessful = false;
                if(ProgHelpers.pickturn == ProgHelpers.captain1id)
                {
                    nextTeam = "team2";
                    pickSuccessful = PickTeamMember(textChannel, author, message.Content, ProgHelpers.team1ids, ProgHelpers.team1, ProgHelpers.captain2id);    
                } 
                else 
                {
                    nextTeam = "team1";
                    pickSuccessful = PickTeamMember(textChannel, author, message.Content, ProgHelpers.team2ids, ProgHelpers.team2, ProgHelpers.captain1id);                        
                }
                if (!pickSuccessful) return;

                // automatically pick the fat kid
                if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count == (ProgHelpers.qcount - 1))
                {
                    if(ProgHelpers.pickturn == ProgHelpers.captain1id)
                    {
                        PickFatKid(ProgHelpers.team1ids, ProgHelpers.team1);
                    } else 
                    {
                        PickFatKid(ProgHelpers.team2ids, ProgHelpers.team2);
                    }                                    
                } else {
                    textChannel.CreateMessage($"<@{author.Id}> " + ProgHelpers.locale["pickPhase." + nextTeam + "Turn"] + " <@" + ProgHelpers.pickturn + "> \n " + ProgHelpers.locale["pickPhase.unpicked"] + " \n" + string.Join("\n", ProgHelpers.draftchatnames.Cast<string>().ToArray()));
                }

                // if all players have been picked show the teams and reset bot status
                if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count == ProgHelpers.qcount)
                {
                                
                    ProgHelpers.persistedData.AddHighScores(ProgHelpers.team1ids.Concat(ProgHelpers.team2ids).ToList(), ProgHelpers.team1.Concat(ProgHelpers.team2).ToList());
                    textChannel.CreateMessage(new DiscordMessageDetails()
                     .SetEmbed(new DiscordEmbedBuilder()
                     .SetTitle($"kitsun8's Gatheriino, " + ProgHelpers.locale["status.pickedTeams"])
                     .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                     .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                     .AddField("Team1: ", string.Join("\n", ProgHelpers.team1.Cast<string>().ToArray()), true)
                     .AddField("Team2: ", string.Join("\n", ProgHelpers.team2.Cast<string>().ToArray()), true)
                      ));

                    //clear other lists as well, resetting bot to default values, ready for another round!
                    ResetQueue();
                }

            } catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }            

        private static void ResetQueue()
        {
            ProgHelpers.team1.Clear();
            ProgHelpers.team1ids.Clear();
            ProgHelpers.team2.Clear();
            ProgHelpers.team2ids.Clear();
            ProgHelpers.captain1 = "";
            ProgHelpers.captain2 = "";
            ProgHelpers.captain1id = "";
            ProgHelpers.captain2id = "";
            ProgHelpers.pickturn = "";
            ProgHelpers.draftchatids.Clear();
            ProgHelpers.draftchatnames.Clear();
            ProgHelpers.queue.Clear();
            ProgHelpers.queueids.Clear();
            ProgHelpers.readycheckids.Clear();
            ProgHelpers.readycheck.Clear();
            ProgHelpers.cptrandom.Clear();
            ProgHelpers.cptrandomids.Clear();
        }

        private async Task CmdReady(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                var authorId = message.Author.Id.Id.ToString();
                var authorUserName = message.Author.Username.ToString();

                HandleReady(message, textChannel, authorId, authorUserName);
            }
            catch (Exception)
            {
                Console.WriteLine("EX-!ready" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdNoCap(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                var authorId = message.Author.Id.Id.ToString();
                var authorUserName = message.Author.Username.ToString();
                ProgHelpers.persistedData.AddNotMe(authorId, authorUserName);

                textChannel.CreateMessage($"<@"+authorId+"> " + ProgHelpers.locale["noCap.text"]);
            }
            catch (Exception)
            {
                Console.WriteLine("EX-!NoCap" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdYesCap(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                var authorId = message.Author.Id.Id.ToString();
                var authorUserName = message.Author.Username.ToString();
                ProgHelpers.persistedData.RemoveNotMe(authorId, authorUserName);

                textChannel.CreateMessage($"<@"+authorId+"> " + ProgHelpers.locale["yesCap.text"]);
            }
            catch (Exception)
            {
                Console.WriteLine("EX-!YesCap" + " --- " + DateTime.Now);
            }
        }

        private void HandleReady(DiscordMessage message, ITextChannel textChannel, string authorId, string authorUserName)
        {
            if (ProgHelpers.queueids.IndexOf(authorId) != -1)
            {
                if (ProgHelpers.draftchatids.Count > 0) 
                {
                    textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["pickPhase.alreadyInProcess"] + " " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                    return;
                }
                //check if person has added himself in the queue
                if (ProgHelpers.queue.Count != ProgHelpers.qcount)
                {
                    textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.notReadyYet"] + " " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                    return;
                }
                    //01.08.2017 Check if person has ALREADY readied...
                var checkExists = ProgHelpers.readycheckids.FirstOrDefault(x => x == authorId);
                if (checkExists != null)
                {
                    //Person has already readied
                    textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["readyPhase.alreadyMarkedReady"] + " " + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                    return;
                }
                //Proceed

                //place person in readycheck queue
                ProgHelpers.readycheckids.Add(authorId);
                ProgHelpers.readycheck.Add(authorUserName);
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["readyPhase.ready"] + " " + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                //if readycheck completes the queue, start captainpick phase, clear readycheck queue in process
                if (ProgHelpers.readycheckids.Count == ProgHelpers.qcount)
                {
                    //dispose of the currenttimer
                    _tm.Dispose();
                    ProgHelpers._counter = 0;

                    //Dispose readychecks
                    ProgHelpers.readycheckids.Clear();
                    ProgHelpers.readycheck.Clear();

                    //Run captain candidates run
                    ProgHelpers.cptrandom = ProgHelpers.queue;
                    ProgHelpers.cptrandomids = ProgHelpers.queueids;
                    //--> Run checks against persisted data, remove all that are found in the file (foreach run)
                    //--> Run random against this updated list, then run a FindIndexOf against queueids list --> We have a captain #1 
                    // -> Remove him from cptrandom list -> Run another randoming -> Run check against queueids -> We have captain #2 -> Reset the cptrandom lists

                    //Random captain 1
                    Random rnd = new Random();
                    int c1 = rnd.Next(ProgHelpers.queueids.Count);
                    string c1n = "";
                    string c1i = "";

                    c1n = ProgHelpers.queue[c1];
                    c1i = ProgHelpers.queueids[c1];
                    ProgHelpers.queue.RemoveAt(c1);
                    ProgHelpers.queueids.RemoveAt(c1);
                    ProgHelpers.captain1 = c1n;
                    ProgHelpers.captain1id = c1i;
                    ProgHelpers.team1.Add(c1n);
                    ProgHelpers.team1ids.Add(c1i);

                    //Random captain 2
                    Random rnd2 = new Random();
                    int c2 = rnd2.Next(ProgHelpers.queueids.Count);
                    string c2n = "";
                    string c2i = "";

                    c2n = ProgHelpers.queue[c2];
                    c2i = ProgHelpers.queueids[c2];
                    ProgHelpers.queue.RemoveAt(c2);
                    ProgHelpers.queueids.RemoveAt(c2);
                    ProgHelpers.captain2 = c2n;
                    ProgHelpers.captain2id = c2i;
                    ProgHelpers.team2.Add(c2n);
                    ProgHelpers.team2ids.Add(c2i);

                    ProgHelpers.persistedData.AddCaptains(c1i, c1n, c2i, c2n);

                    //Workaround to keep the initial numbering active for whole draft
                    ProgHelpers.draftchatids.AddRange(ProgHelpers.queueids);
                    List<string> draftlist = new List<string>();
                    int qcount = 0;
                    foreach (string item in ProgHelpers.queue)
                    {
                        draftlist.Add(qcount.ToString() + " - " + item);
                        qcount++;
                    }
                    ProgHelpers.draftchatnames.AddRange(draftlist);

                    Console.WriteLine(ProgHelpers.draftchatnames.Cast<string>().ToArray());
                    Console.WriteLine(ProgHelpers.draftchatids.Cast<string>().ToArray());

                    ProgHelpers.pickturn = ProgHelpers.captain1id; //initial pickturn


                    List<string> phlist = new List<string>();
                    int count = 0;
                    foreach (string item in ProgHelpers.queue)
                    {
                        phlist.Add(count.ToString() + " - " + item);
                        count++;
                    }

                    textChannel.CreateMessage(ProgHelpers.locale["pickPhase.started"] + " " + "<@" + c1i + ">" + "\n"
                                              + ProgHelpers.locale["pickPhase.team2Captain"] + " " + "<@" + c2i + ">" + "\n" + ProgHelpers.locale["pickPhase.instructions"]
                                              + "\n \n" + string.Join("\n", phlist.Cast<string>().ToArray()));
                }
            }
            else
            {
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["pickPhase.alreadyInProcess"]);
            }
            Console.WriteLine("!ready" + " --- " + DateTime.Now);
        }

        private async Task CmdRemove(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            try
            {
                if (ProgHelpers.queue.Count == ProgHelpers.qcount)
                {
                    //too late to bail out
                    textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["pickPhase.cannotRemove"]);
                }
                else
                {
                    if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count > 0)
                    {
                        textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["readyPhase.cannotAdd"]);
                    }
                    else
                    {
                        //remove player from list
                        var aa = message.Author.Id.Id.ToString();
                        var bb = message.Author.Username.ToString();

                        if (ProgHelpers.queueids.IndexOf(aa) != -1)
                        {
                            var inx = ProgHelpers.queueids.IndexOf(aa); //Get index because discord name can change, id can not
                            ProgHelpers.queueids.Remove(aa);
                            ProgHelpers.queue.RemoveAt(inx);
                            //queue.Remove(message.Author.Username);

                            textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.removed"] + " " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                            Console.WriteLine("!remove - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
                        }
                        else
                        {

                            textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.notInQueue"]);
                            Console.WriteLine("!remove - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
                        }
                    }


                }

            }
            catch (Exception)
            {
                Console.WriteLine("EX-!remove" + " --- " + DateTime.Now);
            }
        }

        private async Task CmdAdd(Shard shard, DiscordMessage message)
        {
            ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
            var authorId = message.Author.Id.Id.ToString();
            var authorUserName = message.Author.Username.ToString();
            try
            {
                HandleAdd(message, textChannel, authorId, authorUserName);                       
            }
            catch (Exception)
            {
                Console.WriteLine("EX-!add" + " --- " + DateTime.Now);
            }
        }
        private void HandleAdd(DiscordMessage message, ITextChannel textChannel, string authorId, string authorUserName)
        {
            if (ProgHelpers.queue == null) {
                return;
            }
            if (ProgHelpers.queue.Count == ProgHelpers.qcount)
            {
                //readycheck in process, cant add anymore
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["ready.alreadyInProcess"]);
                return;
            }
            //Additional check, check if the picking phase is in progress...
            if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count > 0)
            {
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["readyPhase.cannotAdd"]);
                return;
            }
            var findx = ProgHelpers.queueids.Find(item => item == authorId);
            if (findx == null)
            {
                //check offline players
                //await Offlinecheck();

                //add player to queue
                ProgHelpers.queueids.Add(authorId);
                ProgHelpers.queue.Add(authorUserName);
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.added"] + " " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                Console.WriteLine("!add - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                //check if queue is full
                if (ProgHelpers.queue.Count == ProgHelpers.qcount)
                {
                    List<string> phlist = new List<string>();
                    foreach (string item in ProgHelpers.queueids)
                    {
                        phlist.Add("<@" + item + ">");
                    }
                    //if queue complete, announce readychecks
                    textChannel.CreateMessage(ProgHelpers.locale["readyPhase.started"] + " \n" + string.Join("\t", phlist.Cast<string>().ToArray()));
                    StartTimer();
                }
            }
            else
            {
                textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.locale["queuePhase.alreadyInQueue"] + " " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                Console.WriteLine("!add - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
            }
                        
        }

        private Tuple<string, string> ParseIdAndUsername(DiscordMessage message) {
            string id = null;
            string userName = null;

            var msg = message.Content;
            if (message.Mentions.Count > 0)
            {
                id = message.Mentions[0].Id.Id.ToString();
            }
            else if (msg.Split(null).Length < 2)
            {
                id = message.Author.Id.Id.ToString();
            }

            if(msg.Split(' ').Length > 1 && id == null)
            {
                userName = msg.Substring(msg.Split(' ')[0].Length + 1);
            }
            return Tuple.Create(id, userName);
        }
    }
}