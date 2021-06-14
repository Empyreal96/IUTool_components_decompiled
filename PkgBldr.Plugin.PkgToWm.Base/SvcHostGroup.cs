using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000017 RID: 23
	[Export(typeof(IPkgPlugin))]
	internal class SvcHostGroup : PkgPlugin
	{
		// Token: 0x0600004A RID: 74 RVA: 0x0000562C File Offset: 0x0000382C
		public override void ConvertEntries(XElement ToWm, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromPkg)
		{
			XElement xelement = PkgBldrHelpers.AddIfNotFound(ToWm, "regKeys");
			XElement xelement2 = new XElement(xelement.Name.Namespace + "regKey");
			string text = PkgBldrHelpers.GetAttributeValue(FromPkg, "Name");
			text = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\SvcHost\\" + text;
			xelement2.Add(new XAttribute("keyName", text));
			xelement.Add(xelement2);
			text = PkgBldrHelpers.GetAttributeValue(FromPkg, "CoInitializeSecurityParam");
			if (!string.IsNullOrEmpty(text))
			{
				XElement xelement3 = new XElement(xelement.Name.Namespace + "regValue");
				string text2 = int.Parse(text).ToString("x");
				text2 = text2.PadLeft(8, '0');
				text2 = "0x" + text2;
				xelement3.Add(new XAttribute("name", "CoInitializeSecurityParam"));
				xelement3.Add(new XAttribute("type", "REG_DWORD"));
				xelement3.Add(new XAttribute("value", text2));
				xelement2.Add(xelement3);
			}
			text = PkgBldrHelpers.GetAttributeValue(FromPkg, "CoInitializeSecurityAllowLowBox");
			if (!string.IsNullOrEmpty(text))
			{
				XElement xelement4 = new XElement(xelement.Name.Namespace + "regValue");
				string text3 = int.Parse(text).ToString("x");
				text3 = text3.PadLeft(8, '0');
				text3 = "0x" + text3;
				xelement4.Add(new XAttribute("name", "CoInitializeSecurityAllowLowBox"));
				xelement4.Add(new XAttribute("type", "REG_DWORD"));
				xelement4.Add(new XAttribute("value", text3));
				xelement2.Add(xelement4);
			}
			text = PkgBldrHelpers.GetAttributeValue(FromPkg, "ImpersonationLevel");
			if (!string.IsNullOrEmpty(text))
			{
				if (text != "Identify")
				{
					enviorn.Logger.LogWarning("Skipping SvcHostGroup entry because ImpersonationLevel != Identify", new object[0]);
				}
				else
				{
					XElement xelement5 = new XElement(xelement.Name.Namespace + "regValue");
					xelement5.Add(new XAttribute("name", "ImpersonationLevel"));
					xelement5.Add(new XAttribute("type", "REG_DWORD"));
					xelement5.Add(new XAttribute("value", "0x00000002"));
					xelement2.Add(xelement5);
				}
			}
			text = PkgBldrHelpers.GetAttributeValue(FromPkg, "AuthenticationCapabilities");
			if (!string.IsNullOrEmpty(text))
			{
				XElement xelement6 = new XElement(xelement.Name.Namespace + "regValue");
				xelement6.Add(new XAttribute("name", "AuthenticationCapabilities"));
				xelement6.Add(new XAttribute("type", "REG_DWORD"));
				string[] array = text.Split(new char[]
				{
					' '
				});
				SvcHostGroup.AuthenticationCapabitities authenticationCapabitities = SvcHostGroup.AuthenticationCapabitities.None;
				string[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					string text4 = array2[i];
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text4);
					if (num <= 706511090U)
					{
						if (num <= 433860734U)
						{
							if (num != 78439758U)
							{
								if (num != 139015802U)
								{
									if (num != 433860734U)
									{
										goto IL_607;
									}
									if (!(text4 == "Default"))
									{
										goto IL_607;
									}
									authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.Default;
								}
								else
								{
									if (!(text4 == "SecureRefs"))
									{
										goto IL_607;
									}
									authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.SecureRefs;
								}
							}
							else
							{
								if (!(text4 == "Dynamic"))
								{
									goto IL_607;
								}
								authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.Dynamic;
							}
						}
						else if (num <= 504234540U)
						{
							if (num != 488350910U)
							{
								if (num != 504234540U)
								{
									goto IL_607;
								}
								if (!(text4 == "DisableAAA"))
								{
									goto IL_607;
								}
								authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.DisableAAA;
							}
							else
							{
								if (!(text4 == "AnyAuthority"))
								{
									goto IL_607;
								}
								authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.AnyAuthority;
							}
						}
						else if (num != 548085825U)
						{
							if (num != 706511090U)
							{
								goto IL_607;
							}
							if (!(text4 == "RequireFullSIC"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.RequireFullSIC;
						}
						else
						{
							if (!(text4 == "NoCustomMarshal"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.NoCustomMarshal;
						}
					}
					else if (num <= 2814147527U)
					{
						if (num <= 818401559U)
						{
							if (num != 810547195U)
							{
								if (num != 818401559U)
								{
									goto IL_607;
								}
								if (!(text4 == "AutoImpersonate"))
								{
									goto IL_607;
								}
								authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.AutoImpersonate;
							}
							else
							{
								if (!(text4 == "None"))
								{
									goto IL_607;
								}
								authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.None;
							}
						}
						else if (num != 1610031932U)
						{
							if (num != 2814147527U)
							{
								goto IL_607;
							}
							if (!(text4 == "StaticCloaking"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.StaticCloaking;
						}
						else
						{
							if (!(text4 == "AccessControl"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.AccessControl;
						}
					}
					else if (num <= 3476141839U)
					{
						if (num != 3446456898U)
						{
							if (num != 3476141839U)
							{
								goto IL_607;
							}
							if (!(text4 == "MakeFullSIC"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.MakeFullSIC;
						}
						else
						{
							if (!(text4 == "DynamicCloaking"))
							{
								goto IL_607;
							}
							authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.DynamicCloaking;
						}
					}
					else if (num != 3946121697U)
					{
						if (num != 4120386499U)
						{
							goto IL_607;
						}
						if (!(text4 == "MutualAuth"))
						{
							goto IL_607;
						}
						authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.MutualAuth;
					}
					else
					{
						if (!(text4 == "AppId"))
						{
							goto IL_607;
						}
						authenticationCapabitities |= SvcHostGroup.AuthenticationCapabitities.AppId;
					}
					IL_624:
					i++;
					continue;
					IL_607:
					enviorn.Logger.LogWarning(string.Format("Unknown Authentication Capability {0}", text4), new object[0]);
					goto IL_624;
				}
				text = string.Format("0x{0:X8}", (int)authenticationCapabitities);
				xelement6.Add(new XAttribute("value", text));
				xelement2.Add(xelement6);
			}
		}

		// Token: 0x0200002C RID: 44
		[Flags]
		private enum AuthenticationCapabitities
		{
			// Token: 0x04000016 RID: 22
			None = 0,
			// Token: 0x04000017 RID: 23
			MutualAuth = 1,
			// Token: 0x04000018 RID: 24
			StaticCloaking = 32,
			// Token: 0x04000019 RID: 25
			DynamicCloaking = 64,
			// Token: 0x0400001A RID: 26
			AnyAuthority = 128,
			// Token: 0x0400001B RID: 27
			MakeFullSIC = 256,
			// Token: 0x0400001C RID: 28
			Default = 2048,
			// Token: 0x0400001D RID: 29
			SecureRefs = 2,
			// Token: 0x0400001E RID: 30
			AccessControl = 4,
			// Token: 0x0400001F RID: 31
			AppId = 8,
			// Token: 0x04000020 RID: 32
			Dynamic = 16,
			// Token: 0x04000021 RID: 33
			RequireFullSIC = 512,
			// Token: 0x04000022 RID: 34
			AutoImpersonate = 1024,
			// Token: 0x04000023 RID: 35
			NoCustomMarshal = 8192,
			// Token: 0x04000024 RID: 36
			DisableAAA = 4096
		}
	}
}
