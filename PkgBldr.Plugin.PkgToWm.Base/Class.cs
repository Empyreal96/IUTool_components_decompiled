using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000008 RID: 8
	[Export(typeof(IPkgPlugin))]
	internal class Class : PkgPlugin
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002A48 File Offset: 0x00000C48
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			ComData comData = (ComData)enviorn.arg;
			XElement xelement = new XElement(toWm.Name.Namespace + "classDefinition");
			comData.InProcServerClasses.Add(xelement);
			bool flag = true;
			foreach (XAttribute xattribute in fromPkg.Attributes())
			{
				xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
				string localName = xattribute.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
				if (num <= 2624714636U)
				{
					if (num <= 921221376U)
					{
						if (num != 524754527U)
						{
							if (num == 921221376U)
							{
								if (localName == "Id")
								{
									xelement.Add(new XAttribute("id", xattribute.Value));
								}
							}
						}
						else if (!(localName == "SkipInProcServer32"))
						{
						}
					}
					else if (num != 1573770551U)
					{
						if (num != 1725856265U)
						{
							if (num == 2624714636U)
							{
								if (localName == "ThreadingModel")
								{
									string text = Helpers.ComConvertThreading(xattribute.Value, enviorn.Logger);
									if (text != null)
									{
										xelement.Add(new XAttribute("threading", text));
										flag = false;
									}
								}
							}
						}
						else if (localName == "Description")
						{
							xelement.Add(new XAttribute("name", xattribute.Value));
						}
					}
					else if (localName == "Version")
					{
						string text2 = xattribute.Value;
						if (!text2.Contains('.'))
						{
							text2 += ".0";
						}
						xelement.Add(new XAttribute("version", text2));
					}
				}
				else if (num <= 3077825508U)
				{
					if (num != 2669841878U)
					{
						if (num == 3077825508U)
						{
							if (localName == "TypeLib")
							{
								xelement.Add(new XAttribute("typeLib", xattribute.Value));
							}
						}
					}
					else if (localName == "VersionIndependentProgId")
					{
						xelement.Add(new XAttribute("versionIndependentProgId", xattribute.Value));
					}
				}
				else if (num != 3946121697U)
				{
					if (num != 4196823611U)
					{
						if (num == 4243899896U)
						{
							if (localName == "ProgId")
							{
								xelement.Add(new XAttribute("progId", xattribute.Value));
							}
						}
					}
					else if (localName == "DefaultIcon")
					{
						xelement.Add(new XAttribute("defaultIcon", xattribute.Value));
					}
				}
				else if (localName == "AppId")
				{
					xelement.Add(new XAttribute("appId", xattribute.Value));
				}
			}
			if (flag)
			{
				enviorn.Logger.LogWarning("<Class> COM threading not specified, setting it to \"Both\"", new object[0]);
				xelement.Add(new XAttribute("threading", "Both"));
			}
		}
	}
}
