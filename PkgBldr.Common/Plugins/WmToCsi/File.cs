using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Plugins.WmToCsi
{
	// Token: 0x0200000C RID: 12
	[Export(typeof(IPkgPlugin))]
	internal class File : PkgPlugin
	{
		// Token: 0x0600003A RID: 58 RVA: 0x000048C0 File Offset: 0x00002AC0
		public override void ConvertEntries(XElement ToCsi, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromWm)
		{
			XElement xelement = new XElement(ToCsi.Name.Namespace + "file");
			string text = null;
			string text2 = null;
			foreach (XAttribute xattribute in FromWm.Attributes())
			{
				string localName = xattribute.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
				if (num <= 466561496U)
				{
					if (num != 128283678U)
					{
						if (num != 314371098U)
						{
							if (num == 466561496U)
							{
								if (localName == "source")
								{
									try
									{
										text = enviorn.Bld.BuildMacros.Resolve(xattribute.Value).TrimEnd(new char[]
										{
											'\\'
										});
									}
									catch
									{
									}
									if (text == null)
									{
										text = enviorn.Macros.Resolve(xattribute.Value).TrimEnd(new char[]
										{
											'\\'
										});
									}
									else
									{
										text = Environment.ExpandEnvironmentVariables(text);
									}
									string text3 = LongPath.GetDirectoryName(text);
									if (string.IsNullOrEmpty(text3) || text3.StartsWith(".", StringComparison.OrdinalIgnoreCase))
									{
										text3 = LongPath.GetDirectoryName(enviorn.Input);
										text3 = LongPath.Combine(text3, text);
										text = text3;
									}
								}
							}
						}
						else if (localName == "securityDescriptor")
						{
							xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
							xelement.Add(new XElement(ToCsi.Name.Namespace + "securityDescriptor", new XAttribute("name", xattribute.Value)));
						}
					}
					else if (localName == "destinationDir")
					{
						string text4 = enviorn.Macros.Resolve(xattribute.Value).TrimEnd(new char[]
						{
							'\\'
						});
						if (text4.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
						{
							text4 = "$(runtime.systemDrive)" + text4;
						}
						xelement.Add(new XAttribute("destinationPath", text4));
					}
				}
				else if (num <= 1463485645U)
				{
					if (num != 1200417740U)
					{
						if (num == 1463485645U)
						{
							if (localName == "buildFilter")
							{
								xelement.Add(new XAttribute("buildFilter", xattribute.Value));
							}
						}
					}
					else if (localName == "writeableType")
					{
						xelement.Add(new XAttribute("writeableType", xattribute.Value));
					}
				}
				else if (num != 2369371622U)
				{
					if (num == 3791641492U)
					{
						if (localName == "attributes")
						{
							xattribute.Value = enviorn.Macros.Resolve(xattribute.Value);
							xelement.Add(new XAttribute("attributes", xattribute.Value));
						}
					}
				}
				else if (localName == "name")
				{
					text2 = xattribute.Value;
				}
			}
			LongPath.GetFileName(text);
			string directoryName = LongPath.GetDirectoryName(text);
			string fileName = LongPath.GetFileName(text);
			string value = ".\\";
			xelement.Add(new XAttribute("importPath", directoryName));
			xelement.Add(new XAttribute("sourceName", fileName));
			xelement.Add(new XAttribute("sourcePath", value));
			string value2;
			if (text2 == null)
			{
				value2 = fileName;
			}
			else
			{
				value2 = text2;
			}
			xelement.Add(new XAttribute("name", value2));
			string attributeValue = PkgBldrHelpers.GetAttributeValue(FromWm.Parent, "buildFilter");
			if (attributeValue != null)
			{
				string attributeValue2 = PkgBldrHelpers.GetAttributeValue(xelement, "buildFilter");
				if (attributeValue2 == null)
				{
					xelement.Add(new XAttribute("buildFilter", attributeValue));
				}
				else
				{
					enviorn.Logger.LogWarning("ambiguous build filter '{0}' on file element", new object[]
					{
						attributeValue2
					});
					xelement.Attribute("buildFilter").Value = attributeValue;
				}
			}
			foreach (XElement xelement2 in FromWm.Elements())
			{
				PkgBldrHelpers.SetDefaultNameSpace(xelement2, ToCsi.Name.Namespace);
				xelement.Add(xelement2);
			}
			ToCsi.Add(xelement);
		}
	}
}
