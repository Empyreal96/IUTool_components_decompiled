using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000021 RID: 33
	public struct VersionInfo
	{
		// Token: 0x0600015C RID: 348 RVA: 0x0000767C File Offset: 0x0000587C
		public VersionInfo(ushort major, ushort minor, ushort qfe, ushort build)
		{
			this.Major = major;
			this.Minor = minor;
			this.QFE = qfe;
			this.Build = build;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000769C File Offset: 0x0000589C
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				this.Major,
				this.Minor,
				this.QFE,
				this.Build
			});
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000076F1 File Offset: 0x000058F1
		public override int GetHashCode()
		{
			return this.Major.GetHashCode() ^ this.Minor.GetHashCode() ^ this.QFE.GetHashCode() ^ this.Build.GetHashCode();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00007722 File Offset: 0x00005922
		public override bool Equals(object obj)
		{
			return obj is VersionInfo && this == (VersionInfo)obj;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000773F File Offset: 0x0000593F
		public static bool operator ==(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			return versionInfo1.Major == versionInfo2.Major && versionInfo1.Minor == versionInfo2.Minor && versionInfo1.QFE == versionInfo2.QFE && versionInfo1.Build == versionInfo2.Build;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000777B File Offset: 0x0000597B
		public static bool operator !=(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			return !(versionInfo1 == versionInfo2);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00007788 File Offset: 0x00005988
		public static bool operator <(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			int[,] array = new int[4, 2];
			array[0, 0] = (int)versionInfo1.Major;
			array[0, 1] = (int)versionInfo2.Major;
			array[1, 0] = (int)versionInfo1.Minor;
			array[1, 1] = (int)versionInfo2.Minor;
			array[2, 0] = (int)versionInfo1.QFE;
			array[2, 1] = (int)versionInfo2.QFE;
			array[3, 0] = (int)versionInfo1.Build;
			array[3, 1] = (int)versionInfo2.Build;
			int[,] array2 = array;
			for (int i = 0; i < array2.GetLength(0); i++)
			{
				if (array2[i, 0] < array2[i, 1])
				{
					return true;
				}
				if (array2[i, 0] > array2[i, 1])
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00007848 File Offset: 0x00005A48
		public static bool operator <=(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			return versionInfo1 < versionInfo2 || versionInfo1 == versionInfo2;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000785C File Offset: 0x00005A5C
		public static bool operator >(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			return versionInfo2 < versionInfo1;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00007865 File Offset: 0x00005A65
		public static bool operator >=(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			return versionInfo1 > versionInfo2 || versionInfo1 == versionInfo2;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00007879 File Offset: 0x00005A79
		public static int Compare(VersionInfo versionInfo1, VersionInfo versionInfo2)
		{
			if (versionInfo1 < versionInfo2)
			{
				return -1;
			}
			if (versionInfo1 == versionInfo2)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00007894 File Offset: 0x00005A94
		public static bool TryParse(string versionInfoString, out VersionInfo versionInfo)
		{
			bool result;
			try
			{
				versionInfo = VersionInfo.Parse(versionInfoString);
				result = true;
			}
			catch (Exception)
			{
				versionInfo = default(VersionInfo);
				result = false;
			}
			return result;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000078D0 File Offset: 0x00005AD0
		public static VersionInfo Parse(string versionInfoString)
		{
			if (string.IsNullOrEmpty(versionInfoString))
			{
				throw new ArgumentNullException("versionInfoString is null or empty", null);
			}
			ArgumentException ex = new ArgumentException("The version info string must be in the form 'UINT16.UINT16.UINT16.UINT16'", versionInfoString);
			ex.Data.Add("versionInfoString", versionInfoString);
			string[] array = versionInfoString.Split(new char[]
			{
				'.'
			});
			if (array == null || array.Length != 4)
			{
				throw ex;
			}
			VersionInfo result = default(VersionInfo);
			if (!ushort.TryParse(array[0], out result.Major))
			{
				throw ex;
			}
			if (!ushort.TryParse(array[1], out result.Minor))
			{
				throw ex;
			}
			if (!ushort.TryParse(array[2], out result.QFE))
			{
				throw ex;
			}
			if (!ushort.TryParse(array[3], out result.Build))
			{
				throw ex;
			}
			return result;
		}

		// Token: 0x04000071 RID: 113
		public static readonly VersionInfo Empty;

		// Token: 0x04000072 RID: 114
		[XmlAttribute]
		public ushort Major;

		// Token: 0x04000073 RID: 115
		[XmlAttribute]
		public ushort Minor;

		// Token: 0x04000074 RID: 116
		[XmlAttribute]
		public ushort QFE;

		// Token: 0x04000075 RID: 117
		[XmlAttribute]
		public ushort Build;
	}
}
