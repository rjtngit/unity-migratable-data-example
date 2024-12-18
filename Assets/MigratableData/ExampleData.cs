using System;
using UnityEngine;

namespace RJ
{
    public static class ExampleData // Current version of data structure does not have "VX" appended.
    {
        public class DataHandler : BaseMigratableDataHandler<Example>
        {
            public const int SCHEMA_VERSION = 2; // Increment when data structure changes.

            public override int SchemaVersion => SCHEMA_VERSION;

            public override MigratableDataProcessor.IVersionedData MigrateFromPreviousVersion(
                MigratableDataProcessor.IVersionedData migrateFrom)
            {
                var fromData = (ExampleDataV1.Example) migrateFrom; // Migrate from the previous version.

                Debug.Log($"{DataTypeName}: Migrating from {fromData.SchemaVersion} to {SchemaVersion}");

                var values = new ValueWrapped(
                    fromData.exampleValue,  // Move existing value to new location
                    -1  // Set a default value for new data
                );
                
                return new Example(
                    fromData.id,
                    values
                );
            }
        }

        [Serializable]
        public class ValueWrapped
        {
            public int exampleValue1;
            public int exampleValue2;

            public ValueWrapped()
            {
            }

            public ValueWrapped(int exampleValue1, int exampleValue2)
            {
                this.exampleValue1 = exampleValue1;
                this.exampleValue2 = exampleValue2;
            }
        }

        [Serializable]
        public class Example : MigratableDataProcessor.IVersionedData
        {
            public string id;
            public ValueWrapped values;

            public int SchemaVersion => DataHandler.SCHEMA_VERSION;

            public Example()
            {
            }

            public Example(string id, ValueWrapped values)
            {
                this.id = id;
                this.values = values;
            }
        }
    }
}
