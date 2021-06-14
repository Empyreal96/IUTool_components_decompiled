using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.Excel
{
	// Token: 0x0200002C RID: 44
	[CompilerGenerated]
	[Guid("000208D8-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface _Worksheet
	{
		// Token: 0x060000D1 RID: 209
		void _VtblGap1_45();

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000D2 RID: 210
		[DispId(238)]
		Range Cells { [DispId(238)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.Interface)] get; }
	}
}
