namespace CBinding.Refactoring
{/*
	//Based on code from CSharpBinding
	class FindDerivedSymbolsHandler
	{
		public static bool CanFindDerivedSymbols (ISymbol symbol, out string description)
		{
			if (symbol.Kind == SymbolKind.NamedType) {
				var type = (ITypeSymbol)symbol;
				description = type.TypeKind == TypeKind.Interface ? GettextCatalog.GetString ("Find Implementing Types") : GettextCatalog.GetString ("Find Derived Types");
				return !type.IsStatic && !type.IsSealed;
			}
			if (symbol.ContainingType != null && symbol.ContainingType.TypeKind == TypeKind.Interface) {
				description = GettextCatalog.GetString ("Find Implementing Symbols");
			} else {
				description = GettextCatalog.GetString ("Find Derived Symbols");
			}
			return symbol.IsVirtual || symbol.IsAbstract || symbol.IsOverride;
		}

		public static void FindDerivedSymbols (ISymbol symbol)
		{
			Task.Run (delegate {
				using (var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true)) {
					IEnumerable<ISymbol> task;

					if (symbol.ContainingType != null && symbol.ContainingType.TypeKind == TypeKind.Interface) {
						task = SymbolFinder.FindImplementationsAsync (symbol, TypeSystemService.Workspace.CurrentSolution).Result; 
					} else if (symbol.Kind == SymbolKind.NamedType) {
						var type = (INamedTypeSymbol)symbol;
						if (type.TypeKind == TypeKind.Interface) {
							task = SymbolFinder.FindImplementationsAsync (symbol, TypeSystemService.Workspace.CurrentSolution).Result; 
						} else {
							task = type.FindDerivedClassesAsync (TypeSystemService.Workspace.CurrentSolution).Result.Cast<ISymbol> ();
						}
					} else {
						task = SymbolFinder.FindOverridesAsync (symbol, TypeSystemService.Workspace.CurrentSolution).Result;
					}
					foreach (var foundSymbol in task) {
						foreach (var loc in foundSymbol.Locations)
							monitor.ReportResult (new MemberReference (foundSymbol, loc.SourceTree.FilePath, loc.SourceSpan.Start, loc.SourceSpan.Length));
					}
				}
			});
		}

		public async void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null || doc.ParsedDocument == null) {
				info.Enabled = false;
				return;
			}
			var rinfo = await RefactoringSymbolInfo.GetSymbolInfoAsync (doc, doc.Editor.CaretOffset);
			info.Enabled = rinfo.DeclaredSymbol != null;
		}

		public async void Run (object data)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null)
				return;
			var info = await RefactoringSymbolInfo.GetSymbolInfoAsync (doc, doc.Editor.CaretOffset);
			if (info.DeclaredSymbol != null)
				FindDerivedSymbols (info.DeclaredSymbol);
		}
	}*/
}
