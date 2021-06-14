using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.Excel
{
	// Token: 0x02000022 RID: 34
	[CompilerGenerated]
	[Guid("000208D7-0000-0000-C000-000000000046")]
	[DefaultMember("_Default")]
	[TypeIdentifier]
	[ComImport]
	public interface Sheets : IEnumerable
	{
		// Token: 0x060000BE RID: 190
		void _VtblGap1_8();

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000BF RID: 191
		[DispId(170)]
		object Item { [DispId(170)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.IDispatch)] get; }
	}
}
