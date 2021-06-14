using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000016 RID: 22
	public class HiveToRegConverter
	{
		// Token: 0x060000CC RID: 204 RVA: 0x000065EA File Offset: 0x000047EA
		public HiveToRegConverter(string hiveFile)
		{
			this.VerifyHiveFileInput(hiveFile);
			this.m_hiveFile = hiveFile;
			this.m_keyPrefix = null;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00006617 File Offset: 0x00004817
		public HiveToRegConverter(string hiveFile, string keyPrefix)
		{
			this.VerifyHiveFileInput(hiveFile);
			this.m_hiveFile = hiveFile;
			this.m_keyPrefix = keyPrefix;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00006644 File Offset: 0x00004844
		public void VerifyHiveFileInput(string hiveFile)
		{
			if (string.IsNullOrEmpty(hiveFile))
			{
				throw new ArgumentNullException("hiveFile", "HiveFile cannot be null.");
			}
			if (!LongPathFile.Exists(hiveFile))
			{
				throw new FileNotFoundException(string.Format("Hive file {0} does not exist or cannot be read", hiveFile));
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006677 File Offset: 0x00004877
		public void ConvertToReg(string outputFile)
		{
			this.ConvertToReg(outputFile, null, false);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006682 File Offset: 0x00004882
		public void ConvertToReg(string outputFile, HashSet<string> exclusions)
		{
			this.ConvertToReg(outputFile, exclusions, false);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00006690 File Offset: 0x00004890
		public void ConvertToReg(string outputFile, HashSet<string> exclusions, bool append)
		{
			if (string.IsNullOrEmpty(outputFile))
			{
				throw new ArgumentNullException("outputFile", "Output file cannot be empty.");
			}
			if (exclusions != null)
			{
				this.m_exclusions.UnionWith(exclusions);
			}
			FileMode mode = append ? FileMode.Append : FileMode.Create;
			using (this.m_writer = new StreamWriter(LongPathFile.Open(outputFile, mode, FileAccess.Write), Encoding.Unicode))
			{
				this.ConvertToStream(!append, null);
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006710 File Offset: 0x00004910
		public void ConvertToReg(ref StringBuilder outputStr)
		{
			this.ConvertToReg(ref outputStr, null);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000671A File Offset: 0x0000491A
		public void ConvertToReg(ref StringBuilder outputStr, HashSet<string> exclusions)
		{
			this.ConvertToReg(ref outputStr, null, true, exclusions);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006726 File Offset: 0x00004926
		public void ConvertToReg(ref StringBuilder outputStr, string subKey, bool outputHeader)
		{
			this.ConvertToReg(ref outputStr, null, true, null);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00006734 File Offset: 0x00004934
		public void ConvertToReg(ref StringBuilder outputStr, string subKey, bool outputHeader, HashSet<string> exclusions)
		{
			if (outputStr == null)
			{
				throw new ArgumentNullException("outputStr");
			}
			if (exclusions != null)
			{
				this.m_exclusions.UnionWith(exclusions);
			}
			using (this.m_writer = new StringWriter(outputStr))
			{
				this.ConvertToStream(outputHeader, subKey);
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00006798 File Offset: 0x00004998
		private void ConvertToStream(bool outputHeader, string subKey)
		{
			if (outputHeader)
			{
				this.m_writer.WriteLine("Windows Registry Editor Version 5.00");
			}
			using (ORRegistryKey orregistryKey = ORRegistryKey.OpenHive(this.m_hiveFile, this.m_keyPrefix))
			{
				ORRegistryKey orregistryKey2 = orregistryKey;
				if (!string.IsNullOrEmpty(subKey))
				{
					orregistryKey2 = orregistryKey.OpenSubKey(subKey);
				}
				this.WriteKeyContents(orregistryKey2);
				this.WalkHive(orregistryKey2);
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00006808 File Offset: 0x00004A08
		private void WalkHive(ORRegistryKey root)
		{
			foreach (string subkeyname in root.SubKeys.OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase))
			{
				using (ORRegistryKey orregistryKey = root.OpenSubKey(subkeyname))
				{
					try
					{
						bool flag = this.m_exclusions.Contains(orregistryKey.FullName + "\\*");
						bool flag2 = this.m_exclusions.Contains(orregistryKey.FullName);
						if (!flag)
						{
							if (!flag2)
							{
								this.WriteKeyContents(orregistryKey);
							}
							this.WalkHive(orregistryKey);
						}
					}
					catch (Exception innerException)
					{
						throw new IUException("Failed to iterate through hive", innerException);
					}
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000068F4 File Offset: 0x00004AF4
		private void WriteKeyName(string keyname)
		{
			this.m_writer.WriteLine();
			this.m_writer.WriteLine("[{0}]", keyname);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006914 File Offset: 0x00004B14
		private string FormatValueName(string valueName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (valueName.Equals(""))
			{
				stringBuilder.Append("@=");
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder(valueName);
				stringBuilder2.Replace("\\", "\\\\").Replace("\"", "\\\"");
				stringBuilder.AppendFormat("\"{0}\"=", stringBuilder2.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006984 File Offset: 0x00004B84
		private string FormatValue(ORRegistryKey key, string valueName)
		{
			RegistryValueType valueKind = key.GetValueKind(valueName);
			StringBuilder stringBuilder = new StringBuilder();
			switch (valueKind)
			{
			case RegistryValueType.String:
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(key.GetStringValue(valueName));
				stringBuilder2.Replace("\\", "\\\\").Replace("\"", "\\\"");
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\"", new object[]
				{
					stringBuilder2.ToString()
				});
				goto IL_15C;
			}
			case RegistryValueType.DWord:
			{
				uint dwordValue = key.GetDwordValue(valueName);
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "dword:{0:X8}", new object[]
				{
					dwordValue
				});
				goto IL_15C;
			}
			case RegistryValueType.MultiString:
			{
				byte[] byteValue = key.GetByteValue(valueName);
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "hex(7):{0}", new object[]
				{
					OfflineRegUtils.ConvertByteArrayToRegStrings(byteValue)
				});
				string[] multiStringValue = key.GetMultiStringValue(valueName);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(this.GetMultiStringValuesAsComments(multiStringValue));
				goto IL_15C;
			}
			}
			byte[] byteValue2 = key.GetByteValue(valueName);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "hex({0,1:X}):{1}", new object[]
			{
				valueKind,
				OfflineRegUtils.ConvertByteArrayToRegStrings(byteValue2)
			});
			if (valueKind == RegistryValueType.ExpandString)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(this.GetExpandStringValueAsComments(key.GetStringValue(valueName)));
			}
			IL_15C:
			return stringBuilder.ToString();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006AF4 File Offset: 0x00004CF4
		private string GetMultiStringValuesAsComments(string[] values)
		{
			StringBuilder stringBuilder = new StringBuilder(500);
			int num = 80;
			if (values != null && values.Length != 0)
			{
				stringBuilder.Append(";Values=");
				int num2 = stringBuilder.Length;
				foreach (string text in values)
				{
					stringBuilder.AppendFormat("{0},", text);
					num2 += text.Length + 1;
					if (num2 > num)
					{
						stringBuilder.AppendLine();
						stringBuilder.Append(";");
						num2 = 1;
					}
				}
				stringBuilder.Replace(",", string.Empty, stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006B96 File Offset: 0x00004D96
		private string GetExpandStringValueAsComments(string value)
		{
			return string.Format(";Value={0}", value);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006BA4 File Offset: 0x00004DA4
		private void WriteKeyContents(ORRegistryKey key)
		{
			this.WriteKeyName(key.FullName);
			string @class = key.Class;
			if (!string.IsNullOrEmpty(@class))
			{
				this.m_writer.WriteLine(string.Format(";Class=\"{0}\"", @class));
			}
			foreach (string valueName in key.ValueNames.OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.FormatValueName(valueName));
				stringBuilder.Append(this.FormatValue(key, valueName));
				this.m_writer.WriteLine(stringBuilder.ToString());
			}
		}

		// Token: 0x04000042 RID: 66
		private HashSet<string> m_exclusions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000043 RID: 67
		private string m_keyPrefix;

		// Token: 0x04000044 RID: 68
		private string m_hiveFile;

		// Token: 0x04000045 RID: 69
		private TextWriter m_writer;
	}
}
