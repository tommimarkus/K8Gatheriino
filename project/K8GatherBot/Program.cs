using Discore;
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

namespace K8GatherBot
{
    

    public class Program
    {
        
        public static class ProgHelpers
        {
            //things needed for obvious reasons
            public static IConfigurationRoot Configuration { get; set; }
            public static List<string> queue = new List<string>(); //names
            public static List<string> queueids = new List<string>(); //discord ids
            public static List<string> readycheckids = new List<string>(); //readycheck
            public static List<string> readycheck = new List<string>(); //readychecknames

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
            public static string usechannel = "";
            public static string bottoken = "";
            public static string txtversion = "";
            public static string language = ""; //en-fi

            //--------------------------------------LANGUAGETXTs
            public static string txt1 = "Please wait until the previous queue is handled.";
            public static string txt2 = "Added!";
            public static string txt3 = "Queue is now full, proceed to mark yourself ready with ***!ready*** \n You have 60 seconds!";
            public static string txt4 = "You're already in the queue!";
            public static string txt5 = "Too late to back down now! Wait until the queue is handled.";
            public static string txt6 = "Removed!";
            public static string txt7 = "You are not in the queue right now.";
            public static string txt8 = "Queue is not finished yet!";
            public static string txt9 = "Ready!";
            public static string txt10 = "Readycheck complete, starting picking phase! First picking turn: Team 1 \n Team 1 Captain:";
            public static string txt11 = "Team 2 Captain:";
            public static string txt12 = "Pick players using ***!pick NUMBER***";
            public static string txt13 = "It seems you're not in the queue.";
            public static string txt14 = "Player added to Team 1!\n Team 2 Turn to pick!";
            public static string txt14x = "Player added to Team 2! \n Team 1 Turn to pick!";
            public static string txt15 = "***Remaining players:***";
            public static string txt16 = "That player is already in a team!";
            public static string txt17 = "Couldn't place a player with that index";
            public static string txt18 = "Not your turn to pick right now!";
            public static string txt19 = "You are not the captain of either teams. Picking is restricted to captains.";
            public static string txt20 = "Nobody in the queue! use ***!add***  to start queue!";
            public static string txt21 = "All lists emptied successfully.";
            public static string txt22 = "Selected teams";
            public static string txt23 = "current queue";
            public static string txt24 = "Get people to gather and play together";
            public static string txt25 = "Only a droplet of coffee was used to develop this bot. :thinking:";
            public static string txt26 = "Developer";
            public static string txt27 = "Purpose";
            public static string txt28 = "Fun fact";
            public static string txt29 = "Commands";
            public static string txt30 = "Players";
            public static string txt31 = "NOT READY YET:";
            //todo: timer and offlinechecker
            public static string txt32 = "Removed because status is ***Offline***";
            public static string txt33 = "Starting Readycheck timer, you have 60 seconds to ***!ready*** yourself.";
            public static string txt34 = "Not all players were ready during the readycheck. Returning to queue with players that were ready.";

            public static string txt35 = "You have already readied!";
        }

        public static void Main(string[] args)
        {
            //Get settings and GO
            Console.WriteLine("Reading settings from appsettings.json");

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            ProgHelpers.Configuration = builder.Build();

            Console.WriteLine("START SETTINGS-----------------------------");
            ProgHelpers.qcount = Convert.ToInt32(ProgHelpers.Configuration["Settings:Queuesize"]);
            Console.WriteLine("qcount:" +Convert.ToInt32(ProgHelpers.Configuration["Settings:Queuesize"]));
            ProgHelpers.counterlimit = Convert.ToInt32(ProgHelpers.Configuration["Settings:Readytimer"]);
            Console.WriteLine("counterlimit:" +Convert.ToInt32(ProgHelpers.Configuration["Settings:Readytimer"]));
            ProgHelpers.language = ProgHelpers.Configuration["Settings:Language"];
            Console.WriteLine("language:" +ProgHelpers.Configuration["Settings:Language"]);
            ProgHelpers.usechannel = ProgHelpers.Configuration["Settings:AllowedChannel"];
            Console.WriteLine("usechannel:" + ProgHelpers.Configuration["Settings:AllowedChannel"]);
            ProgHelpers.bottoken = ProgHelpers.Configuration["Settings:BotToken"];
            Console.WriteLine("token:" + ProgHelpers.Configuration["Settings:BotToken"]);
            ProgHelpers.txtversion = ProgHelpers.Configuration["Settings:Version"];
            Console.WriteLine("txtversion:" + ProgHelpers.Configuration["Settings:Version"]);
            Console.WriteLine("END SETTINGS-----------------------------");


            if (ProgHelpers.language == "fi")
            {
                ProgHelpers.txt1 = "Odota kunnes edellinen jono on käsitelty.";
                ProgHelpers.txt2 = "Lisätty!";
                ProgHelpers.txt3 = "Jono on nyt täynnä, merkitse itsesi valmiiksi käyttäen ***!ready*** komentoa. \n Aikaa 60 sekuntia!";
                ProgHelpers.txt4 = "Olet jo jonossa!";
                ProgHelpers.txt5 = "Liian myöhäistä peruuttaa enää, odota jonon käsittelyn valmistumista.";
                ProgHelpers.txt6 = "Poistettu!";
                ProgHelpers.txt7 = "Et ole juuri nyt jonossa";
                ProgHelpers.txt8 = "Jono ei ole vielä valmis!";
                ProgHelpers.txt9 = "Valmiina!";
                ProgHelpers.txt10 = "Readycheck valmis, aloitetaan poimintavaihe! Ensimmäinen poiminta: Team 1 \n Team1:n kapteeni:";
                ProgHelpers.txt11 = "Team 2:n kapteeni:";
                ProgHelpers.txt12 = "Poimi pelaajia käyttäen ***!pick NUMERO***";
                ProgHelpers.txt13 = "Näyttäisi siltä ettet ole jonossa.";
                ProgHelpers.txt14 = "Pelaaja lisätty Team 1:n! \n Team 2:n vuoro poimia!";
                ProgHelpers.txt14x = "Pelaaja lisätty Team 2:n \n Team 1:n vuoro poimia!";
                ProgHelpers.txt15 = "***Poimimatta:***";
                ProgHelpers.txt16 = "Pelaaja on jo joukkueessa!";
                ProgHelpers.txt17 = "Numerolla ei löytynyt pelaajaa!";
                ProgHelpers.txt18 = "Ei ole vuorosi poimia!";
                ProgHelpers.txt19 = "Vain kapteenit voivat poimia pelaajia!";
                ProgHelpers.txt20 = "Jono on tyhjä! Käytä ***!add*** aloittaaksesi jonon!";
                ProgHelpers.txt21 = "Kaikki listat tyhjennetty onnistuneesti!";
                ProgHelpers.txt22 = "Valitut joukkueet";
                ProgHelpers.txt23 = "jonon tilanne";
                ProgHelpers.txt24 = "Saada pelaajia keräytymään pelien äärelle!";
                ProgHelpers.txt25 = "Vain tilkka kahvia käytettiin kehitykseen. :thinking:";
                ProgHelpers.txt26 = "Kehittäjä";
                ProgHelpers.txt27 = "Tarkoitus";
                ProgHelpers.txt28 = "Tiesitkö";
                ProgHelpers.txt29 = "Komennot";
                ProgHelpers.txt30 = "Pelaajat";
                ProgHelpers.txt31 = "EI VALMIINA";
                ProgHelpers.txt32 = "Poistettu koska käyttäjä on ***OFFLINE***.";
                ProgHelpers.txt33 = "Aloitetaan readycheck. Teillä on 60 sekuntia aikaa käyttää ***!ready*** komentoa.";
                ProgHelpers.txt34 = "Kaikki pelaajat eivät olleet valmiita readycheckin aikana. Palataan jonoon valmiina olleiden pelaajien kanssa.";
                ProgHelpers.txt35 = "Olet jo merkinnyt itsesi valmiiksi!";
            }
            Program program = new Program();
            program.Run().Wait();
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
            ulong xxid = (ulong)Convert.ToInt64(ProgHelpers.usechannel);
            xx.Id = xxid;
            ITextChannel textChannel = (ITextChannel)shard.Application.HttpApi.Channels.CreateMessage(xx, ProgHelpers.txt34);
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
        private static async void Gateway_OnMessageCreated(object sender, MessageEventArgs e)
        {
            Shard shard = e.Shard;
            DiscordMessage message = e.Message;

            //Open helper functions
            KGHelper kgh = new KGHelper();
            
            if (message.Author == shard.User)
                // Ignore messages created by our bot.
                return;

            if (message.ChannelId.Id.ToString() == ProgHelpers.usechannel)//Prevent DM abuse, only react to messages sent on a set channel.
            {
                //--------------------------------------------------------------------------------------------------------QUEUECHECKER-run on each message before draft
                //Check offline status for ids in the queue
                //if offline, delete from lists
                //run this at all functions touching the queue, before picking phase 
                //async Task Offlinecheck()
                //{
                //    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                //    int cnt = 0;
                //    foreach (string item in ProgHelpers.queueids)
                //    {
                //        Snowflake pollid = new Snowflake();
                //        ulong pidhlp = (ulong)Convert.ToInt64(ProgHelpers.queueids[cnt]);
                //        pollid.Id = pidhlp;

                //        DiscordUser member = shard.Cache.Users.Get(pollid);
                //        DiscordUserPresence pres = //shard.Cache.Users.Get(pollid) ??


                //        if (pres.Status = "offline")
                //        {
                //            var inx = ProgHelpers.queueids.IndexOf(ProgHelpers.queueids[cnt]); //Get index because discord name can change, id can not
                //            var idtochat = ProgHelpers.queue[inx];
                //            ProgHelpers.queueids.RemoveAt(cnt);
                //            ProgHelpers.queue.RemoveAt(inx);
                //            await textChannel.CreateMessage($"<@" + idtochat + "> " + ProgHelpers.txt32);

                //            cnt = 0; //start over if an offline user is found
                //        }
                //        else
                //        {
                //            cnt++; //go to next iteration
                //        }
                //    }

                //}
                //--------------------------------------------------------------------------------------------------------READYCHECKTIMER
                //Start timer when queue completed and readycheck starts
                //Push not ready people off the queuelist when timer is up, clear readychecklists and announce current queue status

                //--------------------------------------------------------------------------------------------------------ADD-donev1
                if (message.Content == "!add")
                {
                    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                    try
                    {

                        if (ProgHelpers.queue != null)
                        {
                            if (ProgHelpers.queue.Count == ProgHelpers.qcount)
                            {
                                //readycheck in process, cant add anymore
                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt1);
                            }
                            else
                            {
                                var aa = message.Author.Id.Id.ToString();
                                var bb = message.Author.Username.ToString();
                                var findx = ProgHelpers.queueids.Find(item => item == aa);
                                if (findx == null)
                                {
                                    //check offline players
                                    //await Offlinecheck();

                                    //add player to queue
                                    ProgHelpers.queueids.Add(aa);
                                    ProgHelpers.queue.Add(bb);
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt2+" "+ ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
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
                                        await textChannel.CreateMessage(ProgHelpers.txt3 + " \n" + string.Join("\t", phlist.Cast<string>().ToArray()));
                                        StartTimer();
                                    }
                                }
                                else
                                {
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt4+" "+ ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                                    Console.WriteLine("!add - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
                                }

                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!add" + " --- " + DateTime.Now);
                    }
                }

                //--------------------------------------------------------------------------------------------------------REMOVE-donev1
                if (message.Content == "!remove" || message.Content == "!rm")
                {
                    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                    try
                    {
                        if (ProgHelpers.queue.Count == ProgHelpers.qcount)
                        {
                            //too late to bail out
                            await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt5);
                        }
                        else
                        {
                            //check offline players
                            //await Offlinecheck();

                            //remove player from list
                            var aa = message.Author.Id.Id.ToString();
                            var bb = message.Author.Username.ToString();

                            if (ProgHelpers.queueids.IndexOf(aa) != -1)
                            {
                                var inx = ProgHelpers.queueids.IndexOf(aa); //Get index because discord name can change, id can not
                                ProgHelpers.queueids.Remove(aa);
                                ProgHelpers.queue.RemoveAt(inx);
                                //queue.Remove(message.Author.Username);

                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt6 +" "+ ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                                Console.WriteLine("!remove - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
                            }
                            else
                            {

                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt7);
                                Console.WriteLine("!remove - " + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + " --- " + DateTime.Now);
                            }

                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!remove" + " --- " + DateTime.Now);
                    }
                }
                //--------------------------------------------------------------------------------------------------------READY-donev1
                if (message.Content == "!ready" || message.Content =="!r")
                {
                    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                    try
                    {
                        var aa = message.Author.Id.Id.ToString();
                        var bb = message.Author.Username.ToString();

                        if (ProgHelpers.queueids.IndexOf(aa) != -1)
                        {

                            //check if person has added himself in the queue
                            if (ProgHelpers.queue.Count != ProgHelpers.qcount)
                            {
                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt8+" "+ ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                            }
                            else
                            {
                                //01.08.2017 Check if person has ALREADY readied...
                                var checkExists = ProgHelpers.readycheckids.FirstOrDefault(x => x == aa);
                                if (checkExists != null)
                                {
                                    //Person has already readied
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.txt35 + " " + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                                }
                                else
                                {
                                    //Proceed

                                    //place person in readycheck queue
                                    ProgHelpers.readycheckids.Add(aa);
                                    ProgHelpers.readycheck.Add(bb);
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.txt9 + " " + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString());
                                    //if readycheck completes the queue, start captainpick phase, clear readycheck queue in process
                                    if (ProgHelpers.readycheckids.Count == ProgHelpers.qcount)
                                    {
                                        //dispose of the currenttimer
                                        _tm.Dispose();
                                        ProgHelpers._counter = 0;

                                        //Dispose readychecks
                                        ProgHelpers.readycheckids.Clear();
                                        ProgHelpers.readycheck.Clear();

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

                                        await textChannel.CreateMessage(ProgHelpers.txt10 + " " + "<@" + c1i + ">" + "\n" + ProgHelpers.txt11 + " " + "<@" + c2i + ">" + "\n" + ProgHelpers.txt12 + "\n \n" + string.Join("\n", phlist.Cast<string>().ToArray()));



                                    }
                                }
                                
                            }
                        }
                        else
                        {
                            await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt13);
                        }
                        Console.WriteLine("!ready" + " --- " + DateTime.Now);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!ready" + " --- " + DateTime.Now);
                    }

                }


                //--------------------------------------------------------------------------------------------------------PICK
                if (message.Content.StartsWith("!pick") || message.Content.StartsWith("!p"))
                {
                    try
                    {
                        ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                        if (message.Author.Id.Id.ToString() == ProgHelpers.pickturn)
                        {
                            
                            
                            //pickturn for team1
                            if (ProgHelpers.pickturn == ProgHelpers.captain1id)
                            {
                                //check if index exists, check if value of that index is already in a team
                                //get the number (index) to "sel"
                                var msg = message.Content;
                                string[] msgsp = msg.Split(null);
                                int sel = 0;
                                var sels2 = "";
                                var sels2name = "";
                                Int32.TryParse(msgsp[1], out sel);

                                if (ProgHelpers.queueids.ElementAtOrDefault(sel) != null)
                                {
                                    //Assign string value of discord id and name 
                                    sels2 = ProgHelpers.queueids.ElementAtOrDefault(sel);
                                    sels2name = ProgHelpers.queue.ElementAtOrDefault(sel);

                                    //check if exists in current teams
                                    if (ProgHelpers.team1ids.IndexOf(sels2) == -1)
                                    {
                                        if(ProgHelpers.team2ids.IndexOf(sels2) == -1)
                                        {
                                            //checks complete, adding player to team
                                            ProgHelpers.team1.Add(sels2name);
                                            ProgHelpers.team1ids.Add(sels2);
                                            ProgHelpers.pickturn = ProgHelpers.captain2id;

                                            //find and remove player from playerlist shown to users
                                            int finderremover = ProgHelpers.draftchatids.IndexOf(sels2);
                                            ProgHelpers.draftchatnames.RemoveAt(finderremover);
                                            ProgHelpers.draftchatids.RemoveAt(finderremover);

                                            Console.WriteLine(ProgHelpers.draftchatnames.Cast<string>().ToArray());

                                            //post the remaining players
                                            if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count != ProgHelpers.qcount)
                                            {
                                                await textChannel.CreateMessage($"<@{message.Author.Id}>"+ProgHelpers.txt14+" <@" + ProgHelpers.captain2id + "> \n "+ProgHelpers.txt15+" \n" + string.Join("\n", ProgHelpers.draftchatnames.Cast<string>().ToArray()));
                                            }
                                        }
                                        else
                                        {
                                            await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt16);
                                        }
                                    }
                                    else
                                    {
                                        await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt16);
                                    }
                                }
                                else
                                {
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.txt17);
                                }
                                //if exists, place index in other list. 

                            }
                            //pickturn for team2
                            else
                            {
                                //check if index exists, check if value of that index is already in a team
                                //get the number (index) to "sel"
                                var msg = message.Content;
                                string[] msgsp = msg.Split(null);
                                int sel = 0;
                                var sels21 = "";
                                var sels21name = "";
                                Int32.TryParse(msgsp[1], out sel);

                                if (ProgHelpers.queueids.ElementAtOrDefault(sel) != null)
                                {
                                    //Assign string value of discord id and name                                   
                                    sels21 = ProgHelpers.queueids.ElementAtOrDefault(sel);
                                    sels21name = ProgHelpers.queue.ElementAtOrDefault(sel);

                                    //check if exists in current teams
                                    if (ProgHelpers.team1ids.IndexOf(sels21) == -1)
                                    {
                                        if (ProgHelpers.team2ids.IndexOf(sels21) == -1)
                                        {
                                            //checks complete, adding player to team
                                            ProgHelpers.team2.Add(sels21name);
                                            ProgHelpers.team2ids.Add(sels21);
                                            ProgHelpers.pickturn = ProgHelpers.captain1id;

                                            //find and remove player from playerlist shown to users
                                            int finderremover = ProgHelpers.draftchatids.IndexOf(sels21);
                                            ProgHelpers.draftchatnames.RemoveAt(finderremover);
                                            ProgHelpers.draftchatids.RemoveAt(finderremover);

                                            Console.WriteLine(ProgHelpers.draftchatnames.Cast<string>().ToArray());
                                            //post the remaining players
                                            if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count != ProgHelpers.qcount)
                                            {
                                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt14x+ "<@" + ProgHelpers.captain1id + "> \n "+ProgHelpers.txt15+" \n" + string.Join("\n", ProgHelpers.draftchatnames.Cast<string>().ToArray()));
                                            }
                                        }
                                        else
                                        {
                                            await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt16);
                                        }
                                    }
                                    else
                                    {
                                        await textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.txt16);
                                    }
                                }
                                else
                                {
                                    await textChannel.CreateMessage($"<@{message.Author.Id}> " + ProgHelpers.txt17);
                                }
                                //if exists, place index in other list. 

                            }

                            //01.08.2017 Check if queue has only one player left, if so, put it in team
                            //This should lead to the queue completing in this loop
                            if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count == (ProgHelpers.qcount - 1))
                            {
                                //Find out team that is picking
                                if (ProgHelpers.pickturn == ProgHelpers.captain1id)
                                {
                                    //Team1 is picking
                                    //Find out the remaining player
                                    var remainingPlayer = ProgHelpers.draftchatids.FirstOrDefault();
                                    var remainingPlayerIndex = ProgHelpers.queueids.IndexOf(remainingPlayer);
                                    var remainingPlayername = ProgHelpers.queue.ElementAtOrDefault(remainingPlayerIndex);

                                    //Put remaining player in picking team (pickturn has already gotten a new value above)
                                    ProgHelpers.team1.Add(remainingPlayername);
                                    ProgHelpers.team1ids.Add(remainingPlayer);

                                    //Clear draftchat names (you could say this is redundant but..)
                                    ProgHelpers.draftchatnames.Clear();
                                    ProgHelpers.draftchatids.Clear();

                                }
                                else
                                {
                                    //Team2 is picking
                                    //Find out the remaining player
                                    var remainingPlayer = ProgHelpers.draftchatids.FirstOrDefault();
                                    var remainingPlayerIndex = ProgHelpers.queueids.IndexOf(remainingPlayer);
                                    var remainingPlayername = ProgHelpers.queue.ElementAtOrDefault(remainingPlayerIndex);

                                    //Put remaining player in picking team (pickturn has already gotten a new value above)
                                    ProgHelpers.team2.Add(remainingPlayername);
                                    ProgHelpers.team2ids.Add(remainingPlayer);

                                    //Clear draftchat names (you could say this is redundant but..)
                                    ProgHelpers.draftchatnames.Clear();
                                    ProgHelpers.draftchatids.Clear();
                                }


                            }

                            //if queue completes with this pick, announce teams
                            if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count == ProgHelpers.qcount)
                            {
                                ProgHelpers.queue.Clear();
                                ProgHelpers.queueids.Clear();
                                ProgHelpers.readycheckids.Clear();
                                ProgHelpers.readycheck.Clear();

                                if (ProgHelpers.queueids.Count == 0)
                                {
                                    await textChannel.CreateMessage(new DiscordMessageDetails()
                                     .SetEmbed(new DiscordEmbedBuilder()
                                     .SetTitle($"kitsun8's Gatheriino, "+ProgHelpers.txt22)
                                     .SetFooter("Discore (.NET Core), C#, "+ProgHelpers.txtversion)
                                     .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                     .AddField("Team1: ", string.Join("\n", ProgHelpers.team1.Cast<string>().ToArray()), true)
                                     .AddField("Team2: ", string.Join("\n", ProgHelpers.team2.Cast<string>().ToArray()), true)
                                      ));

                                    //clear other lists as well, resetting bot to default values, ready for another round!
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
                                }
                            }

                        }
                        else
                        {
                            if (message.Author.Id.Id.ToString() == ProgHelpers.captain2id || message.Author.Id.Id.ToString() == ProgHelpers.captain1id)
                            {
                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt18);
                            }
                            else
                            {
                                await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt19);
                            }
                        }
                        Console.WriteLine("!pick" + " --- " + DateTime.Now);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!pick" + " --- " + DateTime.Now);
                    }
                    //place person (index of player) in team (id and name)

                }
                //--------------------------------------------------------------------------------------------------------status-donev1
                if (message.Content == "!gstatus" || message.Content == "!gs")
                {
                    //print current queue.
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
                                    await textChannel.CreateMessage(new DiscordMessageDetails()
                                    .SetEmbed(new DiscordEmbedBuilder()
                                    .SetTitle($"kitsun8's GatherBot, readycheck  " + "(" + ProgHelpers.readycheckids.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + ")")
                                    .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                                    .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                    .AddField(ProgHelpers.txt31 + " ", string.Join("\n", notinlist.Cast<string>().ToArray()), false)
                                    ));
                                
                            }
                            else
                            {
                                if (ProgHelpers.team1ids.Count + ProgHelpers.team2ids.Count > 0)
                                {
                                    //picking phase
                                    await textChannel.CreateMessage(new DiscordMessageDetails()
                                    .SetEmbed(new DiscordEmbedBuilder()
                                    .SetTitle($"kitsun8's GatherBot, " + ProgHelpers.txt22)
                                    .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                                    .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                    .AddField("Team1: ", string.Join("\n", ProgHelpers.team1.Cast<string>().ToArray()), true)
                                    .AddField("Team2: ", string.Join("\n", ProgHelpers.team2.Cast<string>().ToArray()), true)
                                    ));
                                }
                                else
                                {
                                    //queue phase
                                    await textChannel.CreateMessage(new DiscordMessageDetails()
                                    .SetEmbed(new DiscordEmbedBuilder()
                                    .SetTitle($"kitsun8's GatherBot, " + ProgHelpers.txt23 + " " + "(" + ProgHelpers.queue.Count.ToString() + "/" + ProgHelpers.qcount.ToString() + ")")
                                    .SetFooter("Discore (.NET Core), C#, " + ProgHelpers.txtversion)
                                    .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                                    .AddField(ProgHelpers.txt30 + " ", string.Join("\n", ProgHelpers.queue.Cast<string>().ToArray()), false)
                                    ));
                                }
                                
                            }
                           
                            Console.WriteLine("!status");
                        }
                        else
                        {
                            await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt20);
                            Console.WriteLine("!status" + " --- " + DateTime.Now);
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!status" + " --- " + DateTime.Now);
                    }

                }
                //--------------------------------------------------------------------------------------------------------RESETBOT-donev1
                if (message.Content == "!resetbot")
                {
                    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);
                    try
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

                        await textChannel.CreateMessage($"<@{message.Author.Id}> "+ProgHelpers.txt21);
                        Console.WriteLine("!resetbot" + " --- " + DateTime.Now);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("EX-!resetbot" + " --- " + DateTime.Now);
                    }
                }

                //-----------------------------------------------------------------------------------------INFO - VALMIS V1
                if (message.Content == "!gatherinfo" || message.Content == "!gi")
                {
                    ITextChannel textChannel = (ITextChannel)shard.Cache.Channels.Get(message.ChannelId);

                    try
                    {

                        // Reply to the user who posted "!info".
                        await textChannel.CreateMessage(new DiscordMessageDetails()
                         .SetEmbed(new DiscordEmbedBuilder()
                         .SetTitle($"kitsun8's GatherBot")
                         .SetFooter("Discore (.NET Core), C# , "+ProgHelpers.txtversion)
                         .SetColor(DiscordColor.FromHexadecimal(0xff9933))
                         .AddField(ProgHelpers.txt26+" ", "kitsun8#4567", false)
                         .AddField(ProgHelpers.txt27+" ", ProgHelpers.txt24, false)
                         .AddField(ProgHelpers.txt28+" ", ProgHelpers.txt25, false)
                         .AddField(ProgHelpers.txt29+" ", "!add, !remove/rm, !ready/r, !pick/p, !gatherinfo/gi, !gstatus/gs, !resetbot", false)
                        ));

                        Console.WriteLine($"!gatherinfo - " + message.Author.Username + "-" + message.Author.Id + " --- " + DateTime.Now);
                    }
                    catch (Exception) { Console.WriteLine($"!gatherinfo - EX -" + message.Author.Username + "-" + message.Author.Id + " --- " + DateTime.Now); }
                }
                
            }
        }
    }
}