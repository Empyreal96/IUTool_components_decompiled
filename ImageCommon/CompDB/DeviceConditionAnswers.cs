using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000012 RID: 18
	public class DeviceConditionAnswers : ConditionSet
	{
		// Token: 0x060000BB RID: 187 RVA: 0x0000A1CA File Offset: 0x000083CA
		public DeviceConditionAnswers()
		{
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000A1D2 File Offset: 0x000083D2
		public DeviceConditionAnswers(IULogger logger)
		{
			this._iuLogger = logger;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000A1E1 File Offset: 0x000083E1
		public DeviceConditionAnswers(DeviceConditionAnswers srcCA) : base(srcCA)
		{
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000A1EC File Offset: 0x000083EC
		public void PopulateConditionAnswers(List<FMConditionalFeature> condFeatures, List<Hashtable> registryTable)
		{
			List<Condition> list = new List<Condition>();
			foreach (Condition condition in condFeatures.SelectMany((FMConditionalFeature cf) => cf.GetAllConditions()))
			{
				switch (condition.Type)
				{
				case Condition.ConditionType.NameValuePair:
				{
					Condition condition2 = this.GetNameValuePairAnswer(condition);
					if (condition2 != null)
					{
						list.Add(condition2);
					}
					break;
				}
				case Condition.ConditionType.Registry:
				{
					Condition condition2 = this.GetRegistryAnswer(condition, registryTable);
					if (condition2 != null)
					{
						list.Add(condition2);
					}
					break;
				}
				}
			}
			if (list.Any<Condition>())
			{
				if (this.Conditions == null)
				{
					this.Conditions = new List<Condition>(list);
					return;
				}
				this.Conditions.AddRange(list);
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000A2C4 File Offset: 0x000084C4
		private Condition GetRegistryAnswer(Condition cond, List<Hashtable> registryTable)
		{
			string registryAnswer = DeviceConditionAnswers.GetRegistryAnswer(cond.RegistryKey, cond.Name, registryTable);
			if (!string.IsNullOrEmpty(registryAnswer))
			{
				Condition condition = new Condition();
				condition.SetRegistry(cond.RegistryKey, cond.Name, cond.RegistryKeyType, registryAnswer);
				return condition;
			}
			return null;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000A30C File Offset: 0x0000850C
		public static string GetRegistryAnswer(string registryKey, string name, List<Hashtable> registryTables)
		{
			string result = null;
			foreach (Hashtable hashtable in registryTables)
			{
				Hashtable hashtable2 = hashtable[registryKey] as Hashtable;
				if (hashtable2 != null)
				{
					object obj = hashtable2[name];
					if (obj != null)
					{
						result = obj.ToString();
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000A378 File Offset: 0x00008578
		private Condition GetNameValuePairAnswer(Condition cond)
		{
			throw new NotImplementedException("ImageCommon::DeviceCompDB!GetNameValuePairAnswer:  Not yet implemented.");
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000A384 File Offset: 0x00008584
		public override string ToString()
		{
			return "Device DB Condition Answers: Count=" + base.GetAllConditions().Count<Condition>();
		}

		// Token: 0x040000A1 RID: 161
		private IULogger _iuLogger;
	}
}
