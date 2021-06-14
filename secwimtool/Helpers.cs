using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SecureWim
{
	// Token: 0x02000006 RID: 6
	internal class Helpers
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002888 File Offset: 0x00000A88
		public static void WriteUintToStream(uint value, Stream s)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			s.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000028A8 File Offset: 0x00000AA8
		public static void SeekStreamToCatalogStart(Stream s)
		{
			uint catalogSize = Helpers.GetCatalogSize(s);
			s.Seek((long)(-(long)((ulong)catalogSize + 4UL)), SeekOrigin.End);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000028CC File Offset: 0x00000ACC
		public static uint GetCatalogSize(Stream s)
		{
			byte[] array = new byte[4];
			s.Seek((long)(-(long)array.Length), SeekOrigin.End);
			s.Read(array, 0, array.Length);
			return BitConverter.ToUInt32(array, 0);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002900 File Offset: 0x00000B00
		public static bool HasSdiHeader(string sdiFile)
		{
			byte[] header = new byte[]
			{
				36,
				83,
				68,
				73,
				48,
				48,
				48,
				49
			};
			bool result;
			using (FileStream fileStream = File.OpenRead(sdiFile))
			{
				result = Helpers.HasHeader(fileStream, header);
			}
			return result;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000294C File Offset: 0x00000B4C
		public static bool HasWimHeader(string wimFile)
		{
			byte[] header = new byte[]
			{
				77,
				83,
				87,
				73,
				77,
				0,
				0,
				0
			};
			bool result;
			using (FileStream fileStream = File.OpenRead(wimFile))
			{
				result = Helpers.HasHeader(fileStream, header);
			}
			return result;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002998 File Offset: 0x00000B98
		public static void AddPadding(Stream s, uint padding)
		{
			while (s.Length % (long)((ulong)padding) != 0L)
			{
				s.WriteByte(0);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000029B0 File Offset: 0x00000BB0
		internal static Guid[] GetGuids(string[] idStrings)
		{
			Guid[] array = new Guid[idStrings.Length];
			for (int i = 0; i < idStrings.Length; i++)
			{
				if (idStrings[i].Length != 32)
				{
					throw new FormatException("Expected a raw byte string serial number.");
				}
				byte[] array2 = new byte[16];
				try
				{
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = Helpers.GetByte(idStrings[i].Substring(2 * j, 2));
					}
				}
				catch (FormatException ex)
				{
					throw new FormatException(ex.Message + " ID being parsed was: " + idStrings[i], ex);
				}
				array[i] = new Guid(array2);
			}
			return array;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A54 File Offset: 0x00000C54
		private static byte GetByte(string byteString)
		{
			return byte.Parse(byteString, NumberStyles.HexNumber);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002A64 File Offset: 0x00000C64
		private static bool HasHeader(Stream s, byte[] header)
		{
			byte[] array = new byte[header.Length];
			if (s.Length < (long)array.Length)
			{
				return false;
			}
			s.Seek(0L, SeekOrigin.Begin);
			s.Read(array, 0, array.Length);
			return header.SequenceEqual(array);
		}
	}
}
