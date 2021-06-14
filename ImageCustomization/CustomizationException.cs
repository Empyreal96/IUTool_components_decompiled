using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000008 RID: 8
	public class CustomizationException : Exception
	{
		// Token: 0x06000078 RID: 120 RVA: 0x0000575D File Offset: 0x0000395D
		public CustomizationException(string message) : base(message)
		{
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005766 File Offset: 0x00003966
		public CustomizationException(string message, params object[] args) : this(string.Format(message, args))
		{
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005775 File Offset: 0x00003975
		public CustomizationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
