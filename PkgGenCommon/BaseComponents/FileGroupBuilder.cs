using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200003C RID: 60
	public sealed class FileGroupBuilder : FilterGroupBuilder<FileGroup, FileGroupBuilder>
	{
		// Token: 0x060000E7 RID: 231 RVA: 0x0000556D File Offset: 0x0000376D
		public FileGroupBuilder()
		{
			this.files = new List<FileBuilder>();
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00005580 File Offset: 0x00003780
		public FileBuilder AddFile(XElement fileElement)
		{
			FileBuilder fileBuilder = new FileBuilder(fileElement);
			this.files.Add(fileBuilder);
			return fileBuilder;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000055A4 File Offset: 0x000037A4
		public FileBuilder AddFile(string source, string destinationDir)
		{
			FileBuilder fileBuilder = new FileBuilder(source, destinationDir);
			this.files.Add(fileBuilder);
			return fileBuilder;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000055C6 File Offset: 0x000037C6
		public FileBuilder AddFile(string source)
		{
			return this.AddFile(source, "$(runtime.default)");
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000055D4 File Offset: 0x000037D4
		public override FileGroup ToPkgObject()
		{
			this.filterGroup.Files.Clear();
			this.files.ForEach(delegate(FileBuilder x)
			{
				this.filterGroup.Files.Add(x.ToPkgObject());
			});
			return base.ToPkgObject();
		}

		// Token: 0x040000E9 RID: 233
		private List<FileBuilder> files;
	}
}
