using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x02000011 RID: 17
	public class DependencySuppression
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00002E5C File Offset: 0x0000105C
		public bool IsFileSupressed(string targetName)
		{
			return this.IsFileSupressed("*", "*", targetName);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002E80 File Offset: 0x00001080
		public bool IsFileSupressed(string partitionName, string sourceName, string targetName)
		{
			bool result;
			lock (this)
			{
				bool flag2 = !this._binarySupressionTable.ContainsKey(partitionName);
				if (flag2)
				{
					bool flag3 = !this._binarySupressionTable.ContainsKey("*");
					if (flag3)
					{
						return false;
					}
					partitionName = "*";
				}
				Dictionary<string, HashSet<string>> dictionary = this._binarySupressionTable[partitionName];
				bool flag4 = !dictionary.ContainsKey(targetName);
				if (flag4)
				{
					result = false;
				}
				else
				{
					HashSet<string> hashSet = dictionary[targetName];
					bool flag5 = !hashSet.Contains(sourceName) && !hashSet.Contains("*");
					if (flag5)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002F4C File Offset: 0x0000114C
		public bool IsPackageSupressed(string partitionName, string packageName)
		{
			bool result;
			lock (this)
			{
				bool flag2 = !this._packageSupressionTable.ContainsKey(partitionName);
				if (flag2)
				{
					bool flag3 = !this._packageSupressionTable.ContainsKey("*");
					if (flag3)
					{
						return false;
					}
					partitionName = "*";
				}
				result = this._packageSupressionTable[partitionName].Contains(packageName);
			}
			return result;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002FD4 File Offset: 0x000011D4
		public bool IsPackageSupressed(string packageName)
		{
			return this.IsPackageSupressed("*", packageName);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002FF4 File Offset: 0x000011F4
		public DependencySuppression(string supressionFile)
		{
			this._supressionFile = supressionFile;
			lock (this)
			{
				using (StreamReader streamReader = new StreamReader(supressionFile))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.ToLowerInvariant().Trim();
						string[] array = text.Split(new char[]
						{
							','
						});
						bool flag2 = string.IsNullOrWhiteSpace(text) || text.StartsWith("#", StringComparison.OrdinalIgnoreCase) || (array.Count<string>() != 3 && array.Count<string>() != 4);
						if (!flag2)
						{
							string text2 = array[0].Trim();
							string partitionName = array[1].Trim();
							bool flag3 = text2.Equals("BIN", StringComparison.OrdinalIgnoreCase);
							if (flag3)
							{
								string sourceName = array[2].Trim();
								string targetName = array[3].Trim();
								this.AddBinarySupressionEntry(targetName, sourceName, partitionName);
							}
							else
							{
								bool flag4 = text2.Equals("PKG", StringComparison.OrdinalIgnoreCase);
								if (flag4)
								{
									string packageName = array[2].Trim();
									this.AddPackageSupressionEntry(packageName, partitionName);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003180 File Offset: 0x00001380
		private void AddPackageSupressionEntry(string packageName, string partitionName)
		{
			lock (this)
			{
				bool flag2 = !this._packageSupressionTable.ContainsKey(partitionName);
				if (flag2)
				{
					this._packageSupressionTable.Add(partitionName, new HashSet<string>());
				}
				HashSet<string> hashSet = this._packageSupressionTable[partitionName];
				bool flag3 = !hashSet.Contains(packageName);
				if (flag3)
				{
					hashSet.Add(packageName);
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003208 File Offset: 0x00001408
		private void AddBinarySupressionEntry(string targetName, string sourceName, string partitionName)
		{
			lock (this)
			{
				bool flag2 = !this._binarySupressionTable.ContainsKey(partitionName);
				if (flag2)
				{
					this._binarySupressionTable.Add(partitionName, new Dictionary<string, HashSet<string>>());
				}
				Dictionary<string, HashSet<string>> dictionary = this._binarySupressionTable[partitionName];
				bool flag3 = !dictionary.ContainsKey(targetName);
				if (flag3)
				{
					dictionary.Add(targetName, new HashSet<string>());
				}
				HashSet<string> hashSet = dictionary[targetName];
				hashSet.Add(sourceName);
			}
		}

		// Token: 0x04000078 RID: 120
		private const int SupressionType = 0;

		// Token: 0x04000079 RID: 121
		private const int PartitionColumn = 1;

		// Token: 0x0400007A RID: 122
		private const int SourceNameColumn = 2;

		// Token: 0x0400007B RID: 123
		private const int TargetNameColumn = 3;

		// Token: 0x0400007C RID: 124
		private const int PackageNameColumn = 2;

		// Token: 0x0400007D RID: 125
		private const string PackageSupression = "PKG";

		// Token: 0x0400007E RID: 126
		private const string BinarySupression = "BIN";

		// Token: 0x0400007F RID: 127
		private const string MatchAny = "*";

		// Token: 0x04000080 RID: 128
		private string _supressionFile;

		// Token: 0x04000081 RID: 129
		private readonly Dictionary<string, Dictionary<string, HashSet<string>>> _binarySupressionTable = new Dictionary<string, Dictionary<string, HashSet<string>>>();

		// Token: 0x04000082 RID: 130
		private readonly Dictionary<string, HashSet<string>> _packageSupressionTable = new Dictionary<string, HashSet<string>>();
	}
}
