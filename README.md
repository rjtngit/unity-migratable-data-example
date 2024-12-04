# Migratable Serialized Data Example for Unity/C#

**Problem:** Your game has data that belongs to the user, i.e. save data or custom maps, and you’ve introduced breaking changes in a new version of the game. 

**Solution:** Migrate data on load so that it is compatible with the current version of the game.

## Run the Example:

Play the Example scene in Unity and check the logs. You will see the version 1 data (taken from ExampleData.json) and the version 2 data (contents after migration).

### Before:
```
{
  "id": "12345",
  "exampleValue": 100,
  "SchemaVersion": 1
}
```

### After:
```
{
  "id": "12345",
  "values": {
    "exampleValue1": 100,
    "exampleValue2": -1
  },
  "SchemaVersion": 2
}
```


## How to create a new version of your data structure:

### Static Classh Wrapper
Wrap your serialized data in a static class like “ExampleData”. Create a new static class for the previous version. Previous versions of the data are wrapped in versioned static classes like ExampleDataV1, ExampleDataV2, etc. The latest version is always “ExampleData” so that the latest version is always referenced throughout the project. The versioned static classes are only referenced in the implementation of the migration. 

### SchemaVersion
Increment the SchemaVersion when data structure changes and a new versioned static class is created. This is how the MigratableDataProcessor knows where to start the migration process. 

### MigrateFromPreviousVersion
Implement MigrateFromPreviousVersion for the current version of the data. See ExampleData.DataHandler. 

### Example Implementation

#### Latest Data Structure
```
public static class ExampleData // Current version of data structure does not have "VX" appended.
{
    public class DataHandler : BaseMigratableDataHandler<Example>
    {
        public const int SCHEMA_VERSION = 2; // Increment when data structure changes.

        public override int SchemaVersion => SCHEMA_VERSION;

        public override MigratableDataProcessor.IVersionedData MigrateFromPreviousVersion(
            MigratableDataProcessor.IVersionedData migrateFrom)
        {
            var fromData = (ExampleDataV1.Example) migrateFrom; // Migrate from the namespace for the previous version.

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
```

#### Previous Data Structure
```
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
```
