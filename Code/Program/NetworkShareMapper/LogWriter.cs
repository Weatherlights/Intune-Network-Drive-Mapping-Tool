using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkShareMapper
{
    internal class LogWriter
    {
        private string appDataPath = string.Empty;
        public string component = "";
        private string wrapperAppData;

        public LogWriter(string component)
        {
            this.component = component;
            string AppDataPath = System.Environment.GetEnvironmentVariable("APPDATA");

            this.wrapperAppData = AppDataPath + "\\weatherlights\\NetworkShareMapper\\";
            this.CreateDirectoryRecursively(wrapperAppData);
        }

        public void LogWrite(string logMessage)
        {
            this.LogWrite(logMessage, 1);
        }

        private void RotateLogs()
        {
            string dateformat = "MMddyyyy";
            string timeformat = "HHmmss";

            string logFilePath = wrapperAppData + "\\" + "Execution" + ".log";
            string archivedLogFilePath = wrapperAppData + "\\" + "Execution-" + DateTime.Now.ToString(dateformat) + "-" + DateTime.Now.ToString(timeformat) + ".log";

            FileInfo LogFile = new FileInfo(logFilePath);

            if (LogFile.Exists && (LogFile.Length) >= 10 * 1048576)
            {
                LogFile.MoveTo(archivedLogFilePath);
            }
        }

        public void LogWrite(string logMessage, int type)
        {
            string logFilePath = wrapperAppData + "\\" + "Execution" + ".log";
            RotateLogs();
            try
            {
                using (StreamWriter w = File.AppendText(logFilePath))
                {
                    Log(logMessage, type, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, int type, TextWriter txtWriter)
        {
            string dateformat = "MM-dd-yyyy";
            string timeformat = "HH:mm:ss.fff-000";

            string logLine = "<![LOG[" + logMessage + "]LOG]!><time=\"" + DateTime.Now.ToString(timeformat) + "\" date=\"" + DateTime.Now.ToString(dateformat) + "\" component=\"" + component + "\" context=\"\" type=\"" + type + "\" thread=\"" + Thread.CurrentThread.ManagedThreadId + "\" file=\"\">";
            try
            {
                txtWriter.WriteLine(logLine);
            }
            catch (Exception ex)
            {
            }
        }

        private void CreateDirectoryRecursively(string path)
        {
            string[] pathParts = path.Split('\\');
            for (var i = 0; i < pathParts.Length - 1; i++)
            {
                // Correct part for drive letters
                if (i == 0 && pathParts[i].Contains(":"))
                {
                    pathParts[i] = pathParts[i] + "\\";
                } // Do not try to create last part if it has a period (is probably the file name)
                else if (i == pathParts.Length - 1 && pathParts[i].Contains("."))
                {
                    return;
                }
                if (i > 0)
                {
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                }
                if (!Directory.Exists(pathParts[i]))
                {
                    Directory.CreateDirectory(pathParts[i]);
                }
            }
        }
    }
}

