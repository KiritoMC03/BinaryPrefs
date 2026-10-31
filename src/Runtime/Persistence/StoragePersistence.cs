#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Debug = UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace Appegy.Storage
{
    internal sealed class StoragePersistence
    {
        private readonly StorageFile _file;
        private readonly StorageSerializer _serializer;
        private readonly IStorageWriter _writer;

        public StoragePersistence(string filePath, IReadOnlyList<BinarySection> sections, bool saveOnBackgroundThread)
        {
            _file = StorageFile.Of(filePath);
            _serializer = new StorageSerializer(sections);
            _writer = saveOnBackgroundThread ? new BackgroundStorageWriter(_file) : new ImmediateStorageWriter(_file);
        }

        public bool SaveJsonCopyForDebug { get; set; }

        public void Load(Dictionary<string, Record> data, KeyLoadFailedBehaviour keyLoadFailedBehaviour, CorruptedFileBehaviour corruptedFileBehaviour)
        {
            _serializer.Clear(data);

            bool TryRead(string filePath, [NotNullWhen(false)] out StorageFileCorruptedException? failure)
            {
                return _serializer.TryDeserialize(filePath, data, keyLoadFailedBehaviour, out failure);
            }

            try
            {
                _file.Load(TryRead);
            }
            catch (StorageFileCorruptedException exception)
            {
                switch (corruptedFileBehaviour)
                {
                    case CorruptedFileBehaviour.ThrowException:
                        throw;
                    case CorruptedFileBehaviour.ResetToEmpty:
                        break;
                    case CorruptedFileBehaviour.ResetToEmptyWithError:
                        Debug.LogException(exception);
                        break;
                    default:
                        throw new UnexpectedEnumException(typeof(CorruptedFileBehaviour), corruptedFileBehaviour);
                }
            }
        }

        public void Save(Dictionary<string, Record> data, bool waitForDisk)
        {
            _writer.Write(_serializer.Serialize(data), waitForDisk);
            if (SaveJsonCopyForDebug)
                SaveJsonCopy(data);
        }

        public bool Flush()
        {
            return _writer.Flush();
        }

        private void SaveJsonCopy(IReadOnlyDictionary<string, Record> data)
        {
            try
            {
                if (data.Count == 0)
                    _file.RemoveDebugJson();
                else
                    _file.WriteDebugJson(DebugJsonWriter.ToJson(data));
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to save JSON debug copy of '{_file.Main}'. Reason: {exception.Message}");
            }
        }
    }
}
