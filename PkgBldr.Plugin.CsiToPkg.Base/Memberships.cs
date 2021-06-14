using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000009 RID: 9
	[Export(typeof(IPkgPlugin))]
	internal class Memberships : PkgPlugin
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002B1C File Offset: 0x00000D1C
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			XElement regKeys = ((MyContainter)enviorn.arg).RegKeys;
			foreach (XElement xelement in FromCsi.Descendants(FromCsi.Name.Namespace + "serviceData"))
			{
				string attributeValue = PkgBldrHelpers.GetAttributeValue(xelement, "name");
				if (attributeValue != null)
				{
					IEnumerable<XAttribute> enumerable = xelement.Attributes();
					XElement xelement2 = RegHelpers.PkgRegKey("$(hklm.services)\\" + attributeValue);
					foreach (XAttribute xattribute in enumerable)
					{
						if (xattribute.Value != null)
						{
							string text = xattribute.Name.LocalName;
							uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
							if (num <= 1825370775U)
							{
								if (num <= 1361572173U)
								{
									if (num != 879704937U)
									{
										if (num != 1235711905U)
										{
											if (num == 1361572173U)
											{
												if (text == "type")
												{
													text = xattribute.Value.ToLowerInvariant();
													if (!(text == "win32shareprocess"))
													{
														if (!(text == "win32ownprocess"))
														{
															if (!(text == "kerneldriver"))
															{
																if (!(text == "filesystemdriver"))
																{
																	Console.WriteLine("warning: unknown service type {0}", xattribute.Value);
																	continue;
																}
																xattribute.Value = "00000002";
															}
															else
															{
																xattribute.Value = "00000001";
															}
														}
														else
														{
															xattribute.Value = "00000010";
														}
													}
													else
													{
														xattribute.Value = "00000020";
													}
													XElement content = RegHelpers.PkgRegValue("Type", "REG_DWORD", xattribute.Value);
													xelement2.Add(content);
													continue;
												}
											}
										}
										else if (text == "dependOnGroup")
										{
											XElement content = RegHelpers.PkgRegValue("DependOnGroup", "REG_SZ", xattribute.Value);
											xelement2.Add(content);
											continue;
										}
									}
									else if (text == "description")
									{
										XElement content = RegHelpers.PkgRegValue("Description", "REG_SZ", xattribute.Value);
										xelement2.Add(content);
										continue;
									}
								}
								else if (num <= 1617529889U)
								{
									if (num != 1605967500U)
									{
										if (num == 1617529889U)
										{
											if (text == "objectName")
											{
												XElement content = RegHelpers.PkgRegValue("ObjectName", "REG_SZ", xattribute.Value);
												xelement2.Add(content);
												continue;
											}
										}
									}
									else if (text == "group")
									{
										XElement content = RegHelpers.PkgRegValue("Group", "REG_SZ", xattribute.Value);
										xelement2.Add(content);
										continue;
									}
								}
								else if (num != 1697318111U)
								{
									if (num == 1825370775U)
									{
										if (text == "sidType")
										{
											text = xattribute.Value.ToLowerInvariant();
											if (!(text == "none"))
											{
												if (!(text == "Restricted"))
												{
													if (!(text == "Unrestricted"))
													{
														Console.WriteLine("warning: unknown sidType");
														continue;
													}
													xattribute.Value = "00000001";
												}
												else
												{
													xattribute.Value = "00000003";
												}
											}
											else
											{
												xattribute.Value = "00000000";
											}
											XElement content = RegHelpers.PkgRegValue("ServiceSidType", "REG_DWORD", xattribute.Value);
											xelement2.Add(content);
											continue;
										}
									}
								}
								else if (text == "start")
								{
									text = xattribute.Value.ToLowerInvariant();
									if (!(text == "auto"))
									{
										if (!(text == "boot"))
										{
											if (!(text == "delayedAuto"))
											{
												if (!(text == "demand"))
												{
													if (!(text == "disabled"))
													{
														if (!(text == "system"))
														{
															Console.WriteLine("warning: unknown service start type {0}", xattribute.Value);
															continue;
														}
														xattribute.Value = "00000001";
													}
													else
													{
														xattribute.Value = "00000004";
													}
												}
												else
												{
													xattribute.Value = "00000003";
												}
											}
											else
											{
												xattribute.Value = "00000002";
											}
										}
										else
										{
											xattribute.Value = "00000000";
										}
									}
									else
									{
										xattribute.Value = "00000002";
									}
									XElement content = RegHelpers.PkgRegValue("Start", "REG_DWORD", xattribute.Value);
									xelement2.Add(content);
									continue;
								}
							}
							else if (num <= 2369371622U)
							{
								if (num != 1919609929U)
								{
									if (num != 1997937216U)
									{
										if (num == 2369371622U)
										{
											if (text == "name")
											{
												continue;
											}
										}
									}
									else if (text == "requiredPrivileges")
									{
										XElement content = RegHelpers.PkgRegValue("RequiredPrivileges", "REG_MULTI_SZ", xattribute.Value);
										xelement2.Add(content);
										continue;
									}
								}
								else if (text == "imagePath")
								{
									XElement content = RegHelpers.PkgRegValue("ImagePath", "REG_EXPAND_SZ", xattribute.Value);
									xelement2.Add(content);
									continue;
								}
							}
							else if (num <= 3197510277U)
							{
								if (num != 2516003219U)
								{
									if (num == 3197510277U)
									{
										if (text == "dependOnService")
										{
											XElement content = RegHelpers.PkgRegValue("DependOnService", "REG_MULTI_SZ", xattribute.Value);
											xelement2.Add(content);
											continue;
										}
									}
								}
								else if (text == "tag")
								{
									XElement content = RegHelpers.PkgRegValue("Tag", "REG_DWORD", xattribute.Value);
									xelement2.Add(content);
									continue;
								}
							}
							else if (num != 3489752662U)
							{
								if (num == 3653152826U)
								{
									if (text == "errorControl")
									{
										text = xattribute.Value.ToLowerInvariant();
										if (!(text == "ignore"))
										{
											if (!(text == "normal"))
											{
												if (!(text == "critical"))
												{
													Console.WriteLine("warning: unknown service errorContorl type {0}", xattribute.Value);
													continue;
												}
												xattribute.Value = "00000003";
											}
											else
											{
												xattribute.Value = "00000001";
											}
										}
										else
										{
											xattribute.Value = "00000000";
										}
										XElement content = RegHelpers.PkgRegValue("ErrorControl", "REG_DWORD", xattribute.Value);
										xelement2.Add(content);
										continue;
									}
								}
							}
							else if (text == "displayName")
							{
								XElement content = RegHelpers.PkgRegValue("DisplayName", "REG_SZ", xattribute.Value);
								xelement2.Add(content);
								continue;
							}
							Console.WriteLine("warning: unknown service attribute {0}", xattribute.Name.LocalName);
						}
					}
					base.ConvertEntries(xelement2, plugins, enviorn, xelement);
					regKeys.Add(xelement2);
				}
			}
		}
	}
}
