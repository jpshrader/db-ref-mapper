# Database Reference Mapper

C# Console App to find all references to databse objects within a given directory, and within a diven Sql Server database.

"Database objects" includes the following:

1. Tables
2. Stored Procs
3. User-defined Functions
4. User-defined Types
5. Views

## Configuration

You may specify the following settings in the `appSettings.json` file. These settings are as follows:

1. `ConnectionStrings`: A connection string of the database you wish to map.
2. `DirectoryToSearch`: The absolute path of the directory you wish to search.
3. `PatternsToIgnore`: Comma-separated list of directories/files to ignore when searching. Ex. `"file.cs,/folder/"`
4. `MatchingTechnique`: There are two options for this setting: `Regex` and `Contains`. `Regex` will match files/folder to the `PatternsToIgnore` list by regex patterns. `Contains` will do a simple string contains.
