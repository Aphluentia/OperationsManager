using SharpCompress.Common;
using System.Runtime.CompilerServices;

namespace OperationsManager.Helpers
{
    public static class LogHelper
    {
        private static StreamWriter _streamWriter;
        private static ICollection<string> _logs; 
        private static readonly object _lockObject = new object();

        public static void Initialize(string filePath)
        {
            //string[] curLogs = File.ReadAllLines(filePath);
            _logs = new List<string>();
            //foreach (string line in curLogs) _logs.Add(line);
            _streamWriter = new StreamWriter(filePath, true); // true to append to an existing file, false to overwrite
        }

        public static void WriteLog(string message)
        {
            lock (_lockObject)
            {
                _logs.Add($"{DateTime.Now} - {message}");
                _streamWriter.WriteLine($"{DateTime.Now} - {message}");
                _streamWriter.Flush();
            }
        }
        public static ICollection<string> GetLogs()
        {
            return _logs;
        }
        public static void Dispose()
        {
            lock (_lockObject)
            {
                _streamWriter.Dispose();
            }
        }
    }
}
