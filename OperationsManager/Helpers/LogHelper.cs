namespace OperationsManager.Helpers
{
    public static class LogHelper
    {
        private static StreamWriter _streamWriter;
        private static readonly object _lockObject = new object();

        public static void Initialize(string filePath)
        {
            _streamWriter = new StreamWriter(filePath, true); // true to append to an existing file, false to overwrite
        }

        public static void WriteLog(string message)
        {
            lock (_lockObject)
            {
                _streamWriter.WriteLine($"{DateTime.Now} - {message}");
                _streamWriter.Flush();
            }
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
