using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace RJ
{
    public static class MigratableDataProcessor
    {
        [Serializable]
        public class SchemaVersionData
        {
            public int schemaVersion;
        }

        public interface IVersionedData
        {
            public string DataTypeName => GetType().Name;
            public int SchemaVersion { get; }
        }

        private static readonly Dictionary<string, Dictionary<int, IMigratableDataHandler>> handlers;

        static MigratableDataProcessor()
        {
            handlers = new Dictionary<string, Dictionary<int, IMigratableDataHandler>>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IMigratableDataHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        var handlerInstance = Activator.CreateInstance(type) as IMigratableDataHandler;

                        if (handlerInstance != null)
                        {
                            if (!handlers.TryGetValue(handlerInstance.DataTypeName, out var handler))
                            {
                                handlers[handlerInstance.DataTypeName] = new Dictionary<int, IMigratableDataHandler>();
                            }

                            var version = handlerInstance.SchemaVersion;
                            handlers[handlerInstance.DataTypeName][version] = handlerInstance;
                        }
                    }
                }
            }
        }

        public static int GetCurrentSchemaVersion<T>() where T : IVersionedData
        {
            if (handlers.TryGetValue(typeof(T).Name, out var handlersForType))
            {
                return handlersForType.Count;
            }

            Debug.LogError($"Handler not found for {typeof(T).Name}");
            return 0;
        }

        public static T MigrateToCurrentVersion<T>(IVersionedData data) where T : IVersionedData
        {
            for (var version = data.SchemaVersion + 1; version <= GetCurrentSchemaVersion<T>(); version++)
            {
                data = handlers[data.DataTypeName][version].MigrateFromPreviousVersion(data);
            }

            return (T) data;
        }


        public static T FromJson<T>(string json) where T : IVersionedData
        {
            var schemaVersionData = JsonConvert.DeserializeObject<SchemaVersionData>(json);

            // Default to latest version if SchemaVersion missing from json
            var schemaVersion = schemaVersionData.schemaVersion;
            if (schemaVersion == 0)
            {
                schemaVersion = GetCurrentSchemaVersion<T>();
            }

            if (!handlers[typeof(T).Name].TryGetValue(schemaVersion, out var handler))
            {
                Debug.LogError($"{typeof(T).Name} Data Handler does not exist SchemaVersion {schemaVersion}");
                return default;
            }

            var data = handler.FromJson(json);
            return MigrateToCurrentVersion<T>(data);
        }

        public static string ToJson(IVersionedData data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static T Clone<T>(T fromData) where T : IVersionedData
        {
            var cloneRaw = ToJson(fromData);
            var clone = FromJson<T>(cloneRaw);
            return clone;
        }
    }

    public interface IMigratableDataHandler
    {
        public string DataTypeName { get; }
        public int SchemaVersion { get; }

        MigratableDataProcessor.IVersionedData MigrateFromPreviousVersion(
            MigratableDataProcessor.IVersionedData migrateFrom);

        MigratableDataProcessor.IVersionedData FromJson(string json);
    }

    public abstract class BaseMigratableDataHandler<T> : IMigratableDataHandler
        where T : MigratableDataProcessor.IVersionedData
    {
        public string DataTypeName => typeof(T).Name;
        public abstract int SchemaVersion { get; }

        public abstract MigratableDataProcessor.IVersionedData MigrateFromPreviousVersion(
            MigratableDataProcessor.IVersionedData migrateFrom);

        public MigratableDataProcessor.IVersionedData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}