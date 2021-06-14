using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x0200000C RID: 12
	public class FileConverter
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002FBA File Offset: 0x000011BA
		public FileConverter(Config env)
		{
			if (env == null)
			{
				throw new ArgumentNullException("env");
			}
			this.m_config = env;
			this.m_macros = env.Macros;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002FE4 File Offset: 0x000011E4
		public XElement WmFile(XNamespace ns, XElement pkgElement)
		{
			if (pkgElement == null)
			{
				throw new ArgumentNullException("pkgElement");
			}
			XElement xelement = new XElement(ns + "file");
			string text = PkgBldrHelpers.GetAttributeValue(pkgElement, "DestinationDir");
			if (text == null)
			{
				text = "$(runtime.system32)";
			}
			else
			{
				text = this.m_macros.Resolve(text);
				if (text.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
				{
					text = "$(runtime.systemDrive)" + text;
				}
			}
			xelement.Add(new XAttribute("destinationDir", text));
			foreach (XAttribute xattribute in pkgElement.Attributes())
			{
				if (!(xattribute.Name.LocalName == "DestinationDir"))
				{
					if (xattribute.Name.LocalName != "Source")
					{
						xattribute.Value = this.m_macros.Resolve(xattribute.Value);
					}
					string localName = xattribute.Name.LocalName;
					if (!(localName == "Attributes"))
					{
						if (!(localName == "Name"))
						{
							if (!(localName == "Source"))
							{
								if (!(localName == "buildFilter"))
								{
									this.m_config.Logger.LogWarning("<File> attribute {0} not converted", new object[]
									{
										xattribute.Name.LocalName
									});
								}
								else
								{
									string value = Helpers.ConvertBuildFilter(xattribute.Value);
									xelement.Add(new XAttribute("buildFilter", value));
								}
							}
							else
							{
								string text2 = xattribute.Value.TrimEnd(new char[]
								{
									'\\'
								});
								if (!text2.Contains("\\"))
								{
									text2 = "$(RETAIL_BINARY_PATH)\\" + text2;
								}
								text2 = this.m_config.Macros.Resolve(text2);
								xelement.Add(new XAttribute("source", text2));
							}
						}
						else
						{
							xelement.Add(new XAttribute("name", xattribute.Value));
						}
					}
					else
					{
						string text3 = xattribute.Value.ToLowerInvariant();
						text3 = Regex.Replace(text3, "readonly", "readOnly");
						xelement.Add(new XAttribute("attributes", text3));
					}
				}
			}
			return xelement;
		}

		// Token: 0x04000006 RID: 6
		private MacroResolver m_macros;

		// Token: 0x04000007 RID: 7
		private Config m_config;

		// Token: 0x04000008 RID: 8
		public bool IsLangFile;
	}
}
