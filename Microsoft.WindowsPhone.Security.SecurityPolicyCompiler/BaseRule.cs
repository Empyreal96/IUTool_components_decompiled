using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000030 RID: 48
	public abstract class BaseRule : RulePolicyElement
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00008AE5 File Offset: 0x00006CE5
		// (set) Token: 0x06000187 RID: 391 RVA: 0x00008AED File Offset: 0x00006CED
		protected CapabilityOwnerType OwnerType
		{
			get
			{
				return this.ownerType;
			}
			set
			{
				this.ownerType = value;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00008AF6 File Offset: 0x00006CF6
		// (set) Token: 0x06000189 RID: 393 RVA: 0x00008AFE File Offset: 0x00006CFE
		protected string RuleType
		{
			get
			{
				return this.ruleType;
			}
			set
			{
				this.ruleType = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00008B07 File Offset: 0x00006D07
		protected string ResolvedRights
		{
			get
			{
				return this.resolvedRights;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00008B0F File Offset: 0x00006D0F
		protected bool ReadOnlyRights
		{
			get
			{
				return this.readOnlyRights;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00008B17 File Offset: 0x00006D17
		// (set) Token: 0x0600018D RID: 397 RVA: 0x00008B1F File Offset: 0x00006D1F
		protected string Inheritance
		{
			get
			{
				return this.inheritance;
			}
			set
			{
				this.inheritance = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00008B28 File Offset: 0x00006D28
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00008B30 File Offset: 0x00006D30
		protected string Rights
		{
			get
			{
				return this.inRights;
			}
			set
			{
				this.inRights = value;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00008B39 File Offset: 0x00006D39
		// (set) Token: 0x06000191 RID: 401 RVA: 0x00008B41 File Offset: 0x00006D41
		[XmlAttribute(AttributeName = "ElementID")]
		public string ElementId
		{
			get
			{
				return this.elementId;
			}
			set
			{
				this.elementId = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00008B4A File Offset: 0x00006D4A
		// (set) Token: 0x06000193 RID: 403 RVA: 0x00008B52 File Offset: 0x00006D52
		[XmlAttribute(AttributeName = "DACL")]
		public string DACL
		{
			get
			{
				return this.dacl;
			}
			set
			{
				this.dacl = value;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00008B5B File Offset: 0x00006D5B
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00008B63 File Offset: 0x00006D63
		[XmlAttribute(AttributeName = "Flags")]
		public uint Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00008B9C File Offset: 0x00006D9C
		protected virtual void AddAttributes(XmlElement baseRuleXmlElement)
		{
			if (this.ownerType == CapabilityOwnerType.Application || this.ownerType == CapabilityOwnerType.Service)
			{
				string attribute = baseRuleXmlElement.GetAttribute("ReadOnly");
				if (!string.IsNullOrEmpty(attribute) && !attribute.Equals("No", GlobalVariables.GlobalStringComparison))
				{
					this.readOnlyRights = true;
				}
				if (this.readOnlyRights)
				{
					this.Rights = "$(GENERIC_READ)";
				}
				else
				{
					this.Rights = "$(ALL_ACCESS)";
				}
				this.flags |= 2147483648U;
				return;
			}
			this.Rights = baseRuleXmlElement.GetAttribute("Rights");
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00008C2C File Offset: 0x00006E2C
		protected virtual void CompileAttributes(string appCapSID, string svcCapSID)
		{
			this.resolvedRights = this.ResolveMacro(this.inRights, "Rights");
			this.CalculateDACL(appCapSID, svcCapSID);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00008C4D File Offset: 0x00006E4D
		protected virtual void CompileResolvedAttributes(string appCapSID, string svcCapSID, string rights)
		{
			this.resolvedRights = rights;
			this.CalculateDACL(appCapSID, svcCapSID);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008C5E File Offset: 0x00006E5E
		protected string ResolveMacro(string valueWithMacro, string type)
		{
			return GlobalVariables.ResolveMacroReference(valueWithMacro, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				this.ruleType,
				type
			}));
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00008C88 File Offset: 0x00006E88
		protected void CalculateDACL(string appCapSID, string svcCapSID)
		{
			this.DACL = string.Empty;
			if (!string.IsNullOrEmpty(appCapSID))
			{
				this.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
				{
					this.Inheritance,
					this.resolvedRights,
					appCapSID
				});
				this.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
				{
					this.Inheritance,
					this.resolvedRights,
					"S-1-5-21-2702878673-795188819-444038987-1030"
				});
			}
			if (!string.IsNullOrEmpty(svcCapSID))
			{
				if (svcCapSID == "ID_CAP_NTSERVICES")
				{
					this.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
					{
						this.Inheritance,
						this.resolvedRights,
						"BU"
					});
					this.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
					{
						this.Inheritance,
						this.resolvedRights,
						"S-1-5-33"
					});
					return;
				}
				this.DACL += string.Format(GlobalVariables.Culture, "(A;{0};{1};;;{2})", new object[]
				{
					this.Inheritance,
					this.resolvedRights,
					svcCapSID
				});
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008DFC File Offset: 0x00006FFC
		protected void CalculateElementId(string value)
		{
			this.ElementId = HashCalculator.CalculateSha256Hash(this.ruleType + value, false);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008E18 File Offset: 0x00007018
		public override string GetAttributesString()
		{
			return this.Flags.ToString(GlobalVariables.Culture) + this.DACL;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008E44 File Offset: 0x00007044
		public override void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "ElementID", this.ElementId);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "DACL", this.DACL);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel5, "Flags", this.Flags.ToString(GlobalVariables.Culture));
		}

		// Token: 0x0400010C RID: 268
		private CapabilityOwnerType ownerType = CapabilityOwnerType.Unknown;

		// Token: 0x0400010D RID: 269
		private string ruleType;

		// Token: 0x0400010E RID: 270
		private string resolvedRights;

		// Token: 0x0400010F RID: 271
		private bool readOnlyRights;

		// Token: 0x04000110 RID: 272
		private string inheritance = string.Empty;

		// Token: 0x04000111 RID: 273
		private string inRights;

		// Token: 0x04000112 RID: 274
		private string elementId = "Not Calculated";

		// Token: 0x04000113 RID: 275
		private string dacl = "Not Calculated";

		// Token: 0x04000114 RID: 276
		protected const uint ProtectedDaclFlag = 2147483648U;

		// Token: 0x04000115 RID: 277
		protected const uint ProtectedSaclFlag = 1073741824U;

		// Token: 0x04000116 RID: 278
		protected const uint DefaultDaclFlagMask = 255U;

		// Token: 0x04000117 RID: 279
		protected const uint DefaultDaclGenericAccessInheritFlag = 1U;

		// Token: 0x04000118 RID: 280
		protected const uint DefaultDaclGenericAccessFlag = 2U;

		// Token: 0x04000119 RID: 281
		protected const uint DefaultDaclAllAccessInheritFlag = 3U;

		// Token: 0x0400011A RID: 282
		protected const uint DefaultDaclAllAccessFlag = 4U;

		// Token: 0x0400011B RID: 283
		protected const uint DefaultDaclAllAccessWithCOFlag = 5U;

		// Token: 0x0400011C RID: 284
		protected const uint DefaultDaclServiceAccessFlag = 6U;

		// Token: 0x0400011D RID: 285
		protected const uint DefaultOwnerFlagMask = 65280U;

		// Token: 0x0400011E RID: 286
		protected const uint DefaultOwnerTrustedInstallerFlag = 256U;

		// Token: 0x0400011F RID: 287
		protected const uint DefaultOwnerSystemFlag = 512U;

		// Token: 0x04000120 RID: 288
		protected const uint DefaultOwnerAdminFlag = 768U;

		// Token: 0x04000121 RID: 289
		protected const uint DefaultOwnerAdminSystemFlag = 1024U;

		// Token: 0x04000122 RID: 290
		protected const uint DefaultMandatoryLabelFlagMask = 16711680U;

		// Token: 0x04000123 RID: 291
		protected const uint DefaultMandatoryLabelInheritFlag = 65536U;

		// Token: 0x04000124 RID: 292
		protected const uint DefaultMandatoryLabelFlag = 131072U;

		// Token: 0x04000125 RID: 293
		protected const uint DefaultExecuteMandatoryLabelInheritFlag = 196608U;

		// Token: 0x04000126 RID: 294
		protected const uint DefaultExecuteMandatoryLabelFlag = 262144U;

		// Token: 0x04000127 RID: 295
		protected const uint DefaultWriteMandatoryLabelInheritFlag = 327680U;

		// Token: 0x04000128 RID: 296
		protected const uint DefaultWriteMandatoryLabelFlag = 393216U;

		// Token: 0x04000129 RID: 297
		private uint flags;
	}
}
