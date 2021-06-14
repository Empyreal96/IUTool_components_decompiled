using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000028 RID: 40
	public class CapabilityRulesChamberProfileMediaFolder : CapabilityRulesChamberProfileDefaultDataFolder
	{
		// Token: 0x06000144 RID: 324 RVA: 0x000072FE File Offset: 0x000054FE
		public CapabilityRulesChamberProfileMediaFolder()
		{
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000075ED File Offset: 0x000057ED
		public CapabilityRulesChamberProfileMediaFolder(CapabilityOwnerType value) : this()
		{
			base.OwnerType = value;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000075FC File Offset: 0x000057FC
		protected sealed override void CompileAttributes(string appCapSID, string svcCapSID)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			base.CompileAttributes(appCapSID, svcCapSID);
			base.Rights = "0x001301bf";
			text = SidBuilder.BuildApplicationCapabilitySidString("ID_CAP_CHAMBER_PROFILE_DATA_MEDIA_RWD");
			text2 = GlobalVariables.SidMapping["ID_CAP_CHAMBER_PROFILE_DATA_MEDIA_RWD"];
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
