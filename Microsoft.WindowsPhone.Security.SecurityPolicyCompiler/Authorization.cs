using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000021 RID: 33
	public class Authorization : IPolicyElement
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600011E RID: 286 RVA: 0x000066F4 File Offset: 0x000048F4
		[XmlElement(ElementName = "PrincipalClass")]
		public List<PrincipalClass> PrincipalClassCollection
		{
			get
			{
				return this.principalClassCollection;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600011F RID: 287 RVA: 0x000066FC File Offset: 0x000048FC
		[XmlElement(ElementName = "CapabilityClass")]
		public List<CapabilityClass> CapabilityClassCollection
		{
			get
			{
				return this.capabilityClassCollection;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00006704 File Offset: 0x00004904
		[XmlElement(ElementName = "ExecuteRule")]
		public List<ExecuteRule> ExecuteRuleCollection
		{
			get
			{
				return this.executeRuleCollection;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000670C File Offset: 0x0000490C
		[XmlElement(ElementName = "CapabilityRule")]
		public List<CapabilityRule> CapabilityRuleCollection
		{
			get
			{
				return this.capabilityRuleCollection;
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00006714 File Offset: 0x00004914
		public void Add(IXPathNavigable authorizationXmlElement)
		{
			this.AddElements((XmlElement)authorizationXmlElement);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00006722 File Offset: 0x00004922
		private void AddElements(XmlElement authorizationXmlElement)
		{
			this.AddPrincipalClasses(authorizationXmlElement);
			this.AddCapabilityClasses(authorizationXmlElement);
			this.AddExecuteRules(authorizationXmlElement);
			this.AddCapabilityRules(authorizationXmlElement);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00006740 File Offset: 0x00004940
		private void AddPrincipalClasses(XmlElement authorizationXmlElement)
		{
			XmlNodeList xmlNodeList = authorizationXmlElement.SelectNodes("./WP_Policy:PrincipalClass", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.principalClassCollection == null)
				{
					this.principalClassCollection = new List<PrincipalClass>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement principalClassXmlElement = (XmlElement)obj;
					PrincipalClass principalClass = new PrincipalClass();
					principalClass.Add(principalClassXmlElement);
					this.principalClassCollection.Add(principalClass);
				}
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000067D8 File Offset: 0x000049D8
		private void AddCapabilityClasses(XmlElement authorizationXmlElement)
		{
			XmlNodeList xmlNodeList = authorizationXmlElement.SelectNodes("./WP_Policy:CapabilityClass", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.capabilityClassCollection == null)
				{
					this.capabilityClassCollection = new List<CapabilityClass>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement capabilityClassXmlElement = (XmlElement)obj;
					CapabilityClass capabilityClass = new CapabilityClass();
					capabilityClass.Add(capabilityClassXmlElement);
					this.capabilityClassCollection.Add(capabilityClass);
				}
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00006870 File Offset: 0x00004A70
		private void AddExecuteRules(XmlElement authorizationXmlElement)
		{
			XmlNodeList xmlNodeList = authorizationXmlElement.SelectNodes("./WP_Policy:ExecuteRule", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.executeRuleCollection == null)
				{
					this.executeRuleCollection = new List<ExecuteRule>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement authorizationRuleXmlElement = (XmlElement)obj;
					ExecuteRule executeRule = new ExecuteRule();
					executeRule.Add(authorizationRuleXmlElement);
					this.executeRuleCollection.Add(executeRule);
				}
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00006908 File Offset: 0x00004B08
		private void AddCapabilityRules(XmlElement authorizationXmlElement)
		{
			XmlNodeList xmlNodeList = authorizationXmlElement.SelectNodes("./WP_Policy:CapabilityRule", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.capabilityRuleCollection == null)
				{
					this.capabilityRuleCollection = new List<CapabilityRule>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement authorizationRuleXmlElement = (XmlElement)obj;
					CapabilityRule capabilityRule = new CapabilityRule();
					capabilityRule.Add(authorizationRuleXmlElement);
					this.capabilityRuleCollection.Add(capabilityRule);
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000069A0 File Offset: 0x00004BA0
		public bool HasChild()
		{
			return (this.principalClassCollection != null && this.principalClassCollection.Count > 0) || (this.capabilityClassCollection != null && this.capabilityClassCollection.Count > 0) || (this.executeRuleCollection != null && this.executeRuleCollection.Count > 0) || (this.capabilityRuleCollection != null && this.capabilityRuleCollection.Count > 0);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00006A0C File Offset: 0x00004C0C
		public void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel1, "Authorization");
			if (this.principalClassCollection != null)
			{
				foreach (PrincipalClass principalClass in this.principalClassCollection)
				{
					instance.DebugLine(string.Empty);
					principalClass.Print();
				}
			}
			if (this.capabilityClassCollection != null)
			{
				foreach (CapabilityClass capabilityClass in this.capabilityClassCollection)
				{
					instance.DebugLine(string.Empty);
					capabilityClass.Print();
				}
			}
			if (this.executeRuleCollection != null)
			{
				foreach (AuthorizationRule authorizationRule in this.executeRuleCollection)
				{
					instance.DebugLine(string.Empty);
					authorizationRule.Print();
				}
			}
			if (this.capabilityRuleCollection != null)
			{
				foreach (AuthorizationRule authorizationRule2 in this.capabilityRuleCollection)
				{
					instance.DebugLine(string.Empty);
					authorizationRule2.Print();
				}
			}
		}

		// Token: 0x040000FC RID: 252
		private List<PrincipalClass> principalClassCollection;

		// Token: 0x040000FD RID: 253
		private List<CapabilityClass> capabilityClassCollection;

		// Token: 0x040000FE RID: 254
		private List<ExecuteRule> executeRuleCollection;

		// Token: 0x040000FF RID: 255
		private List<CapabilityRule> capabilityRuleCollection;
	}
}
