using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.Excel
{
	// Token: 0x0200002A RID: 42
	[CompilerGenerated]
	[DefaultMember("_Default")]
	[Guid("000208D5-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface _Application
	{
		// Token: 0x060000C4 RID: 196
		void _VtblGap1_10();

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000C5 RID: 197
		[DispId(759)]
		Window ActiveWindow { [DispId(759)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.Interface)] get; }

		// Token: 0x060000C6 RID: 198
		void _VtblGap2_34();

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000C7 RID: 199
		[DispId(572)]
		Workbooks Workbooks { [DispId(572)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.Interface)] get; }

		// Token: 0x060000C8 RID: 200
		void _VtblGap3_60();

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000C9 RID: 201
		[DispId(0)]
		[IndexerName("_Default")]
		string _Default { [DispId(0)] [MethodImpl(MethodImplOptions.InternalCall)] [return: MarshalAs(UnmanagedType.BStr)] get; }

		// Token: 0x060000CA RID: 202
		void _VtblGap4_168();

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000CB RID: 203
		// (set) Token: 0x060000CC RID: 204
		[DispId(558)]
		bool Visible { [DispId(558)] [LCIDConversion(0)] [MethodImpl(MethodImplOptions.InternalCall)] get; [LCIDConversion(0)] [DispId(558)] [MethodImpl(MethodImplOptions.InternalCall)] [param: In] set; }
	}
}
