using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000005 RID: 5
	[Export(typeof(IPkgPlugin))]
	internal class ComServer : PkgPlugin
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002530 File Offset: 0x00000730
		public override void ConvertEntries(XElement toWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement fromPkg)
		{
			XElement parent = PkgBldrHelpers.AddIfNotFound(toWm, "COM");
			XElement xelement = PkgBldrHelpers.AddIfNotFound(parent, "servers");
			XElement xelement2 = PkgBldrHelpers.AddIfNotFound(parent, "interfaces");
			XElement xelement3 = new XElement(toWm.Name.Namespace + "inProcServer");
			xelement.Add(xelement3);
			ComData comData = new ComData();
			comData.InProcServer = xelement3;
			comData.Interfaces = xelement2;
			comData.InProcServerClasses = PkgBldrHelpers.AddIfNotFound(xelement3, "classes");
			comData.RegKeys = new XElement(toWm.Name.Namespace + "regKeys");
			comData.Files = new XElement(toWm.Name.Namespace + "files");
			object arg = enviorn.arg;
			enviorn.arg = comData;
			base.ConvertEntries(toWm, plugins, enviorn, fromPkg);
			if (comData.Files.HasElements)
			{
				enviorn.Bld.WM.Root.Add(comData.Files);
			}
			if (comData.RegKeys.HasElements)
			{
				enviorn.Bld.WM.Root.Add(comData.RegKeys);
			}
			this.AddInterfaces(toWm, enviorn, fromPkg);
			if (!xelement2.HasElements)
			{
				xelement2.Remove();
			}
			if (!xelement.HasElements)
			{
				xelement.Remove();
			}
			enviorn.arg = arg;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002680 File Offset: 0x00000880
		private void AddInterfaces(XElement toWm, Config env, XElement fromPkg)
		{
			ComData comData = (ComData)env.arg;
			XElement xelement = fromPkg.Element(fromPkg.Name.Namespace + "Interfaces");
			if (xelement == null)
			{
				return;
			}
			foreach (XElement xelement2 in xelement.Elements(fromPkg.Name.Namespace + "Interface"))
			{
				XElement xelement3 = new XElement(toWm.Name.Namespace + "interface");
				foreach (XAttribute xattribute in xelement2.Attributes())
				{
					xattribute.Value = env.Macros.Resolve(xattribute.Value);
					string localName = xattribute.Name.LocalName;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
					if (num <= 921221376U)
					{
						if (num != 266367750U)
						{
							if (num != 318476547U)
							{
								if (num == 921221376U)
								{
									if (localName == "Id")
									{
										xelement3.Add(new XAttribute("id", xattribute.Value));
										continue;
									}
								}
							}
							else if (localName == "NumMethods")
							{
								continue;
							}
						}
						else if (localName == "Name")
						{
							xelement3.Add(new XAttribute("name", xattribute.Value));
							continue;
						}
					}
					else
					{
						if (num > 3077825508U)
						{
							if (num != 3359262369U)
							{
								if (num != 3696905472U)
								{
									goto IL_26A;
								}
								if (!(localName == "ProxyStubClsId"))
								{
									goto IL_26A;
								}
							}
							else if (!(localName == "ProxyStubClsId32"))
							{
								goto IL_26A;
							}
							xelement3.Add(new XAttribute("proxyStubClsId", xattribute.Value));
							continue;
						}
						if (num != 1573770551U)
						{
							if (num == 3077825508U)
							{
								if (localName == "TypeLib")
								{
									PkgBldrHelpers.AddIfNotFound(xelement3, "typeLib").Add(new XAttribute("id", xattribute.Value));
									continue;
								}
							}
						}
						else if (localName == "Version")
						{
							PkgBldrHelpers.AddIfNotFound(xelement3, "typeLib").Add(new XAttribute("version", xattribute.Value));
							continue;
						}
					}
					IL_26A:
					env.Logger.LogWarning("invalid COM interface attribute {0}", new object[]
					{
						xattribute.Name.LocalName
					});
				}
				comData.Interfaces.Add(xelement3);
			}
		}
	}
}
