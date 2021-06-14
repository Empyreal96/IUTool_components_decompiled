using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.Excel
{
	// Token: 0x02000027 RID: 39
	[CompilerGenerated]
	[Guid("000208DB-0000-0000-C000-000000000046")]
	[DefaultMember("_Default")]
	[TypeIdentifier]
	[ComImport]
	public interface Workbooks : IEnumerable
	{
		// Token: 0x060000C2 RID: 194
		void _VtblGap1_12();

		// Token: 0x060000C3 RID: 195
		[LCIDConversion(15)]
		[DispId(1923)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		Workbook Open([MarshalAs(UnmanagedType.BStr)] [In] string Filename, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object UpdateLinks, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object ReadOnly, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Format, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Password, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object WriteResPassword, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object IgnoreReadOnlyRecommended, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Origin, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Delimiter, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Editable, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Notify, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Converter, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object AddToMru, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Local, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object CorruptLoad);
	}
}
