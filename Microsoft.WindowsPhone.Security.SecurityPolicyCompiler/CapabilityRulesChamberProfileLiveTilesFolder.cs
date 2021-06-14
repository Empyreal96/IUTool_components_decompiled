using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000029 RID: 41
	public class CapabilityRulesChamberProfileLiveTilesFolder : CapabilityRulesChamberProfileDefaultDataFolder
	{
		// Token: 0x06000147 RID: 327 RVA: 0x000072FE File Offset: 0x000054FE
		public CapabilityRulesChamberProfileLiveTilesFolder()
		{
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000076F8 File Offset: 0x000058F8
		public CapabilityRulesChamberProfileLiveTilesFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00007708 File Offset: 0x00005908
		protected sealed override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.CompileAttributes(appCapSID, svcCapSID);
			base.Rights = "0x001301bf";
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_LIVETILES_RWD");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_LIVETILES_RWD"];
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
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_R");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_R"];
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
		}
	}
}
