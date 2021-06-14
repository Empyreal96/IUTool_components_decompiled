using System;

namespace System.Reflection.Adds
{
	// Token: 0x0200001A RID: 26
	internal interface IModule2
	{
		// Token: 0x060000AC RID: 172
		int RowCount(MetadataTable metadataTableIndex);

		// Token: 0x060000AD RID: 173
		AssemblyName GetAssemblyNameFromAssemblyRef(Token token);
	}
}
