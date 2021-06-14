using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000026 RID: 38
	public class CapabilityRulesChamberProfilePlatformDataFolder : CapabilityRulesChamberProfileDefaultDataFolder
	{
		// Token: 0x0600013E RID: 318 RVA: 0x000072FE File Offset: 0x000054FE
		public CapabilityRulesChamberProfilePlatformDataFolder()
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007306 File Offset: 0x00005506
		public CapabilityRulesChamberProfilePlatformDataFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007318 File Offset: 0x00005518
		protected override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.CompileAttributes(appCapSID, svcCapSID);
			base.Rights = "0x001301ff";
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_PLATFORMDATA_ALL");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_PLATFORMDATA_ALL"];
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				text
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				base.Rights,
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
		}
	}
}
