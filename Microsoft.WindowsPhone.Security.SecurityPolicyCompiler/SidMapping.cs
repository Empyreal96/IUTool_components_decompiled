using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000B RID: 11
	internal class SidMapping
	{
		// Token: 0x17000008 RID: 8
		public string this[string capabilityId]
		{
			get
			{
				if (this.sidDictionary != null && this.sidDictionary.ContainsKey(capabilityId))
				{
					return string.Format(GlobalVariables.Culture, "{0}-{1}", new object[]
					{
						"S-1-5-21-2702878673-795188819-444038987",
						1031U + this.sidDictionary[capabilityId] - 1U
					});
				}
				if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POLICY_COMPILER_TEST")))
				{
					return SidBuilder.BuildSidString("S-1-5-21-2702878673-795188819-444038987", HashCalculator.CalculateSha256Hash(capabilityId, true), 8);
				}
				string text = string.Format(GlobalVariables.Culture, "The capability Id {0} is not defined", new object[]
				{
					capabilityId
				});
				if (!string.IsNullOrEmpty(this.sidMappingFilePath))
				{
					text = text + " in capability list file " + this.sidMappingFilePath;
				}
				throw new PolicyCompilerInternalException(text);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026D4 File Offset: 0x000008D4
		private static SidMapping CreateInstance(StreamReader reader)
		{
			SidMapping sidMapping = new SidMapping();
			sidMapping.sidDictionary = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
			sidMapping.reverseSidDictionary = new Dictionary<uint, string>();
			uint num = 0U;
			uint num2 = 1U;
			char[] separator = new char[]
			{
				','
			};
			while (reader.Peek() >= 0)
			{
				string text = reader.ReadLine();
				num += 1U;
				if (!text.StartsWith(";", GlobalVariables.GlobalStringComparison) && !string.IsNullOrWhiteSpace(text))
				{
					string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length > 2 || array.Length < 1)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file is invalid. The line must start with semicolon for comment OR number, comma(,) and the capability Id.", new object[]
						{
							num
						}));
					}
					uint num3 = 0U;
					try
					{
						num3 = uint.Parse(array[0], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, GlobalVariables.Culture);
					}
					catch (FormatException originalException)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file is invalid. Unable to convert '{1}' to integer.", new object[]
						{
							num,
							array[0]
						}), originalException);
					}
					catch (OverflowException originalException2)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file is invalid. '{1}' is out of range of the integer type.", new object[]
						{
							num,
							array[0]
						}), originalException2);
					}
					if (num3 == 0U || num3 > 1750U)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file is invalid. '{1}' is out of range of 1 to {2}.", new object[]
						{
							num,
							num3,
							1750
						}));
					}
					if (num3 != num2)
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file is out of order. The expected index should be {1} instead of {2}.", new object[]
						{
							num,
							num2,
							num3
						}));
					}
					string text2 = array[1].Trim();
					if (!text2.EndsWith("PLACEHOLDER", StringComparison.InvariantCulture) && !text2.EndsWith("DONOTUSE", StringComparison.InvariantCulture) && sidMapping.sidDictionary.ContainsKey(text2))
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Line {0} in capability list file contains duplicate capability {1}. Remove all duplicate capabilities.", new object[]
						{
							num,
							text2
						}));
					}
					if (array.Length == 2)
					{
						sidMapping.sidDictionary[array[1].Trim()] = num2;
						sidMapping.reverseSidDictionary.Add(num2, array[1].Trim());
					}
					num2 += 1U;
				}
			}
			return sidMapping;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000293C File Offset: 0x00000B3C
		public static SidMapping CreateInstance(string sidMappingFilePath)
		{
			SidMapping sidMapping = null;
			using (StreamReader streamReader = new StreamReader(sidMappingFilePath))
			{
				sidMapping = SidMapping.CreateInstance(streamReader);
				sidMapping.sidMappingFilePath = sidMappingFilePath;
			}
			return sidMapping;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002980 File Offset: 0x00000B80
		public static void CompareToSnapshotMapping(SidMapping NewSidMapping)
		{
			string path = Environment.ExpandEnvironmentVariables("%_WINPHONEROOT%\\tools\\oak\\misc\\snapshotcapabilitylist.cfg");
			if (!File.Exists(path))
			{
				return;
			}
			foreach (KeyValuePair<uint, string> keyValuePair in SidMapping.CreateInstance(path).reverseSidDictionary)
			{
				if (!keyValuePair.Value.EndsWith("PLACEHOLDER", StringComparison.InvariantCulture))
				{
					string text;
					if (!NewSidMapping.reverseSidDictionary.TryGetValue(keyValuePair.Key, out text))
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Capability list file is missing slot {0}.", new object[]
						{
							keyValuePair.Key
						}));
					}
					if (!text.EndsWith("DONOTUSE", StringComparison.InvariantCulture) && !text.Equals(keyValuePair.Value))
					{
						throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Capability list file cannot reuse slots. Slot {0} was {1} and has been changed to {2}. If you wish to remove a capability rename it ID_CAP_DONOTUSE", new object[]
						{
							keyValuePair.Key,
							keyValuePair.Value,
							text
						}));
					}
				}
			}
		}

		// Token: 0x040000B1 RID: 177
		private Dictionary<string, uint> sidDictionary;

		// Token: 0x040000B2 RID: 178
		private Dictionary<uint, string> reverseSidDictionary;

		// Token: 0x040000B3 RID: 179
		private string sidMappingFilePath;
	}
}
