using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000021 RID: 33
	public class EditionLookup
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00007BFC File Offset: 0x00005DFC
		[XmlIgnore]
		public string InstallPath
		{
			get
			{
				string text = string.Empty;
				switch (this.Method)
				{
				case EditionLookup.LookupMethod.Registry:
					text = RegistryLookup.GetValue(this.Path, this.Key);
					break;
				case EditionLookup.LookupMethod.EnvironmentVariable:
					text = Environment.GetEnvironmentVariable(this.Key);
					break;
				case EditionLookup.LookupMethod.HardCodedPath:
					text = Environment.ExpandEnvironmentVariables(this.Path);
					break;
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					if (!string.IsNullOrWhiteSpace(this.RelativePath))
					{
						text = System.IO.Path.Combine(text, this.RelativePath);
					}
					text = Environment.ExpandEnvironmentVariables(text);
					if (!LongPathDirectory.Exists(text))
					{
						text = string.Empty;
					}
				}
				return text;
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00007C90 File Offset: 0x00005E90
		public override string ToString()
		{
			string text = this.Method.ToString() + " ";
			switch (this.Method)
			{
			case EditionLookup.LookupMethod.Registry:
			case EditionLookup.LookupMethod.EnvironmentVariable:
				text += this.Key;
				break;
			case EditionLookup.LookupMethod.HardCodedPath:
				text += this.Path;
				break;
			}
			return text;
		}

		// Token: 0x040000A4 RID: 164
		[XmlAttribute]
		public EditionLookup.LookupMethod Method;

		// Token: 0x040000A5 RID: 165
		[XmlAttribute]
		public string Path;

		// Token: 0x040000A6 RID: 166
		[XmlAttribute]
		public string Key;

		// Token: 0x040000A7 RID: 167
		[XmlAttribute]
		public string RelativePath;

		// Token: 0x040000A8 RID: 168
		[XmlAttribute]
		public string MSPackageDirectoryName;

		// Token: 0x0200003B RID: 59
		public enum LookupMethod
		{
			// Token: 0x04000131 RID: 305
			Registry,
			// Token: 0x04000132 RID: 306
			EnvironmentVariable,
			// Token: 0x04000133 RID: 307
			HardCodedPath
		}
	}
}
