using db_ref_finder.Common;
using System.Collections.Concurrent;

namespace db_ref_finder {
	public static class Program {
		private static readonly string reportFileSeparator = new('=', 50);

		public static void Main() {
			var appSettingsProvider = new AppSettingsProvider();
			var sqlReferenceMap = new ConcurrentDictionary<string, ReferenceList>();

			var dbAccess = new DbAccess(appSettingsProvider.GetConnectionString());
			BuildReferenceMap(sqlReferenceMap, dbAccess);

			Console.WriteLine("Retrieving Config Values...");
			var directoryToSearch = appSettingsProvider.GetValue(nameof(DbRefArgs.DirectoryToSearch));

			var patternsToIgnoreString = appSettingsProvider.GetValue(nameof(DbRefArgs.PatternsToIgnore));
			var patternsToIgnore = patternsToIgnoreString.Split(',');

			var matchingTechniqueString = appSettingsProvider.GetValue(nameof(DbRefArgs.MatchingTechnique));
			var matchingTechnique = (MatchingTechnique)Enum.Parse(typeof(MatchingTechnique), matchingTechniqueString);

			Console.WriteLine("Processing Local Files...");
			var dbRefArgs = new DbRefArgs(directoryToSearch, patternsToIgnore, matchingTechnique);
			DirectoryDirector.Scour(directoryToSearch, dbRefArgs, sqlReferenceMap);
			SqlProcInspector.Inspect(dbAccess, sqlReferenceMap);

			var outputPath = WriteOutputFile(sqlReferenceMap);
			Console.WriteLine($"File written to: {outputPath}");
			Console.ReadKey();
		}

		private static void BuildReferenceMap(ConcurrentDictionary<string, ReferenceList> sqlReferenceMap, DbAccess dbAccess) {
			var sqlObjectTypes = new List<SqlEntityType> {
				SqlEntityType.Table,
				SqlEntityType.StoredProcedure,
				SqlEntityType.Function,
				SqlEntityType.View,
				SqlEntityType.Type
			};

			foreach (var type in sqlObjectTypes) {
				var dbEntityNames = dbAccess.GetDbEntities(type);
				sqlReferenceMap.AddToReferenceMap(dbEntityNames, SqlEntityType.Table);
			}
		}

		private static void AddToReferenceMap(this ConcurrentDictionary<string, ReferenceList> sqlReferenceMap, IEnumerable<string> sqlEntityNames, SqlEntityType systemType) {
			foreach (var sqlEntityName in sqlEntityNames) {
				sqlReferenceMap.TryAdd(sqlEntityName, new ReferenceList(sqlEntityName, systemType));
			}
		}

		private static string WriteOutputFile(ConcurrentDictionary<string, ReferenceList> referenceMap) {
			var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "sql-references.txt");

			using (var outputFile = new StreamWriter(pathToFile, append: false)) {
				var itemsWithNoRefs = referenceMap.Values.Where(r => r.FilesReferencing.Count == 0 && r.ProcsReferencing.Count == 0).Select(r => r.Name);
				outputFile.WriteLine($"Items with no references: {string.Join(", ", itemsWithNoRefs)}");

				foreach (var referenceMapEntry in referenceMap) {
					var entity = referenceMapEntry.Value;
					outputFile.WriteLine(reportFileSeparator);
					outputFile.WriteLine($"Name: {entity.Name}");
					outputFile.WriteLine($"Sql Type: {entity.Type}");

					outputFile.WriteLine($"Files referencing: {string.Join(", ", entity.FilesReferencing)}");
					outputFile.WriteLine($"Stored Procs referencing: {string.Join(", ", entity.ProcsReferencing)}");
				}
			}

			return pathToFile;
		}
	}
}
