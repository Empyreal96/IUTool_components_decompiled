using System;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000008 RID: 8
	public class ConfigXmlException : Exception
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00003968 File Offset: 0x00001B68
		public ConfigXmlException()
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003970 File Offset: 0x00001B70
		public ConfigXmlException(string message) : base(message)
		{
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003979 File Offset: 0x00001B79
		public ConfigXmlException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
