using System;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200003D RID: 61
	public sealed class FileBuilder : FileBuilder<PkgFile, FileBuilder>
	{
		// Token: 0x060000ED RID: 237 RVA: 0x0000561B File Offset: 0x0000381B
		internal FileBuilder(XElement element) : base(element)
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00005624 File Offset: 0x00003824
		internal FileBuilder(string source, string destinationDir) : base(source, destinationDir)
		{
		}
	}
}
