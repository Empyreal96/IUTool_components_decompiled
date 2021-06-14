using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.MCSF.Offline
{
	// Token: 0x02000002 RID: 2
	internal static class Extensions
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static XAttribute LocalAttribute<T>(this T source, string localName) where T : XElement
		{
			IEnumerable<XAttribute> enumerable = from a in source.Attributes()
			where a.Name.LocalName == localName
			select a;
			if (enumerable != null && enumerable.Count<XAttribute>() > 0)
			{
				return enumerable.First<XAttribute>();
			}
			return null;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000209C File Offset: 0x0000029C
		public static XElement LocalElement<T>(this T source, string localName) where T : XContainer
		{
			IEnumerable<XElement> enumerable = from e in source.Elements()
			where e.Name.LocalName == localName
			select e;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				return enumerable.First<XElement>();
			}
			return null;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E8 File Offset: 0x000002E8
		public static IEnumerable<XElement> LocalElements<T>(this T source, string localName) where T : XContainer
		{
			IEnumerable<XElement> enumerable = from e in source.Elements()
			where e.Name.LocalName == localName
			select e;
			return enumerable ?? new List<XElement>();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002129 File Offset: 0x00000329
		public static uint ParseInt(string intString)
		{
			if (intString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				return uint.Parse(intString.Substring(2), NumberStyles.HexNumber);
			}
			return uint.Parse(intString);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002151 File Offset: 0x00000351
		public static int ParseSignedInt(string intString, int defaultValue)
		{
			if (intString != null)
			{
				return int.Parse(intString);
			}
			return defaultValue;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000215E File Offset: 0x0000035E
		public static int ParseSignedInt(string intString)
		{
			if (intString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				return int.Parse(intString.Substring(2), NumberStyles.HexNumber);
			}
			return int.Parse(intString);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002186 File Offset: 0x00000386
		public static long ParseSignedInt64(string intString)
		{
			if (intString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
				return long.Parse(intString.Substring(2), NumberStyles.HexNumber);
			}
			return long.Parse(intString);
		}
	}
}
