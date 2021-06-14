using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.WindowsPhone.FeatureAPI;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000007 RID: 7
	public static class DeviceLayoutValidatorExpressionEvaluator
	{
		// Token: 0x06000043 RID: 67 RVA: 0x000022C2 File Offset: 0x000004C2
		private static bool GetOemInputField(string fieldName, out string fieldValue)
		{
			return DeviceLayoutValidatorExpressionEvaluator._oemInputFields.TryGetValue(fieldName.ToLower(CultureInfo.InvariantCulture), out fieldValue);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000022DC File Offset: 0x000004DC
		public static void Initialize(OEMInput OemInput, IULogger Logger)
		{
			DeviceLayoutValidatorExpressionEvaluator._oemInput = OemInput;
			DeviceLayoutValidatorExpressionEvaluator._logger = Logger;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.edition"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.Edition.ToString();
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.product"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.Product;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.ismmos"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.IsMMOS.ToString();
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.description"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.Description;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.soc"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.SOC;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.sv"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.SV;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.device"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.Device;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.releasetype"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.ReleaseType;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.buildtype"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.BuildType;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.formatdpp"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.FormatDPP;
			DeviceLayoutValidatorExpressionEvaluator._oemInputFields["oeminput.cputype"] = DeviceLayoutValidatorExpressionEvaluator._oemInput.CPUType;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002418 File Offset: 0x00000618
		public static bool EvaluateBooleanExpression(string StringValue, string Expression)
		{
			bool flag = false;
			foreach (DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax expressionSyntax in DeviceLayoutValidatorExpressionEvaluator._implementedSyntax)
			{
				Match match = expressionSyntax.RegularExpression.Match(Expression);
				if (match.Success)
				{
					flag = expressionSyntax.Evaluator(StringValue, match.Groups[1].Value.Split(new char[]
					{
						',',
						':'
					}));
					if (flag)
					{
						DeviceLayoutValidatorExpressionEvaluator._logger.LogInfo("DeviceLayoutValidation: EvaluateBooleanExpression SUCCEEDED '" + StringValue + "->" + Expression, new object[0]);
					}
					else
					{
						DeviceLayoutValidatorExpressionEvaluator._logger.LogError("DeviceLayoutValidation: EvaluateBooleanExpression FAILED '" + StringValue + "->" + Expression, new object[0]);
					}
				}
			}
			return flag;
		}

		// Token: 0x0400002E RID: 46
		private static OEMInput _oemInput;

		// Token: 0x0400002F RID: 47
		private static IULogger _logger;

		// Token: 0x04000030 RID: 48
		private static Dictionary<string, string> _oemInputFields = new Dictionary<string, string>();

		// Token: 0x04000031 RID: 49
		private static Func<string, string[], bool, Func<uint, uint, uint, bool>, bool> UintValueEvaluator = delegate(string compareValue, string[] parameters, bool compareAll, Func<uint, uint, uint, bool> comparator)
		{
			uint arg;
			if (DeviceLayoutValidator.StringToUint(compareValue, out arg))
			{
				uint num = 0U;
				for (int i = 0; i < parameters.Length; i++)
				{
					string text = parameters[i].Trim();
					string valueAsString;
					if (!DeviceLayoutValidatorExpressionEvaluator.GetOemInputField(text, out valueAsString))
					{
						valueAsString = text;
					}
					uint arg2;
					if (!DeviceLayoutValidator.StringToUint(valueAsString, out arg2))
					{
						return false;
					}
					if (comparator(arg, arg2, num))
					{
						if (!compareAll)
						{
							return true;
						}
					}
					else if (compareAll)
					{
						return false;
					}
					num += 1U;
				}
				return compareAll;
			}
			return false;
		};

		// Token: 0x04000032 RID: 50
		private static Func<string, string[], bool, Func<string, string, uint, bool>, bool> StringValueEvaluator = delegate(string compareValue, string[] parameters, bool compareAll, Func<string, string, uint, bool> comparator)
		{
			uint num = 0U;
			for (int i = 0; i < parameters.Length; i++)
			{
				string text = parameters[i].Trim();
				string arg;
				if (!DeviceLayoutValidatorExpressionEvaluator.GetOemInputField(text, out arg))
				{
					arg = text;
				}
				if (comparator(compareValue, arg, num))
				{
					if (!compareAll)
					{
						return true;
					}
				}
				else if (compareAll)
				{
					return false;
				}
				num += 1U;
			}
			return compareAll;
		};

		// Token: 0x04000033 RID: 51
		private static DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax[] _implementedSyntax = new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax[]
		{
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^equal\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = delegate(string stringValue, string[] parameters)
				{
					if (DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, (uint uintValue, uint uintParam, uint index) => uintValue == uintParam))
					{
						return true;
					}
					return DeviceLayoutValidatorExpressionEvaluator.StringValueEvaluator(stringValue, parameters, true, (string strValue, string strParam, uint index) => strValue.Equals(strParam, StringComparison.OrdinalIgnoreCase));
				}
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^greater\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = ((string stringValue, string[] parameters) => DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, (uint uintValue, uint uintParam, uint index) => uintValue > uintParam))
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^less\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = ((string stringValue, string[] parameters) => DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, (uint uintValue, uint uintParam, uint index) => uintValue < uintParam))
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^greater_or_equal\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = ((string stringValue, string[] parameters) => DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, (uint uintValue, uint uintParam, uint index) => uintValue >= uintParam))
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^less_or_equal\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = ((string stringValue, string[] parameters) => DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, (uint uintValue, uint uintParam, uint index) => uintValue <= uintParam))
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^between\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = ((string stringValue, string[] parameters) => DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, true, delegate(uint uintValue, uint uintParam, uint index)
				{
					if (index == 0U)
					{
						return uintValue >= uintParam;
					}
					return index == 1U && uintValue <= uintParam;
				}))
			},
			new DeviceLayoutValidatorExpressionEvaluator.ExpressionSyntax
			{
				RegularExpression = new Regex("^one_of\\((.+)\\)$", RegexOptions.IgnoreCase),
				Evaluator = delegate(string stringValue, string[] parameters)
				{
					if (DeviceLayoutValidatorExpressionEvaluator.UintValueEvaluator(stringValue, parameters, false, (uint uintValue, uint uintParam, uint index) => uintValue == uintParam))
					{
						return true;
					}
					return DeviceLayoutValidatorExpressionEvaluator.StringValueEvaluator(stringValue, parameters, false, (string strValue, string strParam, uint index) => strValue.Equals(strParam, StringComparison.OrdinalIgnoreCase));
				}
			}
		};

		// Token: 0x0200000B RID: 11
		private struct ExpressionSyntax
		{
			// Token: 0x04000047 RID: 71
			public Regex RegularExpression;

			// Token: 0x04000048 RID: 72
			public Func<string, string[], bool> Evaluator;
		}
	}
}
