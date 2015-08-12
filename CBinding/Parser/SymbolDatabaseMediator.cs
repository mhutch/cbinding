using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using CBinding.Parser;
using ClangSharp;
using Mono.Data.Sqlite;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace CBinding
{
	public class SymbolDatabaseMediator : IDisposable
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
			ContainingProject.FileChangedInProject += HandleSave;
			ContainingProject.FileRenamedInProject += HandleRename;
			ContainingProject.Disposing += HandleDispose;
		}



		void Initialize ()
		{
			lock (Lock) {
				if (!Initialized) {
					InitializeConnection ();
					InitializeTracking ();
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
			Connection.Open ();
		}

		void InitializeTracking ()
		{
			using (var cmd = Connection.CreateCommand ()) {
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS TRACKING (NAME TEXT, LASTWRITETIME TEXT);";
				cmd.CommandType = CommandType.Text;
				cmd.ExecuteNonQuery ();
			}
		}

		static string TableName (string name)
		{
			return "T_" + (uint)name.GetHashCode ();
		}

		void HandleAddition (object sender, ProjectFileEventArgs args)
		{
			lock (Lock) {
				Initialize ();
				foreach (var e in args) {
					CreateTableForFileIfNotExists (e.ProjectFile.Name);
					PrepareInsert (e.ProjectFile.Name);
					PrepareTracking (e.ProjectFile.Name);
				}
			}
		}

		void HandleRemoval (object sender, ProjectFileEventArgs args)
		{
			lock(Lock) {
				foreach (var e in args) {
					using (var cmd = Connection.CreateCommand ()) {
						cmd.CommandText = "DROP TABLE " + TableName (e.ProjectFile.Name) + ";";
						cmd.CommandType = CommandType.Text;
						cmd.ExecuteNonQuery ();
					}
					InsertCommands.Remove (e.ProjectFile.Name);
				}
			}
		}

		void HandleSave (object sender, ProjectFileEventArgs args)
		{
			foreach (var e in args){
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "UPDATE TRACKING SET LASTWRITETIME = @LWT WHERE NAME = @N;";
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue ("@N", TableName (e.ProjectFile.Name));
					cmd.Parameters.AddWithValue ("@LWT", new FileInfo (e.ProjectFile.Name).LastWriteTime.ToString ());
					cmd.ExecuteNonQuery ();
				}
			}
		}
			
		void HandleRename (object sender, ProjectFileRenamedEventArgs args)
		{
			foreach (var e in args){
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "UPDATE TRACKING SET NAME = @NEWNAME WHERE NAME = @NAME;";
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue ("@NAME", TableName (e.OldName));
					cmd.Parameters.AddWithValue ("@NEWNAME", TableName (e.NewName));
					cmd.ExecuteNonQuery ();
				}
			}
		}

		void PrepareInsert (string fileName)
		{
			lock (Lock) {
				SqliteCommand insert = Connection.CreateCommand ();
				insert.CommandText = "INSERT INTO " + TableName(fileName) + " (OFFSET, USR, ISDEF) VALUES (@OFFSET, @USR, @ISDEF);";
				insert.CommandType = CommandType.Text;
				insert.Prepare ();
				InsertCommands.Add (fileName, insert);
			}
		}

		void TrackingUpdate (string fileName)
		{
			using (var cmd = Connection.CreateCommand ()) {
				cmd.CommandText = "DELETE FROM TRACKING WHERE NAME=@N;";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue ("@N", TableName (fileName));
				cmd.ExecuteNonQuery ();
			}
			using (var cmd = Connection.CreateCommand ()) {
				cmd.CommandText = "INSERT INTO TRACKING (NAME, LASTWRITETIME) VALUES (@N, @LWT);";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue ("@N", TableName (fileName));
				cmd.Parameters.AddWithValue ("@LWT", new FileInfo (fileName).LastWriteTime.ToString ());
				cmd.ExecuteNonQuery ();
			}
		}

		void PrepareTracking (string fileName)
		{
			lock (Lock) {
				CreateTableForTrackingIfNotExist ();
				TrackingUpdate (fileName);
			}
		}

		void CreateTableForTrackingIfNotExist ()
		{
			using (var cmd = Connection.CreateCommand ()) {
				cmd.CommandText = "CREATE TABLE IF NOT EXISTS TRACKING (NAME TEXT, LASTWRITETIME TEXT);";
				cmd.CommandType = CommandType.Text;
				cmd.ExecuteNonQuery ();
			}
		}

		void CreateTableForFileIfNotExists (string fileName)
		{
			lock (Lock) {
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TableName (fileName) + " (OFFSET INTEGER, USR TEXT, ISDEF INTEGER);";
					cmd.CommandType = CommandType.Text;
					cmd.ExecuteNonQuery ();
				}
			}
		}

		void InsertCursor (string fileName, CXCursor cursor, CancellationToken cancel)
		{
			var sym = new Symbol (ContainingProject, cursor);
			var insert = InsertCommands [fileName];
			insert.Parameters.AddWithValue ("@OFFSET", sym.Begin.Offset);
			insert.Parameters.AddWithValue ("@USR", sym.Usr);
			insert.Parameters.AddWithValue ("@ISDEF", sym.Def ? 1 : 0);
			insert.ExecuteNonQueryAsync (cancel);
			insert.Parameters.Clear ();
		}

		public void UpdateDatabase (string fileName, CXTranslationUnit TU, CancellationToken cancellationToken, bool force = false)
		{
			if (!cancellationToken.IsCancellationRequested) {
				if (!force) {
					using (var cmd = Connection.CreateCommand ()) {
						cmd.CommandText = "SELECT LASTWRITETIME FROM TRACKING WHERE NAME=@N;";
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue ("@N", TableName (fileName));
						using (var result = cmd.ExecuteReader ()) {
							result.Read ();
							string lwt = result.GetString (0);
							if (lwt.Equals (new FileInfo (fileName).LastWriteTime.ToString ()))
								return;
						}
					}
				}
				Reset (fileName);
				CXCursor TUcursor = clang.getTranslationUnitCursor (TU);
				var parser = new TranslationUnitParser (this, fileName, cancellationToken, TUcursor);
				using (var tr = Connection.BeginTransaction ()) {
					clang.visitChildren (TUcursor, parser.Visit, new CXClientData (new IntPtr (0)));
					tr.Commit ();
				}
			}
		}

		public void AddToDatabase (string fileName, CXCursor cursor, CancellationToken cancel)
		{
			lock (Lock) {
				if (!cancel.IsCancellationRequested) {
					InsertCursor (fileName, cursor, cancel);
				}
			}
		}

		public void Reset (string fileName)
		{
			lock (Lock) {
				using (var cmd = Connection.CreateCommand ()) {
					cmd.CommandText = "DELETE FROM " + TableName(fileName) + ";";
					cmd.CommandType = CommandType.Text;
					cmd.ExecuteNonQuery ();
				}
			}
		}

		public IEnumerable<SourceLocation> getDeclarations (CXCursor cursor)
		{
			foreach (var file in ContainingProject.Files) {
				using (var selectDeclarations = Connection.CreateCommand ()) {
					selectDeclarations.CommandText = "SELECT OFFSET FROM " + TableName (file.Name) + " WHERE USR=@USR;";
					selectDeclarations.CommandType = CommandType.Text;
					selectDeclarations.Parameters.AddWithValue ("@USR", Manager.GetCursorUsrString (cursor));
					using(var result = selectDeclarations.ExecuteReader ()) {
						while (result.Read ()) {
							yield return new SourceLocation (file.Name, 0, 0, (uint)result.GetInt32 (0));
						}
					}
				}
			}
		}

		public IEnumerable<SourceLocation> GetDefinitionLocation (CXCursor cursor)
		{
			foreach (var file in ContainingProject.Files) {
				using (var selectDeclarations = Connection.CreateCommand ()) {
					selectDeclarations.CommandText = "SELECT OFFSET FROM " + TableName (file.Name) + " WHERE USR=@USR AND ISDEF=1;";
					selectDeclarations.CommandType = CommandType.Text;
					selectDeclarations.Parameters.AddWithValue ("@USR", Manager.GetCursorUsrString (cursor));
					using (var result = selectDeclarations.ExecuteReader ()) {
						while (result.Read ()) {
							yield return new SourceLocation (file.Name, 0, 0, (uint)result.GetInt32 (0));
						}
					}
				}
			}		
		}

		void HandleDispose (object sender, EventArgs e)
		{
			OnDispose (true);
		}

		protected virtual void OnDispose(bool disposing)
		{
			Connection.Close ();
			ContainingProject.FileAddedToProject -= HandleAddition;
			ContainingProject.FileRemovedFromProject -= HandleRemoval;
			ContainingProject.FileChangedInProject -= HandleSave;
			ContainingProject.FileRenamedInProject -= HandleRename;
			ContainingProject.Disposing -= HandleDispose;
		}

		~SymbolDatabaseMediator()
		{
			OnDispose(false);
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			OnDispose(true); 
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
