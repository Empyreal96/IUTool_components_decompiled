using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000027 RID: 39
	public class CapabilityRulesChamberProfileShellContentFolder : CapabilityRulesChamberProfileDefaultDataFolder
	{
		// Token: 0x06000141 RID: 321 RVA: 0x000072FE File Offset: 0x000054FE
		public CapabilityRulesChamberProfileShellContentFolder()
		{
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007414 File Offset: 0x00005614
		public CapabilityRulesChamberProfileShellContentFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007424 File Offset: 0x00005624
		protected sealed override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.CompileAttributes(appCapSID, svcCapSID);
			base.Rights = "0x001200A9";
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_R");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_R"];
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
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_RWD");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_SHELLCONTENT_RWD"];
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301bf",
				text
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301bf",
				text2
			});
			base.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
			{
				"CIOI",
				"0x001301bf",
				"S-1-5-21-2702878673-795188819-444038987-2781"
			});
		}
	}
}
