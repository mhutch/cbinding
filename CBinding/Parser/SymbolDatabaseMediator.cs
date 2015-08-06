using MonoDevelop.Projects;
using MonoDevelop.Core;
using Mono.Data.Sqlite;
using System.IO;
using System.Threading;
using ClangSharp;
using CBinding.Parser;
using System.Collections.Generic;
using System;

namespace CBinding
{
	public class SymbolDatabaseMediator
	{
		public object Lock { get; } = new object ();
		CProject ContainingProject { get; }
		CLangManager Manager { get; }
		public SqliteConnection Connection { get; private set; }
		Dictionary<string, SqliteCommand> InsertCommands { get; } = new Dictionary <string, SqliteCommand>();
		bool Initialized { get; set; } = false;

		public SymbolDatabaseMediator (CProject proj, CLangManager manager)
		{
			ContainingProject = proj;
			Manager = manager;
			ContainingProject.FileAddedToProject += HandleAddition;
			ContainingProject.FileRemovedFromProject += HandleRemoval;
		}

		void Initialize ()
		{
			lock (Lock) {
				if (!Initialized) {
					InitializeConnection ();
					Initialized = true;
				}
			}
		}

		void InitializeConnection ()
		{
			var path = ContainingProject.BaseDirectory;
			string db = Path.Combine (path, "symbols.db3");
			bool exists = File.Exists (db);
			if (!exists)
				SqliteConnection.CreateFile (db);
			Connection = new SqliteConnection ("Data Source=" + db);
		}

		void HandleAddition (object sender, ProjectFileEventArgs args)
		{
			lock (Lock) {
				Initialize ();
				foreach (var e in args) {
					Connection.Open ();
					CreateTableIfNotExists (e.ProjectFile.Name);
					PrepareInsert (e.ProjectFile.Name);
					Connection.Close ();
				}
			}
		}

		void HandleRemoval (object sender, ProjectFileEventArgs args)
		{
			lock(Lock) {
				Connection.Open ();
				foreach (var e in args) {
					using (var cmd = Connection.CreateCommand ()) {
						cmd.CommandText = "DROP TABLE T_" + (uint)e.ProjectFile.Name.GetHashCode () + ";";
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.ExecuteNonQuery ();
					}
					InsertCommands.Remove (e.ProjectFile.Name);
				}
				Connection.Close ();
			}
		}

		void PrepareInsert (string fileName)
		{
			lock(Lock) {
				SqliteCommand insert = Connection.CreateCommand ();
				insert.CommandText = "INSERT INTO T_" + (uint)fileName.GetHashCode () + " (LINE, COLUMN, LENGTH, USR, ISDEF, ISDECL) VALUES (@LINE, @COLUMN, @LENGTH, @USR, @ISDEF, @ISDECL);";
				insert.CommandType = System.Data.CommandType.Text;
				insert.Prepare ();
				InsertCommands.Add (fileName, insert);
			}
		}

		void CreateTableIfNotExists (string fileName)
		{
			lock (Lock) {
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "CREATE TABLE IF NOT EXISTS T_" + (uint)fileName.GetHashCode () + " (LINE INTEGER, COLUMN INTEGER, LENGTH INTEGER, USR TEXT, ISDEF INTEGER, ISDECL INTEGER);";
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.ExecuteNonQuery ();
				}
			}
		}

		void InsertCursor (string fileName, CXCursor cursor, CancellationToken cancel)
		{
			var sym = new Symbol (ContainingProject, cursor, fileName);
			if (sym.Ours) {
				var insert = InsertCommands [fileName];
				insert.Parameters.AddWithValue ("@LINE", sym.Begin.Line);
				insert.Parameters.AddWithValue ("@COLUMN", sym.Begin.Column);
				insert.Parameters.AddWithValue ("@LENGTH", sym.SpellingLength);
				insert.Parameters.AddWithValue ("@USR", sym.Usr);
				insert.Parameters.AddWithValue ("@ISDECL", sym.IsDeclaration);
				insert.Parameters.AddWithValue ("@ISDEF", sym.IsDefinition);
				insert.ExecuteNonQueryAsync (cancel);
				insert.Parameters.Clear ();
			}
		}

		public void AddToDatabase (string fileName, CXCursor cursor, CancellationToken cancel)
		{
			lock (Lock) {
				if (!cancel.IsCancellationRequested && clang.isDeclaration (cursor.kind) != 0) {
					InsertCursor (fileName, cursor, cancel);
				}
			}
		}

		public void Reset (string fileName)
		{
			lock (Lock) {
				Connection.Open ();
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "DELETE FROM T_" + (uint)fileName.GetHashCode () + ";";
					cmd.CommandType = System.Data.CommandType.Text;
					cmd.ExecuteNonQuery ();
				}
				Connection.Close ();
			}
		}

		public CXCursor getDefinition (CXCursor cursor)
		{
			//var cmd = Connection.CreateCommand ();
			//cmd.CommandText = "SELECT * FROM ";
			return cursor;
		}

		public CXCursor getDeclaration (CXCursor cursor)
		{
			return cursor;
		}
			
	}
}
