using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200002B RID: 43
	public class FeatureCondition
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00008A37 File Offset: 0x00006C37
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00008A3F File Offset: 0x00006C3F
		[XmlElement]
		public ConditionSet ConditionSet { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00008A48 File Offset: 0x00006C48
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00008A50 File Offset: 0x00006C50
		[XmlElement]
		public Condition Condition { get; set; }

		// Token: 0x06000100 RID: 256 RVA: 0x0000318A File Offset: 0x0000138A
		public FeatureCondition()
		{
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00008A59 File Offset: 0x00006C59
		public FeatureCondition(ConditionSet conditionSet)
		{
			this.ConditionSet = new ConditionSet(conditionSet);
			this.Condition = null;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00008A74 File Offset: 0x00006C74
		public FeatureCondition(Condition condition)
		{
			this.Condition = new Condition(condition);
			this.ConditionSet = null;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008A90 File Offset: 0x00006C90
		public FeatureCondition(FeatureCondition srcFC)
		{
			if (srcFC.Condition != null)
			{
				this.Condition = new Condition(srcFC.Condition);
			}
			if (srcFC.ConditionSet != null)
			{
				this.ConditionSet = new ConditionSet(srcFC.ConditionSet);
			}
			this.UpdateAction = srcFC.UpdateAction;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00008AE4 File Offset: 0x00006CE4
		public override string ToString()
		{
			if (this.UpdateAction.ToString() + ": " + this.ConditionSet != null)
			{
				return "ConditionSet=(" + this.ConditionSet.ToString() + ")";
			}
			return "Condition=" + this.Condition.ToString();
		}

		// Token: 0x040000D7 RID: 215
		[XmlAttribute]
		[DefaultValue(FeatureCondition.Action.Install)]
		public FeatureCondition.Action UpdateAction;

		// Token: 0x02000045 RID: 69
		public enum Action
		{
			// Token: 0x04000153 RID: 339
			Install,
			// Token: 0x04000154 RID: 340
			Remove,
			// Token: 0x04000155 RID: 341
			NoUpdate
		}
	}
}
