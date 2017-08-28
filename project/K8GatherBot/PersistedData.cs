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
                csvFile.Configuration.HasHeaderRecord = true;
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

        public void AddFatKid(String userName)
        {
            Add(fatKids, userName);
            PersistList(fatKids, fatkidFileName);
        }

        public void AddHighScores(IEnumerable<string> userNames) {
            foreach(string userName in userNames) {
                Add(highScores, userName);
            }
            PersistList(highScores, highScoreFileName);
        }

        public void Add(List<DataPair> data, String userName) {
            DataPair entry = fatKids.Find(x => x.userName.Equals(userName));

            if(entry == null) {
                entry = new DataPair(userName);
                fatKids.Add(entry);
            } 
            entry.Add();
        }

        public string GetFatKidInfo(string userName, string response)
        {
            DataPair fatKid = fatKids.Find(x => x.userName.Equals(userName));
            if (fatKid == null) {
                return String.Format(response, userName, 0, fatKids.Count, fatKids.Count);
            }
            return String.Format(response, fatKid.userName, fatKid.count, fatKids.IndexOf(fatKid) + 1, fatKids.Count);
        }
    }

    public class DataPair : IComparable<DataPair>, IEquatable<DataPair> {
        public string userName { get; }
        public int count { get; set; }

        public DataPair(string username) {
            this.userName = username;
            this.count = 0;
        }

        public DataPair(string username, int count) {
            this.userName = username;
            this.count = count;
        }

        public int CompareTo(DataPair other)
        {
            if(this.count == other.count) {
                return string.Compare(this.userName, other.userName, StringComparison.OrdinalIgnoreCase);
            }
            return this.count.CompareTo(other.count);
        }

        public bool Equals(DataPair other)
        {
            return this.userName.Equals(other.userName);
        }

        internal void Add()
        {
            count++;
        }
    }
}
