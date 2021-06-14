using System;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000043 RID: 67
	public struct ProvXMLConstants_APPX
	{
		// Token: 0x040000B2 RID: 178
		public const string AttributeValueAppxPath = "APPXPATH";

		// Token: 0x040000B3 RID: 179
		public const string AttributeValueAppxManifestPath = "APPXMANIFESTPATH";

		// Token: 0x040000B4 RID: 180
		public const string FrameworkSubDir = "Framework";

		// Token: 0x040000B5 RID: 181
		public const string AppxColdFrameworkPrefix = "mxipcold_appframework_";

		// Token: 0x040000B6 RID: 182
		public const string InRomProvXMLBaseDestinationPath = "$(runtime.commonfiles)\\Provisioning\\";

		// Token: 0x040000B7 RID: 183
		public const string InRomFrameworkProvXMLBaseDestinationPath = "$(runtime.coldBootProvxmlMS)\\";

		// Token: 0x040000B8 RID: 184
		public const string DataPartitionProvXMLBaseDestinationPath = "$(runtime.data)\\SharedData\\Provisioning\\";

		// Token: 0x040000B9 RID: 185
		public const string InRomLicenseBaseDestinationPath = "$(runtime.commonfiles)\\Xaps\\";

		// Token: 0x040000BA RID: 186
		public const string DataPartitionLicenseBaseDestinationPath = "$(runtime.data)\\SharedData\\Provisioning\\";

		// Token: 0x040000BB RID: 187
		public const string ProvXmlCharacteristic_AppxPackage = "AppxPackage";

		// Token: 0x040000BC RID: 188
		public const string ProvXmlCharacteristic_AppxInfused = "AppxInfused";

		// Token: 0x040000BD RID: 189
		public const string ProvXmlCharacteristic_FrameworkPackage = "FrameworkPackage";

		// Token: 0x040000BE RID: 190
		public const string AttributeNameAppxManifestPath = "AppXManifestPath";

		// Token: 0x040000BF RID: 191
		public const string AttributeValueProductID = "PRODUCTID";

		// Token: 0x040000C0 RID: 192
		public const string AttributeValueInstanceID = "INSTANCEID";

		// Token: 0x040000C1 RID: 193
		public const string AttributeValueLicensePath = "LICENSEPATH";

		// Token: 0x040000C2 RID: 194
		public const string AttributeValueOfferID = "OFFERID";

		// Token: 0x040000C3 RID: 195
		public const string AttributeValuePayloadID = "PAYLOADID";

		// Token: 0x040000C4 RID: 196
		public const string AttributeValueIsBundle = "IsBundle";

		// Token: 0x040000C5 RID: 197
		public const string AttributeValueDependencyPkgs = "DependencyPackages";
	}
}
