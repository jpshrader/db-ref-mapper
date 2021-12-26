using System.Collections.Concurrent;

namespace db_ref_finder {
	public static class SqlProcInspector {
		public static void Inspect(DbAccess dbAccess, ConcurrentDictionary<string, ReferenceList> referenceMap) {
			Console.WriteLine("Processing Database References...");
			Parallel.ForEach(referenceMap.Keys, entity => {
				var referenceList = referenceMap[entity];
				var referencingProcs = dbAccess.FindRefsFromStoredProcs(entity);

				foreach (var referencingProc in referencingProcs) {
					referenceList.ProcsReferencing.Add(referencingProc);
				}
			});
		}
	}
}
