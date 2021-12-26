namespace db_ref_finder {
	public class DbRefArgs {
		public string DirectoryToSearch { get; set; }
		public IEnumerable<string> PatternsToIgnore { get; set; }
		public MatchingTechnique MatchingTechnique { get; set; }

		public DbRefArgs(string directoryToSearch, IEnumerable<string> patternsToIgnore, MatchingTechnique matchingTechnique) {
			DirectoryToSearch = directoryToSearch;
			PatternsToIgnore = patternsToIgnore;
			MatchingTechnique = matchingTechnique;
		}
	}
}
