using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200002A RID: 42
	public class ConditionSet
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x0000885B File Offset: 0x00006A5B
		public bool ShouldSerializeConditions()
		{
			return this.Conditions != null && this.Conditions.Count<Condition>() > 0;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00008875 File Offset: 0x00006A75
		public bool ShouldSerializeConditionSets()
		{
			return this.ConditionSets != null && this.ConditionSets.Count<ConditionSet>() > 0;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000318A File Offset: 0x0000138A
		public ConditionSet()
		{
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00008890 File Offset: 0x00006A90
		public ConditionSet(ConditionSet srcCS)
		{
			if (srcCS.Conditions != null)
			{
				this.Conditions = (from cs in srcCS.Conditions
				select new Condition(cs)).ToList<Condition>();
			}
			if (srcCS.ConditionSets != null)
			{
				this.ConditionSets = (from cs in srcCS.ConditionSets
				select new ConditionSet(cs)).ToList<ConditionSet>();
			}
			this.Operator = srcCS.Operator;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00008929 File Offset: 0x00006B29
		public ConditionSet(ConditionSet.ConditionSetOperator conditionSetOperator)
		{
			this.Operator = conditionSetOperator;
			this.Conditions = new List<Condition>();
			this.ConditionSets = new List<ConditionSet>();
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000894E File Offset: 0x00006B4E
		public ConditionSet(ConditionSet.ConditionSetOperator conditionSetOperator, List<Condition> conditionList, List<ConditionSet> conditionSetList)
		{
			this.Operator = conditionSetOperator;
			this.Conditions = conditionList;
			this.ConditionSets = conditionSetList;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000896C File Offset: 0x00006B6C
		public List<Condition> GetAllConditions()
		{
			List<Condition> list = new List<Condition>();
			if (this.ConditionSets != null)
			{
				foreach (ConditionSet conditionSet in this.ConditionSets)
				{
					list.AddRange(conditionSet.GetAllConditions());
				}
			}
			if (this.Conditions != null)
			{
				list.AddRange(this.Conditions);
			}
			return list;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000089E8 File Offset: 0x00006BE8
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Operator.ToString(),
				": Conditions=Count (",
				this.GetAllConditions().Count<Condition>(),
				")"
			});
		}

		// Token: 0x040000D4 RID: 212
		[XmlAttribute]
		[DefaultValue(ConditionSet.ConditionSetOperator.AND)]
		public ConditionSet.ConditionSetOperator Operator;

		// Token: 0x040000D5 RID: 213
		[XmlArrayItem(ElementName = "Condition", Type = typeof(Condition), IsNullable = false)]
		public List<Condition> Conditions;

		// Token: 0x040000D6 RID: 214
		[XmlArrayItem(ElementName = "ConditionSet", Type = typeof(ConditionSet), IsNullable = false)]
		public List<ConditionSet> ConditionSets;

		// Token: 0x02000043 RID: 67
		public enum ConditionSetOperator
		{
			// Token: 0x0400014D RID: 333
			AND,
			// Token: 0x0400014E RID: 334
			OR
		}
	}
}
