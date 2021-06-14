using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200003D RID: 61
	public class Chambers : IPolicyElement
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00009EE4 File Offset: 0x000080E4
		// (set) Token: 0x06000204 RID: 516 RVA: 0x00009EEC File Offset: 0x000080EC
		[XmlElement(ElementName = "Chamber")]
		public List<Chamber> ChamberCollection { get; set; }

		// Token: 0x06000206 RID: 518 RVA: 0x00009EF8 File Offset: 0x000080F8
		public void Add(IXPathNavigable chambersXmlElement)
		{
			XmlElement chambersXmlElement2 = (XmlElement)chambersXmlElement;
			this.AddElements(chambersXmlElement2);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009F14 File Offset: 0x00008114
		public void AddElements(IXPathNavigable chambersXmlElement)
		{
			XmlNodeList xmlNodeList = ((XmlElement)chambersXmlElement).SelectNodes("./WP_Policy:Chamber", GlobalVariables.NamespaceManager);
			if (xmlNodeList.Count > 0)
			{
				if (this.ChamberCollection == null)
				{
					this.ChamberCollection = new List<Chamber>();
				}
				foreach (object obj in xmlNodeList)
				{
					XmlElement chamberXmlElement = (XmlElement)obj;
					Chamber chamber = new Chamber();
					chamber.Add(chamberXmlElement);
					this.ChamberCollection.Add(chamber);
				}
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00009FB0 File Offset: 0x000081B0
		public string GetAllChamberProperties()
		{
			string text = string.Empty;
			if (this.ChamberCollection != null)
			{
				foreach (Chamber chamber in this.ChamberCollection)
				{
					text += chamber.Name;
				}
			}
			return text;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000A018 File Offset: 0x00008218
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, "Chamber");
			foreach (Chamber chamber in this.ChamberCollection)
			{
				instance.DebugLine(string.Empty);
				chamber.Print();
			}
		}
	}
}
