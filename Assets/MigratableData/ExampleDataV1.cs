using System;

namespace RJ
{
    public static class ExampleDataV1 // Append VX to previous versions of data structure.
    {
        public class DataHandler : BaseMigratableDataHandler<Example>
        {
            public const int SCHEMA_VERSION = 1; // Increment when data structure changes.

            public override int SchemaVersion => SCHEMA_VERSION;

            public override MigratableDataProcessor.IVersionedData MigrateFromPreviousVersion(
                MigratableDataProcessor.IVersionedData fromData)
            {
                throw new NotImplementedException($"{DataTypeName}: No previous schema version.");
            }
        }


        [Serializable]
        public class Example : MigratableDataProcessor.IVersionedData
        {
            public string id;
            public int exampleValue;

            public int SchemaVersion => DataHandler.SCHEMA_VERSION;

            public Example()
            {
            }

            public Example(string id, int exampleValue)
            {
                this.id = id;
                this.exampleValue = exampleValue;
            }
        }
    }
}