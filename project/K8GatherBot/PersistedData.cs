using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CsvHelper;

namespace K8GatherBot
{
    public class PersistedData
    {
        private string fatkidFileName = "fatkid.csv";
        private string highScoreFileName = "highscores.csv";

        private List<DataPair> fatKids = new List<DataPair>();
        private List<DataPair> highScores = new List<DataPair>();

        public PersistedData()
        {
            InitData();
        }

        private void InitData() {
            InitList(fatKids, fatkidFileName);
            InitList(highScores, highScoreFileName);
        }

        private void InitList(List<DataPair> data, string fileName)
        {
            if(!File.Exists(@fileName)) {
                return;
            }
            using (TextReader fileReader = File.OpenText(@fileName))
			{
                CsvReader csvFile = new CsvReader(fileReader);
                csvFile.Configuration.HasHeaderRecord = false;
				csvFile.Read();
                var records = csvFile.GetRecords<DataPair>();
                foreach(DataPair pair in records) {
                    data.Add(pair);
                }
            }
		}

        private void PersistList(List<DataPair> data, string fileName) {
            using (TextWriter writer = new StreamWriter(File.Open(@fileName, FileMode.Create)))
			{
				var csv = new CsvWriter(writer);
				csv.Configuration.Encoding = Encoding.UTF8;
                data.Sort();
                csv.WriteRecords(data);
                writer.Flush();
			}
        }

        public void AddFatKid(string id, string userName)
        {
            Add(fatKids, id, userName);
            PersistList(fatKids, fatkidFileName);
        }

        public void AddHighScores(List<string> ids, List<string> userNames) {
            for (int i = 0; i < ids.Count; i++) {
                Add(highScores, ids[i], userNames[i]);
            }
            PersistList(highScores, highScoreFileName);
        }

        private void Add(List<DataPair> data, string id, string userName) {
            DataPair entry = data.Find(x => x.id.Equals(id));

            if(entry == null) {
                entry = new DataPair(id, userName);
                data.Add(entry);
            } else {
                entry.userName = userName;
            } 
            entry.Add();
        }

        public string GetFatKidInfo(string userName, string response)
        {
            return GetInfo(fatKids, userName, response);
        }

        public string GetHighScoreInfo(string userName, string response) {
            return GetInfo(highScores, userName, response);
        }

        private string GetInfo(List<DataPair> data, string userName, string response) {
            DataPair entry = data.Find(x => x.userName.Equals(userName));
            if (entry == null) {
                return String.Format(response, userName, 0, data.Count, data.Count);
            }
            return String.Format(response, entry.userName, entry.count, data.IndexOf(entry) + 1, data.Count);
        }

        public string GetFatKidTop10()
        {
            return GetTop10Info(fatKids);
        }


		public string GetHighScoreTop10()
		{
            return GetTop10Info(highScores);
		}

        private string GetTop10Info(List<DataPair> data) 
        {
            if(data.Count == 0) {
                return ":(";
            }
            string list = "";
            for (int i = 0; i < 10 && i != data.Count; i++) {
                DataPair entry = data[i];
                list += i+1 + ". " + entry.userName + " / " + entry.count + "\n";
            }
            return list;
        }
    }

    public class DataPair : IComparable<DataPair>, IEquatable<DataPair> {
        public string id { get; set; }
        public string userName { get; set; }
        public int count { get; set; }

        public DataPair() {
            
        }

        public DataPair(string id, string username) {
            this.id = id;
            this.userName = username;
            this.count = 0;
        }

        public DataPair(string id, string username, int count) {
            this.id = id;
            this.userName = username;
            this.count = count;
        }

        public int CompareTo(DataPair other)
        {
            if(this.count == other.count) {
                return string.Compare(this.userName, other.userName, StringComparison.OrdinalIgnoreCase);
            }
            return other.count.CompareTo(this.count);
        }

        public bool Equals(DataPair other)
        {
            return this.id.Equals(other.id);
        }

        internal void Add()
        {
            count++;
        }
    }
}
