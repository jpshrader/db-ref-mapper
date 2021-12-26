namespace db_ref_finder {
	public class ReferenceList {
		public string Name { get; set; }

		public SqlEntityType Type { get; set; }

		public HashSet<string> FilesReferencing { get; set; }

		public HashSet<string> ProcsReferencing { get; set; }

		public ReferenceList(string name, SqlEntityType type) {
			Name = name;
			Type = type;
			FilesReferencing = new HashSet<string>();
			ProcsReferencing = new HashSet<string>();
		}
	}
}
