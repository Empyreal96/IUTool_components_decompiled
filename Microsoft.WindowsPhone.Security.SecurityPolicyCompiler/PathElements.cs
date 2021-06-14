using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x02000039 RID: 57
	public abstract class PathElements<T> : IPolicyElement where T : PathElement, new()
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001ED RID: 493 RVA: 0x00009AE7 File Offset: 0x00007CE7
		// (set) Token: 0x060001EE RID: 494 RVA: 0x00009AEF File Offset: 0x00007CEF
		[XmlIgnore]
		public List<T> PathElementCollection { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00009AF8 File Offset: 0x00007CF8
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x00009B00 File Offset: 0x00007D00
		protected string ElementName { get; set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00002A98 File Offset: 0x00000C98
		public PathElements()
		{
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00009B0C File Offset: 0x00007D0C
		public virtual void Add(IXPathNavigable pathElementsXmlElement)
		{
			XmlElement pathElementsXmlElement2 = (XmlElement)pathElementsXmlElement;
			this.AddElements(pathElementsXmlElement2);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00009B28 File Offset: 0x00007D28
		public virtual void AddElements(IXPathNavigable pathElementsXmlElement)
		{
			XmlNodeList xmlNodeList = ((XmlElement)pathElementsXmlElement).SelectNodes("./WP_Policy:" + this.ElementName, GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.PathElementCollection == null)
				{
					this.PathElementCollection = new List<T>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement pathXmlElement = (XmlElement)obj;
					T t = Activator.CreateInstance<T>();
					t.Add(pathXmlElement);
					this.PathElementCollection.Add(t);
				}
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00009BD4 File Offset: 0x00007DD4
		public string GetAllPaths()
		{
			string text = string.Empty;
			if (this.PathElementCollection != null)
			{
				foreach (T t in this.PathElementCollection)
				{
					text += t.Path;
				}
			}
			return text;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00009C44 File Offset: 0x00007E44
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, this.ElementName);
			foreach (T t in this.PathElementCollection)
			{
				instance.DebugLine(string.Empty);
				t.Print();
			}
		}
	}
}
