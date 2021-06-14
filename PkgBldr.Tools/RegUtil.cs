using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001C RID: 28
	public static class RegUtil
	{
		// Token: 0x06000119 RID: 281 RVA: 0x0000581B File Offset: 0x00003A1B
		private static string QuoteString(string input)
		{
			return "\"" + input.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000584B File Offset: 0x00003A4B
		private static string NormalizeValueName(string name)
		{
			if (name == "@")
			{
				return "@";
			}
			return RegUtil.QuoteString(name);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00005866 File Offset: 0x00003A66
		private static byte[] RegStringToBytes(string value)
		{
			return Encoding.Unicode.GetBytes(value);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00005874 File Offset: 0x00003A74
		public static RegValueType RegValueTypeForString(string strType)
		{
			foreach (FieldInfo fieldInfo in typeof(RegValueType).GetFields())
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false);
				if (customAttributes.Length == 1 && strType.Equals(((XmlEnumAttribute)customAttributes[0]).Name, StringComparison.OrdinalIgnoreCase))
				{
					return (RegValueType)fieldInfo.GetRawConstantValue();
				}
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unknown Registry value type: {0}", new object[]
			{
				strType
			}));
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000058FC File Offset: 0x00003AFC
		public static byte[] HexStringToByteArray(string hexString)
		{
			List<byte> list = new List<byte>();
			if (hexString != string.Empty)
			{
				foreach (string s in hexString.Split(new char[]
				{
					','
				}))
				{
					byte item = 0;
					if (!byte.TryParse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out item))
					{
						throw new IUException("Invalid hex string: {0}", new object[]
						{
							hexString
						});
					}
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000597C File Offset: 0x00003B7C
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static void ByteArrayToRegString(StringBuilder output, byte[] data, int maxOnALine = 2147483647)
		{
			int num = 0;
			int i = data.Length;
			while (i > 0)
			{
				int num2 = Math.Min(i, maxOnALine);
				string text = BitConverter.ToString(data, num, num2);
				text = text.Replace('-', ',');
				output.Append(text);
				num += num2;
				i -= num2;
				if (i > 0)
				{
					output.AppendLine(",\\");
				}
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000059D4 File Offset: 0x00003BD4
		public static void RegOutput(StringBuilder output, string name, IEnumerable<string> values)
		{
			string arg = RegUtil.NormalizeValueName(name);
			output.AppendFormat(";Value:{0}", string.Join(";", from x in values
			select x.Replace(";", "\\;")));
			output.AppendLine();
			output.AppendFormat("{0}=hex(7):", arg);
			RegUtil.ByteArrayToRegString(output, RegUtil.RegStringToBytes(string.Join("\0", values) + "\0\0"), RegUtil.BinaryLineLength / 3);
			output.AppendLine();
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005A68 File Offset: 0x00003C68
		public static void RegOutput(StringBuilder output, string name, string value, bool expandable)
		{
			string arg = RegUtil.NormalizeValueName(name);
			if (expandable)
			{
				output.AppendFormat(";Value:{0}", value);
				output.AppendLine();
				output.AppendFormat("{0}=hex(2):", arg);
				RegUtil.ByteArrayToRegString(output, RegUtil.RegStringToBytes(value + "\0"), RegUtil.BinaryLineLength / 3);
			}
			else
			{
				output.AppendFormat("{0}={1}", arg, RegUtil.QuoteString(value));
			}
			output.AppendLine();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00005ADC File Offset: 0x00003CDC
		public static void RegOutput(StringBuilder output, string name, ulong value)
		{
			string arg = RegUtil.NormalizeValueName(name);
			output.AppendFormat(";Value:0X{0:X16}", value);
			output.AppendLine();
			output.AppendFormat("{0}=hex(b):", arg);
			RegUtil.ByteArrayToRegString(output, BitConverter.GetBytes(value), int.MaxValue);
			output.AppendLine();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00005B30 File Offset: 0x00003D30
		public static void RegOutput(StringBuilder output, string name, uint value)
		{
			string arg = RegUtil.NormalizeValueName(name);
			output.AppendFormat("{0}=dword:{1:X8}", arg, value);
			output.AppendLine();
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00005B60 File Offset: 0x00003D60
		public static void RegOutput(StringBuilder output, string name, byte[] value)
		{
			string arg = RegUtil.NormalizeValueName(name);
			output.AppendFormat("{0}=hex:", arg);
			RegUtil.ByteArrayToRegString(output, value.ToArray<byte>(), RegUtil.BinaryLineLength / 3);
			output.AppendLine();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00005B9C File Offset: 0x00003D9C
		public static void RegOutput(StringBuilder output, string name, int type, byte[] value)
		{
			string arg = RegUtil.NormalizeValueName(name);
			output.AppendFormat("{0}=hex({1:x}):", arg, type);
			RegUtil.ByteArrayToRegString(output, value.ToArray<byte>(), RegUtil.BinaryLineLength / 3);
			output.AppendLine();
		}

		// Token: 0x04000054 RID: 84
		private const string c_strDefaultValueName = "@";

		// Token: 0x04000055 RID: 85
		private const int c_iBinaryStringLengthPerByte = 3;

		// Token: 0x04000056 RID: 86
		public const string c_strRegHeader = "Windows Registry Editor Version 5.00";

		// Token: 0x04000057 RID: 87
		public static int BinaryLineLength = 120;
	}
}
