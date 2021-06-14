using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000024 RID: 36
	[Export(typeof(IPkgPlugin))]
	internal class Service : PkgPlugin
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00007084 File Offset: 0x00005284
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = new XElement(ToWm.Name.Namespace + "service");
			foreach (XAttribute xattribute in FromPkg.Attributes())
			{
				xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
				string text = xattribute.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1725856265U)
				{
					if (num <= 266367750U)
					{
						if (num != 91525164U)
						{
							if (num != 182978943U)
							{
								if (num == 266367750U)
								{
									if (text == "Name")
									{
										xelement.Add(new XAttribute("name", xattribute.Value));
										continue;
									}
								}
							}
							else if (text == "Start")
							{
								string value = Helpers.lowerCamel(xattribute.Value);
								xelement.Add(new XAttribute("start", value));
								continue;
							}
						}
						else if (text == "Group")
						{
							xelement.Add(new XAttribute("group", xattribute.Value));
							continue;
						}
					}
					else if (num != 1182329114U)
					{
						if (num != 1463485645U)
						{
							if (num == 1725856265U)
							{
								if (text == "Description")
								{
									xelement.Add(new XAttribute("description", xattribute.Value));
									continue;
								}
							}
						}
						else if (text == "buildFilter")
						{
							string value2 = Helpers.ConvertBuildFilter(xattribute.Value);
							xelement.Add(new XAttribute("buildFilter", value2));
							continue;
						}
					}
					else if (text == "ErrorControl")
					{
						text = xattribute.Value;
						string value3;
						if (!(text == "Normal"))
						{
							if (!(text == "Critical"))
							{
								if (!(text == "Ignore"))
								{
									if (!(text == "Severe"))
									{
										enviorn.Logger.LogWarning("Windows manifest does not support service error type {0}, setting to normal", new object[]
										{
											xattribute.Value
										});
										value3 = "normal";
									}
									else
									{
										value3 = "critical";
										enviorn.Logger.LogWarning("Windows manifest does not support service error type severe, setting to critical", new object[0]);
									}
								}
								else
								{
									value3 = "ignore";
								}
							}
							else
							{
								value3 = "critical";
							}
						}
						else
						{
							value3 = "normal";
						}
						xelement.Add(new XAttribute("errorControl", value3));
						continue;
					}
				}
				else if (num <= 3512062061U)
				{
					if (num != 2404688805U)
					{
						if (num != 2860090561U)
						{
							if (num == 3512062061U)
							{
								if (text == "Type")
								{
									string value4 = Helpers.lowerCamel(xattribute.Value);
									xelement.Add(new XAttribute("type", value4));
									continue;
								}
							}
						}
						else if (text == "DependOnGroup")
						{
							xelement.Add(new XAttribute("dependOnGroup", xattribute.Value));
							continue;
						}
					}
					else if (text == "DependOnService")
					{
						xelement.Add(new XAttribute("dependOnService", xattribute.Value));
						continue;
					}
				}
				else if (num != 3653269462U)
				{
					if (num != 3996245565U)
					{
						if (num == 4176258230U)
						{
							if (text == "DisplayName")
							{
								xelement.Add(new XAttribute("displayName", xattribute.Value));
								continue;
							}
						}
					}
					else if (text == "SvcHostGroupName")
					{
						string value5 = "%SystemRoot%\\System32\\svchost.exe -k " + xattribute.Value;
						xelement.Add(new XAttribute("imagePath", value5));
						continue;
					}
				}
				else if (text == "IsTCB")
				{
					if (xattribute.Value.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
					{
						xelement.Add(new XAttribute("objectName", "LocalSystem"));
						continue;
					}
					continue;
				}
				enviorn.Logger.LogWarning("Unknown service type {0}", new object[]
				{
					xattribute.Name.LocalName
				});
			}
			base.ConvertEntries(xelement, plugins, enviorn, FromPkg);
			foreach (XElement xelement2 in xelement.Elements(xelement.Name.Namespace + "regKeys"))
			{
				ToWm.Add(xelement2);
				xelement2.Remove();
			}
			IEnumerable<XElement> enumerable = xelement.Elements(xelement.Name.Namespace + "files");
			foreach (XElement xelement3 in enumerable)
			{
				ToWm.Add(enumerable);
				enumerable.Remove<XElement>();
			}
			ToWm.Add(xelement);
		}
	}
}
