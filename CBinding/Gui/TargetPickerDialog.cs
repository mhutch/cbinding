using System;
using System.Collections.Generic;
using System.Linq;

using MonoDevelop.Ide;

using Gtk;
namespace CBinding
{
	public partial class TargetPickerDialog : Dialog
	{
		TreeStore store = new TreeStore (typeof (bool), typeof (string));
		readonly Dictionary<string, CMakeTarget> targets = new Dictionary<string, CMakeTarget> ();
		CMakeFileFormat parent;

		public List<CMakeTarget> SelectedTargets { get; set; }

		public TargetPickerDialog (string title, CMakeFileFormat parent)
		{
			this.Build ();

			this.parent = parent;
			Title = title;
			treeview1.Model = store;
			treeview1.HeadersVisible = false;

			var textColumn = new TreeViewColumn ();
			textColumn.Title = "Targets";

			var toggle = new CellRendererToggle ();
			toggle.Toggled += ToggleEventHandler;
			textColumn.PackStart (toggle, false);
			textColumn.AddAttribute (toggle, "active", 0);


			var boolColumn = new TreeViewColumn ();
			textColumn.Title = "Targets";

			var text = new CellRendererText ();
			textColumn.PackStart (text, false);
			textColumn.AddAttribute (text, "text", 1);

			treeview1.AppendColumn (textColumn);

			PopulateTargets ();

			buttonOk.Clicked += ButtonOkClicked;
			select.Clicked += SelectClicked;
			deselect.Clicked += DeselectClicked;
			newTarget.Clicked += NewTargetClicked;
		}

		void PopulateTargets ()
		{
			targets.Clear ();
			store.Clear ();

			foreach (var target in parent.Targets.Values.ToList ()) {
				if (target.IsAliasOrImported) continue;

				store.AppendValues (false, target.ToString ());
				targets.Add (target.ToString (), target);
			}
			treeview1.ExpandAll ();
		}

		void ToggleEventHandler (object o, ToggledArgs args)
		{
			TreeIter itr;

			if (store.GetIterFromString (out itr, args.Path)) {
				bool isToggled = !(bool)store.GetValue (itr, 0);
				store.SetValue (itr, 0, isToggled);
			}
		}

		void ToggleAll (bool isToggled)
		{
			TreeIter itr;
			if (!store.GetIterFirst (out itr)) {
				return;
			}

			do {
				store.SetValue (itr, 0, isToggled);
			} while (store.IterNext (ref itr));
		}

		void ButtonOkClicked (object sender, EventArgs e)
		{
			SelectedTargets = new List<CMakeTarget> ();

			TreeIter itr;
			if (!store.GetIterFirst (out itr)) {
				return;
			}

			do {
				var str = (string)store.GetValue (itr, 1);
				if ((bool)store.GetValue (itr, 0))
					SelectedTargets.Add (targets [str]);
			} while (store.IterNext (ref itr));
		}

		void SelectClicked (object sender, EventArgs e)
		{
			ToggleAll (true);
		}

		void DeselectClicked (object sender, EventArgs e)
		{
			ToggleAll (false);
		}

		void NewTargetClicked (object sender, EventArgs e)
		{
			using (var dlg = new NewTargetDialog ()) {
				if (MessageService.ShowCustomDialog (dlg) != (int)ResponseType.Ok)
					return;

				if (string.IsNullOrEmpty (dlg.TargetName)) {
					MessageService.ShowError ("You can't leave the name empty.");
					return;
				}

				if (!parent.VariableManager.IsValidUnquotedElement (dlg.TargetName)) {
					MessageService.ShowError (@"Target name cannot contain whitespaces or '()#""\' characters.");
					return;
				}

				parent.NewTarget (dlg.TargetName, dlg.TargetType);

				PopulateTargets ();
			}
		}
	}
}

