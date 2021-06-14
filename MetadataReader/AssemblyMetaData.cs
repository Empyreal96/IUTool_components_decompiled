using System;
using System.Globalization;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200001A RID: 26
	internal struct AssemblyMetaData : IDisposable
	{
		// Token: 0x06000109 RID: 265 RVA: 0x0000312C File Offset: 0x0000132C
		public void Init()
		{
			this.szLocale = new UnmanagedStringMemoryHandle();
			this.cbLocale = 0U;
			this.ulProcessor = 0U;
			this.ulOS = 0U;
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00003150 File Offset: 0x00001350
		public Version Version
		{
			get
			{
				return new Version((int)this.majorVersion, (int)this.minorVersion, (int)this.buildNumber, (int)this.revisionNumber);
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00003180 File Offset: 0x00001380
		public string LocaleString
		{
			get
			{
				bool flag = this.szLocale == null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool isInvalid = this.szLocale.IsInvalid;
					if (isInvalid)
					{
						result = string.Empty;
					}
					else
					{
						bool flag2 = this.cbLocale <= 0U;
						if (flag2)
						{
							result = string.Empty;
						}
						else
						{
							int num = (int)this.cbLocale;
							int countCharsNoNull = num - 1;
							string asString = this.szLocale.GetAsString(countCharsNoNull);
							result = asString;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600010C RID: 268 RVA: 0x000031F4 File Offset: 0x000013F4
		public CultureInfo Locale
		{
			get
			{
				bool flag = this.szLocale == null;
				CultureInfo result;
				if (flag)
				{
					result = CultureInfo.InvariantCulture;
				}
				else
				{
					result = new CultureInfo(this.LocaleString);
				}
				return result;
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00003228 File Offset: 0x00001428
		public void Dispose()
		{
			bool flag = this.szLocale != null;
			if (flag)
			{
				this.szLocale.Dispose();
			}
		}

		// Token: 0x04000045 RID: 69
		public ushort majorVersion;

		// Token: 0x04000046 RID: 70
		public ushort minorVersion;

		// Token: 0x04000047 RID: 71
		public ushort buildNumber;

		// Token: 0x04000048 RID: 72
		public ushort revisionNumber;

		// Token: 0x04000049 RID: 73
		public UnmanagedStringMemoryHandle szLocale;

		// Token: 0x0400004A RID: 74
		public uint cbLocale;

		// Token: 0x0400004B RID: 75
		public UnusedIntPtr rdwProcessor;

		// Token: 0x0400004C RID: 76
		public uint ulProcessor;

		// Token: 0x0400004D RID: 77
		public UnusedIntPtr rOS;

		// Token: 0x0400004E RID: 78
		public uint ulOS;
	}
}
