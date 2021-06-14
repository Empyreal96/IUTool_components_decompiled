using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000029 RID: 41
	public class Condition
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000860A File Offset: 0x0000680A
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00008612 File Offset: 0x00006812
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000861B File Offset: 0x0000681B
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00008623 File Offset: 0x00006823
		[XmlAttribute]
		public string Value { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000862C File Offset: 0x0000682C
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00008634 File Offset: 0x00006834
		[XmlAttribute]
		public string RegistryKey { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x0000863D File Offset: 0x0000683D
		// (set) Token: 0x060000EA RID: 234 RVA: 0x00008645 File Offset: 0x00006845
		[XmlAttribute]
		public string FMID { get; set; }

		// Token: 0x060000EB RID: 235 RVA: 0x0000864E File Offset: 0x0000684E
		public Condition()
		{
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008664 File Offset: 0x00006864
		public Condition(Condition srcCond)
		{
			this.FeatureStatus = srcCond.FeatureStatus;
			this.FMID = srcCond.FMID;
			this.Name = srcCond.Name;
			this.Operator = srcCond.Operator;
			this.RegistryKey = srcCond.RegistryKey;
			this.RegistryKeyType = srcCond.RegistryKeyType;
			this.Type = srcCond.Type;
			this.Value = srcCond.Value;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000086E5 File Offset: 0x000068E5
		public void SetRegistry(string key, string name, RegistryValueKind keyType, string value)
		{
			this.SetRegistry(key, name, keyType, value, Condition.ConditionOperator.EQ);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000086F3 File Offset: 0x000068F3
		public void SetRegistry(string key, string name, RegistryValueKind keyType, string value, Condition.ConditionOperator conditionOperator)
		{
			this.Type = Condition.ConditionType.Registry;
			this.Operator = conditionOperator;
			this.RegistryKey = key;
			this.Name = name;
			this.RegistryKeyType = keyType;
			this.Value = value;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008721 File Offset: 0x00006921
		public void SetNameValue(string name, string value)
		{
			this.SetNameValue(name, value, Condition.ConditionOperator.EQ);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000872C File Offset: 0x0000692C
		public void SetNameValue(string name, string value, Condition.ConditionOperator conditionOperator)
		{
			this.Type = Condition.ConditionType.NameValuePair;
			this.Operator = conditionOperator;
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000874A File Offset: 0x0000694A
		public void SetFeature(string featureID, string FMID)
		{
			this.SetFeature(featureID, FMID, Condition.FeatureStatuses.Installed);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008755 File Offset: 0x00006955
		public void SetFeature(string featureID, string FMID, Condition.FeatureStatuses featureStatus)
		{
			this.Type = Condition.ConditionType.Feature;
			this.FeatureStatus = featureStatus;
			this.Name = featureID;
			this.FMID = FMID;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00008774 File Offset: 0x00006974
		public override string ToString()
		{
			string text = this.Type.ToString() + ": ";
			switch (this.Type)
			{
			case Condition.ConditionType.NameValuePair:
				text = string.Concat(new string[]
				{
					text,
					"Name=",
					this.Name,
					" Value=",
					this.Value
				});
				break;
			case Condition.ConditionType.Registry:
				text = string.Concat(new string[]
				{
					text,
					"Path=",
					this.RegistryKey,
					"\\",
					this.Name,
					string.IsNullOrEmpty(this.Value) ? "" : (" Value=" + this.Value)
				});
				break;
			case Condition.ConditionType.Feature:
				text += this.Value;
				break;
			}
			return text;
		}

		// Token: 0x040000CC RID: 204
		[XmlAttribute]
		public Condition.ConditionType Type;

		// Token: 0x040000CD RID: 205
		[XmlAttribute]
		[DefaultValue(Condition.ConditionOperator.EQ)]
		public Condition.ConditionOperator Operator = Condition.ConditionOperator.EQ;

		// Token: 0x040000D1 RID: 209
		[XmlAttribute]
		[DefaultValue(RegistryValueKind.None)]
		public RegistryValueKind RegistryKeyType = RegistryValueKind.None;

		// Token: 0x040000D3 RID: 211
		[XmlAttribute]
		[DefaultValue(Condition.FeatureStatuses.Installed)]
		public Condition.FeatureStatuses FeatureStatus;

		// Token: 0x02000040 RID: 64
		public enum ConditionType
		{
			// Token: 0x0400013D RID: 317
			NameValuePair,
			// Token: 0x0400013E RID: 318
			Registry,
			// Token: 0x0400013F RID: 319
			Feature
		}

		// Token: 0x02000041 RID: 65
		public enum ConditionOperator
		{
			// Token: 0x04000141 RID: 321
			GT,
			// Token: 0x04000142 RID: 322
			GTE,
			// Token: 0x04000143 RID: 323
			LT,
			// Token: 0x04000144 RID: 324
			LTE,
			// Token: 0x04000145 RID: 325
			EQ,
			// Token: 0x04000146 RID: 326
			NEQ,
			// Token: 0x04000147 RID: 327
			EXISTS,
			// Token: 0x04000148 RID: 328
			NOTEXISTS
		}

		// Token: 0x02000042 RID: 66
		public enum FeatureStatuses
		{
			// Token: 0x0400014A RID: 330
			Installed,
			// Token: 0x0400014B RID: 331
			NotInstalled
		}
	}
}
