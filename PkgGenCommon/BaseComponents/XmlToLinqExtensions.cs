using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000054 RID: 84
	public static class XmlToLinqExtensions
	{
		// Token: 0x06000165 RID: 357 RVA: 0x000064A0 File Offset: 0x000046A0
		public static XElement ToXElement<T>(this T pkgObject) where T : PkgObject
		{
			XElement result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (TextWriter textWriter = new StreamWriter(memoryStream))
				{
					new XmlSerializer(pkgObject.GetType()).Serialize(textWriter, pkgObject);
					result = XElement.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
				}
			}
			return result;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00006520 File Offset: 0x00004720
		public static T FromXElement<T>(this XElement element)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(element.ToString())))
			{
				result = (T)((object)new XmlSerializer(typeof(T)).Deserialize(memoryStream));
			}
			return result;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000657C File Offset: 0x0000477C
		public static bool WithLocalAttribute<T>(this T source, string localName, XmlToLinqExtensions.WithEntityDelegate<XAttribute> del) where T : XElement
		{
			if (source.LocalAttribute(localName) != null)
			{
				del(source.LocalAttribute(localName));
				return true;
			}
			return false;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00006598 File Offset: 0x00004798
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

		// Token: 0x06000169 RID: 361 RVA: 0x000065E4 File Offset: 0x000047E4
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

		// Token: 0x0600016A RID: 362 RVA: 0x00006630 File Offset: 0x00004830
		public static IEnumerable<XElement> LocalElements<T>(this T source, string localName) where T : XContainer
		{
			IEnumerable<XElement> enumerable = from e in source.Elements()
			where e.Name.LocalName == localName
			select e;
			return enumerable ?? new List<XElement>();
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00006674 File Offset: 0x00004874
		public static IEnumerable<XElement> LocalDescendants<T>(this T source, string localName) where T : XContainer
		{
			IEnumerable<XElement> enumerable = from e in source.Descendants()
			where e.Name.LocalName == localName
			select e;
			return enumerable ?? new List<XElement>();
		}

		// Token: 0x02000098 RID: 152
		// (Invoke) Token: 0x0600033F RID: 831
		public delegate void WithEntityDelegate<T>(T entity) where T : XObject;
	}
}
