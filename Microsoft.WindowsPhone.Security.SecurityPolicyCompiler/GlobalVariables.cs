using System;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000007 RID: 7
	public static class GlobalVariables
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000022E2 File Offset: 0x000004E2
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000022E9 File Offset: 0x000004E9
		public static XmlNamespaceManager NamespaceManager
		{
			get
			{
				return GlobalVariables.namespaceManager;
			}
			set
			{
				GlobalVariables.namespaceManager = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000022F1 File Offset: 0x000004F1
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000022F8 File Offset: 0x000004F8
		public static IMacroResolver MacroResolver { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002300 File Offset: 0x00000500
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002307 File Offset: 0x00000507
		internal static SidMapping SidMapping { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000230F File Offset: 0x0000050F
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002316 File Offset: 0x00000516
		internal static bool IsInPackageAllowList { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000231E File Offset: 0x0000051E
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002325 File Offset: 0x00000525
		public static CompilationState CurrentCompilationState
		{
			get
			{
				return GlobalVariables.currentCompilationState;
			}
			set
			{
				GlobalVariables.currentCompilationState = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000232D File Offset: 0x0000052D
		public static CultureInfo Culture
		{
			get
			{
				return GlobalVariables.culture;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002334 File Offset: 0x00000534
		public static StringComparison GlobalStringComparison
		{
			get
			{
				return GlobalVariables.stringComparison;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000233C File Offset: 0x0000053C
		public static string ResolveMacroReference(string valueWithMacro, string errorExtraInfo)
		{
			string result = string.Empty;
			try
			{
				result = GlobalVariables.MacroResolver.Resolve(valueWithMacro);
			}
			catch (PkgGenException originalException)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Macro Referencing Error: {0}, Value= {1}", new object[]
				{
					errorExtraInfo,
					valueWithMacro
				}), originalException);
			}
			return result;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002394 File Offset: 0x00000594
		public static string GetPhoneSDDL(string sidMappingFilePath, string capId, string rights)
		{
			SidMapping sidMapping = SidMapping.CreateInstance(sidMappingFilePath);
			StringBuilder stringBuilder = new StringBuilder("D:P(A;;GA;;;SY)");
			DriverRule driverRule = new DriverRule("AccessedByCapability");
			string svcCapSID = sidMapping[capId];
			string appCapSID = SidBuilder.BuildApplicationCapabilitySidString(capId);
			driverRule.Add(appCapSID, svcCapSID, rights);
			stringBuilder.Append(driverRule.DACL);
			return stringBuilder.ToString();
		}

		// Token: 0x040000A7 RID: 167
		private static XmlNamespaceManager namespaceManager;

		// Token: 0x040000AB RID: 171
		private static CompilationState currentCompilationState = CompilationState.Unknown;

		// Token: 0x040000AC RID: 172
		private static CultureInfo culture = new CultureInfo("en-US", false);

		// Token: 0x040000AD RID: 173
		private static StringComparison stringComparison = StringComparison.Ordinal;

		// Token: 0x040000AE RID: 174
		private const int maxReferenceLevel = 100;
	}
}
