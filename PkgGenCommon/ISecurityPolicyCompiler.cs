using System;
using System.Xml;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200000C RID: 12
	public interface ISecurityPolicyCompiler
	{
		// Token: 0x0600002E RID: 46
		bool Compile(string packageName, string projectPath, IMacroResolver macroResolver, string policyPath);

		// Token: 0x0600002F RID: 47
		bool Compile(string packageName, string projectPath, XmlDocument projectXml, IMacroResolver macroResolver, string policyPath);

		// Token: 0x06000030 RID: 48
		void DriverSecurityInitialize(string projectPath, IMacroResolver macroResolver);

		// Token: 0x06000031 RID: 49
		string GetDriverSddlString(string infSectionName, string oldSddl);
	}
}
