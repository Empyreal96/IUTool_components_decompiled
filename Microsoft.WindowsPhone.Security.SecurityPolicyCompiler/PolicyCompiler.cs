using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000010 RID: 16
	public class PolicyCompiler : ISecurityPolicyCompiler
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000310F File Offset: 0x0000130F
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00003116 File Offset: 0x00001316
		public static bool BlockPolicyDefinition { get; private set; }

		// Token: 0x06000043 RID: 67 RVA: 0x0000311E File Offset: 0x0000131E
		public PolicyCompiler()
		{
			ReportingBase.UseInternalLogger = false;
			ReportingBase.EnableDebugMessage = false;
			this.report = ReportingBase.GetInstance();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000313D File Offset: 0x0000133D
		public PolicyCompiler(ReportingBase report)
		{
			this.report = report;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000314C File Offset: 0x0000134C
		public bool Compile(string packageId, string projectPath, IMacroResolver macroResolver, string policyPath)
		{
			return this.Compile(packageId, projectPath, null, macroResolver, policyPath);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000315C File Offset: 0x0000135C
		public bool Compile(string packageId, string projectPath, XmlDocument projectXml, IMacroResolver macroResolver, string policyPath)
		{
			this.report.DebugLine("Compiling fileName = " + projectPath);
			if (!File.Exists(projectPath) || string.IsNullOrEmpty(packageId) || macroResolver == null || string.IsNullOrEmpty(policyPath))
			{
				throw new ArgumentException("The input parameter is invalid!");
			}
			PolicyCompiler.BlockPolicyDefinition = false;
			bool readFileFromDisk = projectXml == null;
			XmlDocument xmlDocument = projectXml ?? new XmlDocument();
			GlobalVariables.MacroResolver = macroResolver;
			this.LoadPolicyFileAndGetLocalMacros(projectPath, xmlDocument, readFileFromDisk, true);
			PolicyXmlClass policyXmlClass = new PolicyXmlClass();
			policyXmlClass.PackageId = packageId;
			policyXmlClass.Add(projectPath, xmlDocument);
			policyXmlClass.Print();
			bool result = policyXmlClass.SaveToXml(policyPath);
			GlobalVariables.CurrentCompilationState = CompilationState.CompletedSuccessfully;
			return result;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000031F8 File Offset: 0x000013F8
		public void DriverSecurityInitialize(string projectPath, IMacroResolver macroResolver)
		{
			this.report.DebugLine("Compiling fileName = " + projectPath);
			if (!File.Exists(projectPath) || macroResolver == null)
			{
				throw new ArgumentException("The input parameter is invalid!");
			}
			this.driverPolicyXmlDocument = new XmlDocument();
			GlobalVariables.MacroResolver = macroResolver;
			bool readFileFromDisk = true;
			this.LoadPolicyFileAndGetLocalMacros(projectPath, this.driverPolicyXmlDocument, readFileFromDisk, false);
			this.DriverSecurityTemplateInitialize();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000325C File Offset: 0x0000145C
		protected void DriverSecurityTemplateInitialize()
		{
			this.driverRuleTemplateXmlDocument = new XmlDocument();
			string text;
			if (this.IsPhoneBuild())
			{
				text = Environment.ExpandEnvironmentVariables("%SDXMAPROOT%\\wm\\tools\\oak\\misc\\DriverRuleTemplate.xml");
			}
			else
			{
				text = Environment.ExpandEnvironmentVariables("%RAZZLETOOLPATH%\\managed\\v4.0\\DriverRuleTemplate.xml");
			}
			if (!File.Exists(text))
			{
				text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DriverRuleTemplate.xml");
				if (!File.Exists(text))
				{
					throw new PolicyCompilerInternalException("DriverRuleTemplate.xml can't be found.");
				}
			}
			this.driverRuleTemplateXmlDocument.Load(text);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000032D8 File Offset: 0x000014D8
		public string GetDriverSddlString(string infSectionName, string oldSddl)
		{
			if (this.driverPolicyXmlDocument == null || this.driverRuleTemplateXmlDocument == null || GlobalVariables.MacroResolver == null || string.IsNullOrEmpty(infSectionName))
			{
				throw new ArgumentException("The operation is invalid!");
			}
			return new DriverSecurity().GetSddlString(infSectionName, oldSddl, this.driverPolicyXmlDocument, this.driverRuleTemplateXmlDocument);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003327 File Offset: 0x00001527
		public bool IsPhoneBuild()
		{
			return Directory.Exists(Environment.ExpandEnvironmentVariables("%_WINPHONEROOT%"));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003340 File Offset: 0x00001540
		protected void PreprocessPolicy(XmlDocument policyXmlDocument, bool parseLocalMacros)
		{
			GlobalVariables.NamespaceManager = new XmlNamespaceManager(policyXmlDocument.NameTable);
			GlobalVariables.NamespaceManager.AddNamespace("WP_Policy", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00");
			if (parseLocalMacros)
			{
				foreach (object obj in policyXmlDocument.SelectNodes("//WP_Policy:Macros/WP_Policy:Macro", GlobalVariables.NamespaceManager))
				{
					XmlElement xmlElement = (XmlElement)obj;
					if (xmlElement.HasAttributes)
					{
						string attribute = xmlElement.GetAttribute("Id");
						string attribute2 = xmlElement.GetAttribute("Value");
						GlobalVariables.MacroResolver.Register(attribute, attribute2);
					}
				}
			}
			string text;
			if (this.IsPhoneBuild())
			{
				text = Environment.ExpandEnvironmentVariables("%_WINPHONEROOT%\\tools\\oak\\misc\\capabilitylist.cfg");
			}
			else
			{
				text = Environment.ExpandEnvironmentVariables("%RAZZLETOOLPATH%\\managed\\v4.0\\capabilitylist.cfg");
			}
			if (!File.Exists(text))
			{
				text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "capabilitylist.cfg");
				if (!File.Exists(text))
				{
					throw new PolicyCompilerInternalException("capabilitylist.cfg can't be found.");
				}
				PolicyCompiler.BlockPolicyDefinition = true;
			}
			GlobalVariables.SidMapping = SidMapping.CreateInstance(text);
			SidMapping.CompareToSnapshotMapping(GlobalVariables.SidMapping);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000346C File Offset: 0x0000166C
		protected void LoadPolicyFileAndGetLocalMacros(string policyXmlFileFullPath, XmlDocument policyXmlDocument, bool readFileFromDisk, bool parseLocalMacros)
		{
			GlobalVariables.CurrentCompilationState = CompilationState.PolicyFileLoadAndValidation;
			if (readFileFromDisk)
			{
				this.report.DebugLine("Loading Policy File : " + policyXmlFileFullPath);
				policyXmlDocument.Load(policyXmlFileFullPath);
			}
			try
			{
				this.PreprocessPolicy(policyXmlDocument, parseLocalMacros);
			}
			catch (XPathException originalException)
			{
				throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "File: {0}", new object[]
				{
					policyXmlFileFullPath
				}), originalException);
			}
			GlobalVariables.CurrentCompilationState = CompilationState.PolicyMacroDereferencing;
		}

		// Token: 0x040000BA RID: 186
		private ReportingBase report;

		// Token: 0x040000BB RID: 187
		protected XmlDocument driverPolicyXmlDocument;

		// Token: 0x040000BC RID: 188
		protected XmlDocument driverRuleTemplateXmlDocument;
	}
}
