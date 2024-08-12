using System;
using BT05;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BT05.screens;


namespace screens
{
    [System.Serializable]
    public class GameLogEntry
    {
        public int SerialNumber;
        public string Timestamp;
        public string EndTimestamp;
        public string Language;
        public string Skipped;
        public string SuccessFailure;
    }

    public class LogManager
    {
        private List<GameLogEntry> logEntries = new List<GameLogEntry>();
        private int serialNumberCounter = 1;
        private string filePath;
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
        public DateTime StartDateTime;
        public String LanguageText, SkippedText, SuccessFailureText;
        public bool hasSkipped;
        private static readonly LogManager _instance = new LogManager();


        public static LogManager Instance
        {
            get { return _instance; }
        }

        public void CSVLogger()
        {
            filePath = Path.Combine(Game1.Instance.Config.LogsPath, $"game_logs.csv");
            replacelogfile(filePath, Game1.Instance.Config.LogsPath);
            LoadLogsFromCSV();
            //LogGameStart();
        }
        private static void replacelogfile(string sourceFilePath, string destinationFolderPath)
        {
            try
            {
                if (File.Exists(sourceFilePath))
                {
                    FileInfo fileInfo = new FileInfo(sourceFilePath);
                    long fileSizeInBytes = fileInfo.Length;

                    if (fileSizeInBytes >= MaxFileSizeBytes)
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string destinationFilePath = Path.Combine(destinationFolderPath, $"log_{timestamp}.csv"); // Modify filename as needed

                        Directory.CreateDirectory(destinationFolderPath);
                        File.Move(sourceFilePath, destinationFilePath);
                    }
                    else
                    {
                        //Console.WriteLine("File size is not greater than 1 KB. No action taken.");
                    }
                }
                else
                {
                    //Console.WriteLine("Source file does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void LoadLogsFromCSV()
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 1; i < lines.Length; i++) // Skip the header line
                {
                    string[] values = lines[i].Split(',');
                    if (values.Length >= 6)
                    {
                        GameLogEntry entry = new GameLogEntry
                        {
                            SerialNumber = int.Parse(values[0]),
                            Timestamp = values[1],
                            EndTimestamp = values[2],
                            Language = values[3],
                            Skipped = values[4],
                            SuccessFailure = values[5]
                        };

                        logEntries.Add(entry);
                        serialNumberCounter = entry.SerialNumber + 1;
                    }
                }
            }
        }

        public void LogGameStart()
        {
            // Check if a game with the same SerialNumber already exists
            if (logEntries.Exists(entry => entry.SerialNumber == serialNumberCounter))
            {
                do
                {
                    serialNumberCounter++;
                } while (logEntries.Exists(entry => entry.SerialNumber == serialNumberCounter));
            }

            //DateTime startTimestamp = DateTime.Now;
            DateTime endTimestamp = DateTime.Now.AddSeconds(10);
            if (hasSkipped)
            {
                SkippedText = "Yes";
                SuccessFailureText = "NA";
            }
            else
            {
                SkippedText = "No";
            }

            GameLogEntry entry = new GameLogEntry
            {
                SerialNumber = serialNumberCounter,
                Timestamp = StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTimestamp = endTimestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                Language = LanguageText,
                Skipped = SkippedText,
                SuccessFailure = SuccessFailureText
            };

            logEntries.Add(entry);
            SaveLogsToCSV();
        }


        private void SaveLogsToCSV()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("SerialNumber,Timestamp,EndTimestamp,Language,Skipped,SuccessFailure");

            foreach (GameLogEntry entry in logEntries)
            {
                csv.Append($"{entry.SerialNumber},{entry.Timestamp},{entry.EndTimestamp},{entry.Language},{entry.Skipped},{entry.SuccessFailure}");
                csv.AppendLine();
            }

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}