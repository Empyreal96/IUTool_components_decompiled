using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Tools;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000030 RID: 48
	public static class PkgBldrHelpers
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x0000721C File Offset: 0x0000541C
		public static XElement AddIfNotFound(XElement parent, string name)
		{
			XElement xelement = parent.Element(parent.Name.Namespace + name);
			if (xelement == null)
			{
				xelement = new XElement(parent.Name.Namespace + name);
				parent.Add(xelement);
			}
			return xelement;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007264 File Offset: 0x00005464
		public static void SetDefaultNameSpace(XElement Parent, XNamespace NameSpace)
		{
			if (Parent == null)
			{
				return;
			}
			foreach (XElement xelement in Parent.DescendantsAndSelf())
			{
				xelement.Name = NameSpace + xelement.Name.LocalName;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000072C8 File Offset: 0x000054C8
		[SuppressMessage("Microsoft.Design", "CA1045")]
		public static void ReplaceDefaultNameSpace(ref XElement parent, XNamespace oldNameSpace, XNamespace newNameSpace)
		{
			if (parent == null)
			{
				return;
			}
			foreach (XElement xelement in parent.DescendantsAndSelf())
			{
				if (xelement.Name.Namespace.NamespaceName == oldNameSpace.NamespaceName)
				{
					xelement.Name = newNameSpace + xelement.Name.LocalName;
				}
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00007348 File Offset: 0x00005548
		public static string GetAttributeValue(XElement element, string attributeName)
		{
			XAttribute xattribute = element.Attribute(attributeName);
			if (xattribute == null)
			{
				return null;
			}
			return xattribute.Value;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00007370 File Offset: 0x00005570
		[SuppressMessage("Microsoft.Design", "CA1011")]
		public static XElement GetFirstDecendant(XElement Parent, XName Name)
		{
			IEnumerable<XElement> source = Parent.Descendants(Name);
			if (source.Count<XElement>() == 0)
			{
				return null;
			}
			return source.First<XElement>();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00007398 File Offset: 0x00005598
		public static XElement FindMatchingAttribute(XElement Parent, string ElementName, string AttributeName, string AttributeValue)
		{
			XElement result = null;
			IEnumerable<XElement> enumerable = from el in Parent.Descendants(Parent.Name.Namespace + ElementName)
			where el.Attribute(AttributeName).Value.Equals(AttributeValue, StringComparison.OrdinalIgnoreCase)
			select el;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				result = enumerable.First<XElement>();
			}
			return result;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000073F8 File Offset: 0x000055F8
		public static IEnumerable<XElement> FindMatchingAttributes(XElement Parent, string ElementName, string AttributeName)
		{
			return from el in Parent.Descendants(Parent.Name.Namespace + ElementName)
			where el.Attribute(AttributeName) != null
			select el;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000743C File Offset: 0x0000563C
		public static XDocument XDocumentLoadFromLongPath(string path)
		{
			if (!LongPathFile.Exists(path))
			{
				throw new PkgGenException("Can't find path {0}", new object[]
				{
					path
				});
			}
			SafeFileHandle safeFileHandle = PkgBldrHelpers.CreateFile(path, 1U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
			XDocument result = XDocument.Load(new FileStream(safeFileHandle, FileAccess.Read));
			safeFileHandle.Close();
			return result;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007490 File Offset: 0x00005690
		public static void XDocumentSaveToLongPath(XDocument document, string path)
		{
			string directoryName = LongPath.GetDirectoryName(path);
			if (!LongPathDirectory.Exists(directoryName))
			{
				throw new PkgGenException("Can't find path {0}", new object[]
				{
					directoryName
				});
			}
			SafeFileHandle safeFileHandle = PkgBldrHelpers.CreateFile(path, 2U, 3U, IntPtr.Zero, 2U, 0U, IntPtr.Zero);
			FileStream stream = new FileStream(safeFileHandle, FileAccess.Write);
			document.Save(stream);
			safeFileHandle.Close();
		}

		// Token: 0x060000C9 RID: 201
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);
	}
}
