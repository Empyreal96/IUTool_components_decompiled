using System;
using System.Reflection.Adds;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001C RID: 28
	[Guid("D8F579AB-402D-4b8e-82D9-5D63B1065C68")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataTables
	{
		// Token: 0x0600011B RID: 283
		void GetStringHeapSize(out uint countBytesStrings);

		// Token: 0x0600011C RID: 284
		void GetBlobHeapSize(out uint countBytesBlobs);

		// Token: 0x0600011D RID: 285
		void GetGuidHeapSize(out uint countBytesGuids);

		// Token: 0x0600011E RID: 286
		void GetUserStringHeapSize(out uint countByteBlobs);

		// Token: 0x0600011F RID: 287
		void GetNumTables(out uint countTables);

		// Token: 0x06000120 RID: 288
		void GetTableIndex(uint token, out uint tableIndex);

		// Token: 0x06000121 RID: 289
		void GetTableInfo(MetadataTable tableIndex, out int countByteRows, out int countRows, out int countColumns, out int columnPrimaryKey, out UnusedIntPtr name);

		// Token: 0x06000122 RID: 290
		void GetColumnInfo_();

		// Token: 0x06000123 RID: 291
		void GetCodedTokenInfo_();

		// Token: 0x06000124 RID: 292
		void GetRow_();

		// Token: 0x06000125 RID: 293
		void GetColumn_();

		// Token: 0x06000126 RID: 294
		void GetString_();

		// Token: 0x06000127 RID: 295
		void GetBlob_();

		// Token: 0x06000128 RID: 296
		void GetGuid_();

		// Token: 0x06000129 RID: 297
		void GetUserString_();

		// Token: 0x0600012A RID: 298
		void GetNextString_();

		// Token: 0x0600012B RID: 299
		void GetNextBlob_();

		// Token: 0x0600012C RID: 300
		void GetNextGuid_();

		// Token: 0x0600012D RID: 301
		void GetNextUserString_();
	}
}
