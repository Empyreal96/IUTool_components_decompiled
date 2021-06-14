using System;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000009 RID: 9
	internal class CustomizationFile
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007B RID: 123 RVA: 0x0000577F File Offset: 0x0000397F
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00005787 File Offset: 0x00003987
		public FileType FileType { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00005790 File Offset: 0x00003990
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00005798 File Offset: 0x00003998
		public string Source { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000057A1 File Offset: 0x000039A1
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000057A9 File Offset: 0x000039A9
		public string Destination { get; private set; }

		// Token: 0x06000081 RID: 129 RVA: 0x000057B2 File Offset: 0x000039B2
		public CustomizationFile(string source, string destination) : this(FileType.Regular, source, destination)
		{
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000057BD File Offset: 0x000039BD
		public CustomizationFile(FileType type, string source, string destination)
		{
			this.FileType = type;
			this.Source = source;
			this.Destination = CustomizationFile.RemoveDriveLetter(destination);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000057E0 File Offset: 0x000039E0
		private static string RemoveDriveLetter(string path)
		{
			string pathRoot = Path.GetPathRoot(path);
			return "\\" + path.Substring(pathRoot.Length);
		}
	}
}
