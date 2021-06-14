using System;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000012 RID: 18
	internal class XmlFile
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000060 RID: 96 RVA: 0x0000464C File Offset: 0x0000284C
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00004654 File Offset: 0x00002854
		public string Filename
		{
			get
			{
				return this.filename;
			}
			set
			{
				this.filename = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000465D File Offset: 0x0000285D
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00004665 File Offset: 0x00002865
		public string Schema
		{
			get
			{
				return this.schema;
			}
			set
			{
				this.schema = value;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000466E File Offset: 0x0000286E
		public XmlFile(string file, string sch)
		{
			this.filename = file;
			this.schema = sch;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004684 File Offset: 0x00002884
		public bool Validate()
		{
			return XmlFileHandler.ValidateSchema(XElement.Load(this.Filename), this.Schema);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000469C File Offset: 0x0000289C
		public void ExpandFilePath()
		{
			try
			{
				this.filename = Environment.ExpandEnvironmentVariables(this.filename);
				if (this.schema == Settings.CustomizationSchema)
				{
					string text = Path.GetDirectoryName(this.filename);
					string fileName = Path.GetFileName(this.filename);
					if (string.IsNullOrEmpty(text))
					{
						text = Settings.CustomizationIncludeDirectory;
					}
					this.filename = Path.Combine(text, fileName);
				}
			}
			catch
			{
				TraceLogger.LogMessage(TraceLevel.Info, string.Format("Couldn't get directory/filename. File='{0}', schema='{1}'. Ignoring.", this.filename, this.schema), true);
			}
		}

		// Token: 0x0400004F RID: 79
		private string filename;

		// Token: 0x04000050 RID: 80
		private string schema;
	}
}
