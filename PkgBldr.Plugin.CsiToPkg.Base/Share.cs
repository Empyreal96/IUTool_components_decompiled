using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000012 RID: 18
	public static class Share
	{
		// Token: 0x06000046 RID: 70 RVA: 0x0000404C File Offset: 0x0000224C
		public static XElement CreatePolicyXmlRoot(string PolicyID)
		{
			XElement xelement = new XElement("urn:Microsoft.WindowsPhone/PhoneSecurityPolicyInternal.v8.00" + "PhoneSecurityPolicy");
			xelement.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
			xelement.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
			xelement.Add(new XAttribute("PackageID", PolicyID));
			xelement.Add(new XAttribute("Description", "Mobile Core Policy"));
			xelement.Add(new XAttribute("Vendor", "Microsoft"));
			xelement.Add(new XAttribute("RequiredOSVersion", "8.00"));
			xelement.Add(new XAttribute("FileVersion", "8.00"));
			xelement.Add(new XAttribute("HashType", "Sha2"));
			return xelement;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004148 File Offset: 0x00002348
		public static void MergeNewPkgRegKey(XElement RegKeys, XElement RegKey)
		{
			string attributeValue = PkgBldrHelpers.GetAttributeValue(RegKey, "KeyName");
			XElement xelement = PkgBldrHelpers.FindMatchingAttribute(RegKeys, "RegKey", "KeyName", attributeValue);
			if (xelement != null)
			{
				using (IEnumerator<XElement> enumerator = RegKey.Elements(RegKeys.Name.Namespace + "RegValue").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xelement2 = enumerator.Current;
						string attributeValue2 = PkgBldrHelpers.GetAttributeValue(xelement2, "Name");
						if (PkgBldrHelpers.FindMatchingAttribute(xelement, "RegValue", "Name", attributeValue2) != null)
						{
							Console.WriteLine("error: duplicate RegValue Name {0}", attributeValue2);
						}
						else
						{
							xelement.Add(xelement2);
						}
					}
					return;
				}
			}
			RegKeys.Add(RegKey);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004204 File Offset: 0x00002404
		public static Share.PhoneIdentity CsiNameToPhoneIdentity(string csiName)
		{
			Share.PhoneIdentity phoneIdentity = new Share.PhoneIdentity();
			string text = null;
			Regex regex = new Regex("_lang_([A-Za-z]+\\-[A-Za-z]+)", RegexOptions.IgnoreCase);
			Match match = regex.Match(csiName);
			if (match.Success)
			{
				text = match.Value;
				csiName = regex.Replace(csiName, "");
			}
			csiName = Regex.Replace(csiName, "-+", ".");
			csiName = Regex.Replace(csiName, "_+", ".");
			csiName = Regex.Replace(csiName, "microsoft", "", RegexOptions.IgnoreCase);
			csiName = Regex.Replace(csiName, "windows", "", RegexOptions.IgnoreCase);
			csiName = Regex.Replace(csiName, "package", "", RegexOptions.IgnoreCase);
			csiName = Regex.Replace(csiName, "deployment", "", RegexOptions.IgnoreCase);
			csiName = Regex.Replace(csiName, "product", "", RegexOptions.IgnoreCase);
			csiName = csiName.Trim(new char[]
			{
				'.'
			});
			csiName = Regex.Replace(csiName, "\\.+", ".");
			phoneIdentity.Owner = "Microsoft";
			phoneIdentity.OwnerType = "Microsoft";
			string[] array = csiName.Split(new char[]
			{
				'.'
			});
			if (array.Length < 2)
			{
				phoneIdentity.SubComponent = csiName;
				phoneIdentity.Component = "OneCore";
				return phoneIdentity;
			}
			string text2 = "";
			phoneIdentity.Component = array[0];
			for (int i = 1; i < array.Length; i++)
			{
				text2 = text2 + array[i] + ".";
			}
			phoneIdentity.SubComponent = text2.Trim(new char[]
			{
				'.'
			});
			if (text != null)
			{
				Share.PhoneIdentity phoneIdentity2 = phoneIdentity;
				phoneIdentity2.SubComponent += text;
			}
			return phoneIdentity;
		}

		// Token: 0x02000014 RID: 20
		public class PhoneIdentity
		{
			// Token: 0x1700000E RID: 14
			// (get) Token: 0x0600004A RID: 74 RVA: 0x000043D8 File Offset: 0x000025D8
			// (set) Token: 0x0600004B RID: 75 RVA: 0x000043E0 File Offset: 0x000025E0
			public string Owner { get; set; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x0600004C RID: 76 RVA: 0x000043E9 File Offset: 0x000025E9
			// (set) Token: 0x0600004D RID: 77 RVA: 0x000043F1 File Offset: 0x000025F1
			public string OwnerType { get; set; }

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x0600004E RID: 78 RVA: 0x000043FA File Offset: 0x000025FA
			// (set) Token: 0x0600004F RID: 79 RVA: 0x00004402 File Offset: 0x00002602
			public string Component { get; set; }

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x06000050 RID: 80 RVA: 0x0000440B File Offset: 0x0000260B
			// (set) Token: 0x06000051 RID: 81 RVA: 0x00004413 File Offset: 0x00002613
			public string SubComponent { get; set; }
		}
	}
}
