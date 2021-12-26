using System.Collections.Concurrent;

namespace db_ref_finder {
	public static class DirectoryDirector {
		// TODO: Implement ignoring/search method logic
		public static void Scour(string directoryPath, DbRefArgs dbRefArgs, ConcurrentDictionary<string, ReferenceList> referenceMap) {
			var directoryFiles = Directory.GetFiles(directoryPath);
			Console.WriteLine($"Processing: {directoryPath} ({directoryFiles.Length} file(s))");

			Parallel.ForEach(directoryFiles, filePath => {
				ScourFile(filePath, referenceMap);
			});

			Parallel.ForEach(Directory.GetDirectories(directoryPath), directory => {
				Scour(directory, dbRefArgs, referenceMap);
			});
		}

		private static void ScourFile(string filePath, ConcurrentDictionary<string, ReferenceList> referenceMap) {
			var line = string.Empty;
			var file = new StreamReader(filePath);

			while ((line = file.ReadLine()) != null) {
				var referencedEntities = referenceMap.Keys.Where(r => line.Contains(r));
				foreach (var entity in referencedEntities) {
					referenceMap[entity].FilesReferencing.Add(filePath);
				}
			}
			file.Close();
		}
	}
}
