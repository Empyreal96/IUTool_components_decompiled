using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000A RID: 10
	public class NativeMethods
	{
		// Token: 0x06000003 RID: 3
		[DllImport("Keyform.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int GetKeyForm(string Arch, string Name, string Version, string Pkt, string VersionScope, string Culture, string Type, StringBuilder Output, int OutputBytes);
	}
}
