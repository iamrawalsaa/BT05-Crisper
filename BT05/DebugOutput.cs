
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared
{
    public sealed class DebugOutput
    {
        Random random = new Random();

        private static readonly DebugOutput _instance = new DebugOutput();

        StringBuilder _output = new StringBuilder();

        public bool ShowDebug { get; set; } = false;

        string crashLogFileName = "crashlog.txt";
        string _logFileName = "none.log";
        StreamWriter? _logFileWriter = null;

        const bool SAVE_LOG_FILE = true;

        DebugOutput()
        {
            CreateLogFile();
        }

        public string DebugString
        {
            get { return _output.ToString(); }
        }

        public static DebugOutput Instance
        {
            get { return _instance; }
        }

        public string _liveString = "";
        public string LiveString
        {
            get
            {
                return _liveString;
            }
        }

       
        public void ClearLiveString()
        {
            _liveString = "";
        }

        public void AppendToLiveString(string append)
        {
            _liveString += append;
        }

        public void WriteError(string error)
        {
            string msg = "ERROR: " + error + "\n";
            _output.Append(msg);

            if (SAVE_LOG_FILE && _logFileWriter != null)
            {
                _logFileWriter.WriteLine(CurrentTime() + " : " + msg);
                _logFileWriter.Flush();
            }

            CheckLength();
        }

        public void WriteInfo(string info)
        {
            string msg = "INFO: " + info + "\n";
            _output.Append(msg);

            if (SAVE_LOG_FILE && _logFileWriter != null)
            {
                _logFileWriter.WriteLine(CurrentTime() + " : " + msg);
                _logFileWriter.Flush();
            }

            CheckLength();
        }

        private void CheckLength()
        {
            try
            {
                if (_output.Length > 800)
                {
                    _output.Remove(0, 300);
                }
            }
            catch (Exception e)
            {
                // ignore this
            }
        }

        string CurrentTime()
        {
            DateTime dateTimeNow = DateTime.Now;
            return dateTimeNow.ToString("HH:mm:ss");
        }

        string CurrentDateTime()
        {
            DateTime dateTimeNow = DateTime.Now;
            return dateTimeNow.ToShortDateString() + " : " + dateTimeNow.ToString("HH:mm:ss");
        }

        void CreateLogFile()
        {
            if (SAVE_LOG_FILE)
            {
                DateTime dateTimeNow = DateTime.Now;

                try
                {
                    _logFileName = dateTimeNow.ToString("yyMMdd_HHmm") + ".txt";
                    _logFileWriter = new StreamWriter(_logFileName, true);
                }
                catch (Exception e)
                {
                    // probably opening the same file

                    _logFileName = dateTimeNow.ToString("yyMMdd_HHmm") + "_" + random.Next() + ".txt";
                    _logFileWriter = new StreamWriter(_logFileName, true);
                }
            }
        }

        //public void OpenCrashLog()
        //{
        //    StreamWriter streamWriter = new StreamWriter(crashLogFileName, true);
        //    string output = CurrentDateTime() + "\t:\tOPEN - Build: \n";// " + GlobalConstants.Instance.LaunchDateTime + "\n";

        //    streamWriter.WriteLine(output);
        //    streamWriter.Close();
        //}

        //public void CloseCrashLog()
        //{
        //    StreamWriter streamWriter = new StreamWriter(crashLogFileName, true);
        //    string output = CurrentDateTime() + "\t:\tCLOSE\n";

        //    streamWriter.WriteLine(output);
        //    streamWriter.Close();
        //}

        public void AddToCrashLog(Exception e, string extraDescription = "")
        {
            StreamWriter streamWriter = new StreamWriter(crashLogFileName, true);
            DateTime dateTimeNow = DateTime.Now;

            if (extraDescription != null)
            {
                string outputDesc = CurrentDateTime() + "\t:\tEXCEPTION (info): " + extraDescription;
                streamWriter.WriteLine(outputDesc);
            }

            string output = CurrentDateTime() + "\t:\tEXCEPTION : " + e.Message;
            streamWriter.WriteLine(output);

            if (e.InnerException != null)
            {
                string inner = CurrentDateTime() + "\t:\tINNER EXCEPTION : " + e.InnerException.Message;
                streamWriter.WriteLine(inner);

                if (e.InnerException.InnerException != null)
                {
                    string inner2 = CurrentDateTime() + "\t:\tINNER INNER EXCEPTION: " + e.InnerException.InnerException.Message;
                    streamWriter.WriteLine(inner2);

                }
            }

            string stackTrace = CurrentDateTime() + "\t:\tSTACK TRACE : \n" + e.StackTrace;
            streamWriter.WriteLine(stackTrace);

            streamWriter.Close();

            //string body = "\n\nException: " + e.Message + "\n\n" + e.StackTrace;

            //            SendEMail(body);
        }
    }
}
