#nullable enable

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal sealed class StorageFile
    {
        internal const string TempFileExtension = ".tmp";
        internal const string BackupFileExtension = ".bak";
        internal const string DebugJsonFileExtension = ".json";

        private const int NoBuffering = 1;

        private static readonly ConcurrentDictionary<string, StorageFile> _files = new();
        private static readonly UTF8Encoding _debugJsonEncoding = new(false);
        public readonly string Backup;
        public readonly string DebugJson;

        public readonly string Main;
        public readonly string Temp;

        private readonly object _publishGate = new();

        private StorageFile(string mainFilePath)
        {
            Main = mainFilePath;
            Temp = mainFilePath + TempFileExtension;
            Backup = mainFilePath + BackupFileExtension;
            DebugJson = mainFilePath + DebugJsonFileExtension;
        }

        public static StorageFile Of(string filePath)
        {
            return _files.GetOrAdd(Normalize(filePath), path => new StorageFile(path))
                ?? throw new FileNotFoundException(filePath);
        }

        public static string Normalize(string filePath)
        {
            return Path.GetFullPath(filePath).TrimEnd(Path.DirectorySeparatorChar)
                ?? throw new FileNotFoundException(filePath);
        }

        public void Publish(StorageSnapshot snapshot)
        {
            try
            {
                lock (_publishGate)
                {
                    if (snapshot.Buffer == null)
                    {
                        RemoveFiles();
                        return;
                    }

                    WriteTemp(snapshot.Buffer, snapshot.Length);
                    if (File.Exists(Main))
                        File.Replace(Temp, Main, Backup);
                    else
                        File.Move(Temp, Main);
                }
            }
            finally
            {
                snapshot.Release();
            }
        }

        public void Remove()
        {
            lock (_publishGate)
            {
                RemoveFiles();
            }
        }

        public void WriteDebugJson(string json)
        {
            EnsureDirectoryExists();
            File.WriteAllText(DebugJson, json, _debugJsonEncoding);
        }

        public void RemoveDebugJson()
        {
            DeleteFileIfExists(DebugJson);
        }

        public void Load(ReadAttempt tryRead)
        {
            lock (_publishGate)
            {
                if (!File.Exists(Main))
                {
                    if (!File.Exists(Temp) || !File.Exists(Backup))
                    {
                        return;
                    }
                    File.Move(Temp, Main);
                }

                if (tryRead(Main, out var failure))
                {
                    DeleteFileIfExists(Temp);
                    return;
                }

                if (File.Exists(Backup))
                {
                    File.Replace(Backup, Main, null);
                    if (tryRead(Main, out _))
                    {
                        DeleteFileIfExists(Temp);
                        return;
                    }
                }

                DeleteFileIfExists(Main);
                ExceptionDispatchInfo.Capture(failure).Throw();
            }
        }

        private void WriteTemp(byte[] buffer, int length)
        {
            EnsureDirectoryExists();
            using var stream = new FileStream(
                Temp,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                NoBuffering
            );
            stream.Write(buffer, 0, length);
            stream.Flush(true);
        }

        private void RemoveFiles()
        {
            DeleteFileIfExists(Backup);
            DeleteFileIfExists(Main);
            DeleteFileIfExists(Temp);
        }

        private void EnsureDirectoryExists()
        {
            var directoryName = Path.GetDirectoryName(Main);
            if (directoryName != null && !string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }

        private static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        internal delegate bool ReadAttempt
            (string filePath, [NotNullWhen(false)] out StorageFileCorruptedException? failure);
    }
}
