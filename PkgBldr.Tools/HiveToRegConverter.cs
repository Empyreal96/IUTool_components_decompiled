using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000017 RID: 23
	public class HiveToRegConverter
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x000050EC File Offset: 0x000032EC
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public HiveToRegConverter(string hiveFile, string keyPrefix = null)
		{
			if (string.IsNullOrEmpty(hiveFile))
			{
				throw new ArgumentNullException("hiveFile");
			}
			if (!LongPathFile.Exists(hiveFile))
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Hive file {0} does not exist or cannot be read", new object[]
				{
					hiveFile
				}));
			}
			this.m_hiveFile = hiveFile;
			this.m_keyPrefix = keyPrefix;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005158 File Offset: 0x00003358
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public void ConvertToReg(string outputFile, HashSet<string> exclusions = null, bool append = false)
		{
			if (string.IsNullOrEmpty(outputFile))
			{
				throw new ArgumentNullException("outputFile");
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

		// Token: 0x060000F9 RID: 249 RVA: 0x000051D4 File Offset: 0x000033D4
		[SuppressMessage("Microsoft.Design", "CA1026")]
		[SuppressMessage("Microsoft.Design", "CA1045")]
		public void ConvertToReg(ref StringBuilder outputStr, HashSet<string> exclusions = null)
		{
			this.ConvertToReg(ref outputStr, null, true, exclusions);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000051E0 File Offset: 0x000033E0
		[SuppressMessage("Microsoft.Design", "CA1026")]
		[SuppressMessage("Microsoft.Design", "CA1045")]
		public void ConvertToReg(ref StringBuilder outputStr, string subKey, bool outputHeader, HashSet<string> exclusions = null)
		{
			if (outputStr == null)
			{
				throw new ArgumentNullException("outputStr");
			}
			if (exclusions != null)
			{
				this.m_exclusions.UnionWith(exclusions);
			}
			using (this.m_writer = new StringWriter(outputStr, CultureInfo.InvariantCulture))
			{
				this.ConvertToStream(outputHeader, subKey);
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005248 File Offset: 0x00003448
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

		// Token: 0x060000FC RID: 252 RVA: 0x000052B8 File Offset: 0x000034B8
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

		// Token: 0x060000FD RID: 253 RVA: 0x000053A4 File Offset: 0x000035A4
		private void WriteKeyName(string keyname)
		{
			this.m_writer.WriteLine();
			this.m_writer.WriteLine("[{0}]", keyname);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000053C4 File Offset: 0x000035C4
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

		// Token: 0x060000FF RID: 255 RVA: 0x00005434 File Offset: 0x00003634
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

		// Token: 0x06000100 RID: 256 RVA: 0x000055A4 File Offset: 0x000037A4
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

		// Token: 0x06000101 RID: 257 RVA: 0x00005646 File Offset: 0x00003846
		private string GetExpandStringValueAsComments(string value)
		{
			return string.Format(CultureInfo.InvariantCulture, ";Value={0}", new object[]
			{
				value
			});
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005664 File Offset: 0x00003864
		private void WriteKeyContents(ORRegistryKey key)
		{
			this.WriteKeyName(key.FullName);
			string @class = key.Class;
			if (!string.IsNullOrEmpty(@class))
			{
				this.m_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, ";Class=\"{0}\"", new object[]
				{
					@class
				}));
			}
			foreach (string valueName in key.ValueNames.OrderBy((string x) => x, StringComparer.OrdinalIgnoreCase))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.FormatValueName(valueName));
				stringBuilder.Append(this.FormatValue(key, valueName));
				this.m_writer.WriteLine(stringBuilder.ToString());
			}
		}

		// Token: 0x04000043 RID: 67
		private HashSet<string> m_exclusions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000044 RID: 68
		private string m_keyPrefix;

		// Token: 0x04000045 RID: 69
		private string m_hiveFile;

		// Token: 0x04000046 RID: 70
		private TextWriter m_writer;
	}
}
