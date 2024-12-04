# Migratable Serialized Data Example for Unity/C#

**Problem:** Your game has data that belongs to the user, i.e. save data or custom maps, and you’ve introduced breaking changes in a new version of the game. 

**Solution:** Migrate data on load so that it is compatible with the current version of the game.

## Run the Example:

Play the Example scene in Unity and check the logs. You will see the version 1 data (taken from ExampleData.json) and the version 2 data (contents after migration).

## How to create a new version of your data structure:

### Namespacing
Wrap your serialized data in a namespace like “ExampleData”. Create a new namespace for the previous version. Previous versions of the data are wrapped in versioned namespaces like ExampleDataV1, ExampleDataV2, etc. The latest version is always “ExampleData” so that the latest version is always referenced throughout the project. The versioned namespaces are only referenced in the implementation of the migration. 

### SchemaVersion
Increment the SchemaVersion for the data when data structure changes and a new versioned namespace is created. This is how the MigratableDataProcessor knows where to start the migration process. 

### MigrateFromPreviousVersion
Implement MigrateFromPreviousVersion for the current version of the data. See ExampleData.DataHandler. 
