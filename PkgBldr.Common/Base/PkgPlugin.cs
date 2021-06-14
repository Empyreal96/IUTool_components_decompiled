using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000029 RID: 41
	public abstract class PkgPlugin : IPkgPlugin
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00006D0A File Offset: 0x00004F0A
		public virtual string Name
		{
			get
			{
				return this.XmlElementName;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00006D14 File Offset: 0x00004F14
		public virtual string XmlElementName
		{
			get
			{
				return char.ToLowerInvariant(base.GetType().Name[0]).ToString() + base.GetType().Name.Substring(1);
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00006D5B File Offset: 0x00004F5B
		public virtual string XmlElementUniqueXPath
		{
			get
			{
				return this.XmlSchemaPath;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00006D63 File Offset: 0x00004F63
		public virtual string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00006D6A File Offset: 0x00004F6A
		public virtual string XmlSchemaNameSpace
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006D6D File Offset: 0x00004F6D
		public virtual bool Pass(BuildPass pass)
		{
			return pass == BuildPass.PLUGIN_PASS;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006D74 File Offset: 0x00004F74
		public virtual void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component)
		{
			if (plugins == null)
			{
				throw new ArgumentNullException("plugins");
			}
			if (enviorn == null)
			{
				throw new ArgumentNullException("enviorn");
			}
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			foreach (XElement xelement in component.Elements())
			{
				if (plugins.ContainsKey(xelement.Name.LocalName) && plugins[xelement.Name.LocalName].Pass(enviorn.Pass))
				{
					plugins[xelement.Name.LocalName].ConvertEntries(parent, plugins, enviorn, xelement);
				}
			}
		}

		// Token: 0x04000015 RID: 21
		internal static string BaseComponentSchemaPath = "PkgBldr.WM.XSD\\BasePlugins.xsd";
	}
}
