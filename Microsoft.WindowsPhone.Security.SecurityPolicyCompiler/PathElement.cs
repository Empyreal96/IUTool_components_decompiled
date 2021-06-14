using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200003E RID: 62
	public abstract class PathElement : IPolicyElement
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000A08C File Offset: 0x0000828C
		// (set) Token: 0x0600020B RID: 523 RVA: 0x0000A094 File Offset: 0x00008294
		protected string ElementName { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000A09D File Offset: 0x0000829D
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000A0A5 File Offset: 0x000082A5
		protected bool WildcardSupport { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000A0AE File Offset: 0x000082AE
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000A0B6 File Offset: 0x000082B6
		[XmlAttribute(AttributeName = "Path")]
		public string Path { get; set; }

		// Token: 0x06000210 RID: 528 RVA: 0x00002A98 File Offset: 0x00000C98
		public PathElement()
		{
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000A0BF File Offset: 0x000082BF
		public virtual void Add(IXPathNavigable pathXmlElement)
		{
			this.AddAttributes((XmlElement)pathXmlElement);
			this.CompileAttributes();
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000A0D4 File Offset: 0x000082D4
		protected virtual void AddAttributes(IXPathNavigable pathXmlElement)
		{
			XmlElement xmlElement = (XmlElement)pathXmlElement;
			this.Path = xmlElement.GetAttribute("Path");
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000A0F9 File Offset: 0x000082F9
		protected virtual void CompileAttributes()
		{
			this.ResolveMacrosAndWildcard();
			this.Path = NormalizedString.Get(this.Path);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000A114 File Offset: 0x00008314
		private void ResolveMacrosAndWildcard()
		{
			if (this.WildcardSupport)
			{
				int num = this.Path.IndexOf("\\(*)", GlobalVariables.GlobalStringComparison);
				if (num == -1 || num != this.Path.Length - "\\(*)".Length)
				{
					throw new PolicyCompilerInternalException(string.Format(GlobalVariables.Culture, "Element {0}: Path does not have required wildcard", new object[]
					{
						this.ElementName
					}));
				}
				this.Path = this.Path.Substring(0, num + 1);
			}
			this.Path = GlobalVariables.ResolveMacroReference(this.Path, string.Format(GlobalVariables.Culture, "Element={0}, Attribute={1}", new object[]
			{
				this.ElementName,
				"Path"
			}));
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000A1CC File Offset: 0x000083CC
		public virtual void Print()
		{
			ReportingBase instance = ReportingBase.GetInstance();
			instance.XmlElementLine(ConstantStrings.IndentationLevel3, this.ElementName);
			instance.XmlAttributeLine(ConstantStrings.IndentationLevel4, "Path", this.Path);
		}
	}
}
