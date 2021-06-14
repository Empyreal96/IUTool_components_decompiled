using System;
using System.Collections.ObjectModel;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x0200003A RID: 58
	public struct PkgGenConstants
	{
		// Token: 0x0400005B RID: 91
		public const string XmlElementUniqueXPath = "@Source";

		// Token: 0x0400005C RID: 92
		public const string XmlSchemaPath = "Microsoft.WindowsPhone.ImageUpdate.InboxApp.InboxApp.Resources.Schema.xsd";

		// Token: 0x0400005D RID: 93
		public const string AttrSource = "Source";

		// Token: 0x0400005E RID: 94
		public const string AttrLicense = "License";

		// Token: 0x0400005F RID: 95
		public const string AttrProvXML = "ProvXML";

		// Token: 0x04000060 RID: 96
		public const string AttrInfuseIntoDataPartition = "InfuseIntoDataPartition";

		// Token: 0x04000061 RID: 97
		public static readonly ReadOnlyCollection<string> ValidAttrInfuseIntoDataPartitionValues = new ReadOnlyCollection<string>(new string[]
		{
			"true",
			"false"
		});

		// Token: 0x04000062 RID: 98
		public const string AttrUpdate = "Update";

		// Token: 0x04000063 RID: 99
		public static readonly ReadOnlyCollection<string> ValidAttrUpdateValues = new ReadOnlyCollection<string>(new string[]
		{
			"early",
			"normal"
		});

		// Token: 0x04000064 RID: 100
		public const string VariablePROVXMLTYPE = "PROVXMLTYPE";

		// Token: 0x04000065 RID: 101
		public static readonly ReadOnlyCollection<string> ValidVariablePROVXMLTYPEValues = new ReadOnlyCollection<string>(new string[]
		{
			"Microsoft",
			"OEM",
			"Test"
		});
	}
}
