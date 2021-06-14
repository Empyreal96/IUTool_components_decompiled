using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Adds;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000043 RID: 67
	[DebuggerDisplay("{TypeName},{m_AssemblyName}")]
	internal class UnresolvedTypeName
	{
		// Token: 0x0600048E RID: 1166 RVA: 0x0000F297 File Offset: 0x0000D497
		public UnresolvedTypeName(string typeName, AssemblyName assemblyName)
		{
			this.TypeName = typeName;
			this._assemblyName = assemblyName;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0000F2B0 File Offset: 0x0000D4B0
		public Type ConvertToType(ITypeUniverse universe, Module moduleContext)
		{
			string input = string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
			{
				this.TypeName,
				this._assemblyName.FullName
			});
			return TypeNameParser.ParseTypeName(universe, moduleContext, input);
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000490 RID: 1168 RVA: 0x0000F2F9 File Offset: 0x0000D4F9
		public string TypeName { get; }

		// Token: 0x040000E6 RID: 230
		private readonly AssemblyName _assemblyName;
	}
}
