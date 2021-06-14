using System;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000009 RID: 9
	public class CustomizationXmlException : Exception
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00003968 File Offset: 0x00001B68
		public CustomizationXmlException()
		{
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003970 File Offset: 0x00001B70
		public CustomizationXmlException(string message) : base(message)
		{
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003979 File Offset: 0x00001B79
		public CustomizationXmlException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
