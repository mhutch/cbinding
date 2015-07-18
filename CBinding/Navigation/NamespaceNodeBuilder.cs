//
// NamespaceNodeBuilder.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using CBinding.Parser;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;

namespace CBinding.Navigation
{
	public class NamespaceNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Namespace); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof (SymbolCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			if (!thisNode.Options ["NestedNamespaces"] && ((Namespace)dataObject).Parent != null)
				return ((Namespace)dataObject).ParentCursor.ToString () + "::" + ((Namespace)dataObject).Name;
			return ((Namespace)dataObject).Name;
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder,
										object dataObject,
										NodeInfo nodeInfo)
		{
			if (!treeBuilder.Options["NestedNamespaces"] && ((Namespace)dataObject).Parent != null)
				nodeInfo.Label = ((Namespace)dataObject).ParentCursor.ToString () + "::" + ((Namespace)dataObject).Name;
			else
				nodeInfo.Label = ((Namespace)dataObject).Name;			
			nodeInfo.Icon = Context.GetIcon (Stock.NameSpace);
		}
		
		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			CProject p = (CProject)treeBuilder.GetParentDataItem (typeof(CProject), false);
			
			if (p == null) return;
			
			ClangProjectSymbolDatabase info = p.DB;
			
			Namespace thisNamespace = ((Namespace)dataObject);
			
			// Namespaces
			if (treeBuilder.Options["NestedNamespaces"])
				foreach (Namespace n in info.Namespaces.Values)
					if (n.Parent != null && n.Parent.Equals (thisNamespace))
						treeBuilder.AddChild (n);
			
			foreach(Symbol c in info.CanBeInNamespaces.Values)
				if (c.Ours && c.Parent != null && c.Parent.Equals (thisNamespace))
					treeBuilder.AddChild (c);

		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
		
		public override int CompareObjects (ITreeNavigator thisNode, ITreeNavigator otherNode)
		{
			return -1;
		}
	}
}
