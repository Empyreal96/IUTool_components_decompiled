using System;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000024 RID: 36
	public class CapabilityRulesInstallationFolder : CapabilityRulesDirectory
	{
		// Token: 0x06000134 RID: 308 RVA: 0x00006C4D File Offset: 0x00004E4D
		public CapabilityRulesInstallationFolder()
		{
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00006C55 File Offset: 0x00004E55
		public CapabilityRulesInstallationFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00006C64 File Offset: 0x00004E64
		protected sealed override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.Rights = "0x001200A9";
			base.CompileAttributes(appCapSID, svcCapSID);
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				base.Inheritance,
				base.Rights,
				"OW"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"FA",
				"SY"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"FA",
				"S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				"OW"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				appCapSID
			});
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_CODE_R");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_CODE_R"];
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				text
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_CODE_RW");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_CODE_RW"];
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001201bf",
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001201bf",
				text
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001201bf",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006F9C File Offset: 0x0000519C
		protected sealed override void ValidateOutPath()
		{
			base.ValidateFileOutPath(true);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00006FA5 File Offset: 0x000051A5
		protected override void AddAttributes(XmlElement baseRuleXmlElement)
		{
			base.AddAttributes(baseRuleXmlElement);
			base.Flags |= 2147483648U;
		}
	}
}
