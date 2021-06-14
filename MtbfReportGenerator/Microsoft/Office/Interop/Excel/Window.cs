using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.Excel
{
	// Token: 0x02000023 RID: 35
	[CompilerGenerated]
	[InterfaceType(2)]
	[Guid("00020893-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface Window
	{
		// Token: 0x060000C0 RID: 192
		void _VtblGap1_12();

		// Token: 0x060000C1 RID: 193
		[DispId(277)]
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		bool Close([MarshalAs(UnmanagedType.Struct)] [In] [Optional] object SaveChanges, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Filename, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object RouteWorkbook);
	}
}
