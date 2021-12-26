using System.Data.SqlClient;

namespace db_ref_finder {
	public class DbAccess {
		private readonly string connectionString;

		public DbAccess(string connectionString) {
			this.connectionString = connectionString;
		}

		/// <summary>
		/// Gets a list of distinct names of the given sql entity type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable<string> GetDbEntities(SqlEntityType type) {
			var query = GetSqlObjectTypeQuery(type);

			return Get(query);
		}

		/// <summary>
		/// Gets all stored procs referencing a given db entity name (ex: table name, proc name, etc)
		/// </summary>
		/// <param name="name">Name of db entity you're checking</param>
		/// <returns>Names of stored procs referencing a given db entity</returns>
		public IEnumerable<string> FindRefsFromStoredProcs(string name) {
			var sqlQuery = $"SELECT DISTINCT(Name) FROM sys.procedures WHERE OBJECT_DEFINITION(OBJECT_ID) LIKE '%{name}%' AND [Name] != '{name}'";

			return Get(sqlQuery);
		}

		private IEnumerable<string> Get(string sqlQuery) {
			using var connection = new SqlConnection(connectionString);
			try {
				connection.Open();
				using var command = new SqlCommand(sqlQuery, connection);
				using var reader = command.ExecuteReader();
				while (reader.Read()) {
					yield return reader.GetString(0);
				}
			} finally {
				connection.Close();
			}
		}

		private static string GetSqlObjectTypeQuery(SqlEntityType type) {
			return type switch {
				SqlEntityType.Table => "SELECT DISTINCT([name]) FROM sys.tables",

				SqlEntityType.StoredProcedure => "SELECT DISTINCT([name]) FROM dbo.sysobjects WHERE type = 'P'",

				SqlEntityType.Function => "SELECT DISTINCT([name]) FROM sys.sql_modules m INNER JOIN sys.objects o ON m.object_id= o.object_id WHERE type_desc like '%function%'",

				SqlEntityType.View => "SELECT DISTINCT([name]) FROM sys.views",

				SqlEntityType.Type => "SELECT DISTINCT([name]) FROM sys.types WHERE is_user_defined = 1",

				_ => throw new KeyNotFoundException()
			};
		}
	}
}
