using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200000A RID: 10
	[DebuggerDisplay("AssemblyRef: {m_name}")]
	internal class AssemblyRef : AssemblyProxy
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000286A File Offset: 0x00000A6A
		public AssemblyRef(AssemblyName name, ITypeUniverse universe) : base(universe)
		{
			this._name = name;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000287C File Offset: 0x00000A7C
		protected override Assembly GetResolvedAssemblyWorker()
		{
			return base.TypeUniverse.ResolveAssembly(this._name);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000028A0 File Offset: 0x00000AA0
		protected override AssemblyName GetNameWithNoResolution()
		{
			return this._name;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000028B8 File Offset: 0x00000AB8
		public override AssemblyName GetName()
		{
			return this._name;
		}

		// Token: 0x04000005 RID: 5
		private readonly AssemblyName _name;
	}
}
