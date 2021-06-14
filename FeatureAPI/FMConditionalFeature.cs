using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000020 RID: 32
	public class FMConditionalFeature : FeatureCondition
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00007AAC File Offset: 0x00005CAC
		[XmlIgnore]
		public string FeatureIDWithFMID
		{
			get
			{
				string text = this.FeatureID;
				if (!string.IsNullOrEmpty(this.FMID))
				{
					text = text + "." + this.FMID;
				}
				return text;
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00007AE0 File Offset: 0x00005CE0
		public FMConditionalFeature()
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00007AE8 File Offset: 0x00005CE8
		public FMConditionalFeature(FMConditionalFeature srcFeature)
		{
			if (srcFeature.Condition != null)
			{
				base.Condition = new Condition(srcFeature.Condition);
			}
			if (srcFeature.ConditionSet != null)
			{
				base.ConditionSet = new ConditionSet(srcFeature.ConditionSet);
			}
			this.FeatureID = srcFeature.FeatureID;
			this.FMID = srcFeature.FMID;
			this.UpdateAction = srcFeature.UpdateAction;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007B54 File Offset: 0x00005D54
		public List<Condition> GetAllConditions()
		{
			List<Condition> list = new List<Condition>();
			if (base.ConditionSet != null && base.ConditionSet.ConditionSets != null)
			{
				foreach (ConditionSet conditionSet in base.ConditionSet.ConditionSets)
				{
					list.AddRange(conditionSet.GetAllConditions());
				}
			}
			if (base.ConditionSet != null && base.ConditionSet.Conditions != null)
			{
				list.AddRange(base.ConditionSet.Conditions);
			}
			return list;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00007BF4 File Offset: 0x00005DF4
		public override string ToString()
		{
			return this.FeatureIDWithFMID;
		}

		// Token: 0x040000A2 RID: 162
		[XmlAttribute]
		[DefaultValue(null)]
		public string FeatureID;

		// Token: 0x040000A3 RID: 163
		[XmlAttribute]
		[DefaultValue(null)]
		public string FMID;
	}
}
