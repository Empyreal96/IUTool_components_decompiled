using System;
using System.Diagnostics;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x0200000A RID: 10
	[DebuggerDisplay("Resolve for {Name}")]
	internal class ResolveAssemblyNameEventArgs : EventArgs
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00002FCC File Offset: 0x000011CC
		public ResolveAssemblyNameEventArgs(AssemblyName name)
		{
			this.Name = name;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002FDE File Offset: 0x000011DE
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00002FE6 File Offset: 0x000011E6
		public AssemblyName Name { get; internal set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002FEF File Offset: 0x000011EF
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00002FF7 File Offset: 0x000011F7
		public Assembly Target { get; set; }
	}
}
