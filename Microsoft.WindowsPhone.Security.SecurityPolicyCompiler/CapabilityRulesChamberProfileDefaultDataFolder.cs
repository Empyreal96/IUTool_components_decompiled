using System;
using System.Xml;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000025 RID: 37
	public class CapabilityRulesChamberProfileDefaultDataFolder : CapabilityRulesDirectory
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00006C4D File Offset: 0x00004E4D
		public CapabilityRulesChamberProfileDefaultDataFolder()
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00006FC0 File Offset: 0x000051C0
		public CapabilityRulesChamberProfileDefaultDataFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00006FD0 File Offset: 0x000051D0
		protected override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.Rights = "FA";
			base.CompileAttributes(appCapSID, svcCapSID);
			base.DACL = string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				"SY"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				"S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				"S-1-5-80-1551822644-3134808374-1042292604-2865742758-3851661496"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301ff",
				"OW"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301ff",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301ff",
				appCapSID
			});
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_R");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_R"];
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
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001200A9",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_RW");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_RW"];
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
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001201bf",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00006F9C File Offset: 0x0000519C
		protected sealed override void ValidateOutPath()
		{
			base.ValidateFileOutPath(true);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00006FA5 File Offset: 0x000051A5
		protected override void AddAttributes(XmlElement baseRuleXmlElement)
		{
			base.AddAttributes(baseRuleXmlElement);
			base.Flags |= 2147483648U;
		}
	}
}
