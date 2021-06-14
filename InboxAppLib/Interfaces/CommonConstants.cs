using System;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000039 RID: 57
	public struct CommonConstants
	{
		// Token: 0x0400004F RID: 79
		public const string AttributeValueProductID = "PRODUCTID";

		// Token: 0x04000050 RID: 80
		public const string AttributeValueName = "NAME";

		// Token: 0x04000051 RID: 81
		public const string AttributeValueID = "ID";

		// Token: 0x04000052 RID: 82
		public const string AttributeValueTitle = "TITLE";

		// Token: 0x04000053 RID: 83
		public const string AttributeValueVersion = "VERSION";

		// Token: 0x04000054 RID: 84
		public const string AttributeValuePublisher = "PUBLISHER";

		// Token: 0x04000055 RID: 85
		public const string AttributeValueDescription = "DESCRIPTION";

		// Token: 0x04000056 RID: 86
		public const string PkgGenMacroHeader = "$(";

		// Token: 0x04000057 RID: 87
		public const string PkgGenMacroFooter = ")";

		// Token: 0x04000058 RID: 88
		public const string InRomUpdateMicrosoftProvXMLDestinationPathFormat = "$(runtime.updateProvxmlMS)\\mxipupdate{0}";

		// Token: 0x04000059 RID: 89
		public const string InRomUpdateOEMProvXMLDestinationPathFormat = "$(runtime.updateProvxmlOEM)\\mxipupdate{0}";

		// Token: 0x0400005A RID: 90
		public const string InRomUpdateAppFrameworkProvXMLDestinationPathFormat = "$(runtime.updateProvxmlMS)\\appframework{0}";
	}
}
