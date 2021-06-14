using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;
using Microsoft.CompPlat.PkgBldr.Interfaces;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	internal class Assembly : PkgPlugin
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override void ConvertEntries(XElement ToPkg, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement FromCsi)
		{
			if (enviorn.AutoGenerateOutput)
			{
				Console.WriteLine("error: auto file generation not supported for csi2pkg conversions");
				return;
			}
			MyContainter myContainter = new MyContainter();
			XmlReader macros = XmlReader.Create(PkgGenResources.GetResourceStream("Macros_Policy.xml"));
			myContainter.Security = new Security();
			myContainter.Security.LoadPolicyMacros(macros);
			enviorn.Macros = new MacroResolver();
			enviorn.Macros.Load(XmlReader.Create(PkgGenResources.GetResourceStream("Macros_CsiToPkg.xml")));
			enviorn.Pass = BuildPass.PLUGIN_PASS;
			enviorn.arg = myContainter;
			XElement xelement = new XElement(ToPkg.Name.Namespace + "Components");
			XElement xelement2 = new XElement(ToPkg.Name.Namespace + "OSComponent");
			XElement xelement3 = new XElement(ToPkg.Name.Namespace + "Files");
			XElement xelement4 = new XElement(ToPkg.Name.Namespace + "RegKeys");
			xelement2.Add(xelement3);
			xelement2.Add(xelement4);
			xelement.Add(xelement2);
			ToPkg.Add(xelement);
			myContainter.Files = xelement3;
			myContainter.RegKeys = xelement4;
			foreach (BuildPass pass in (BuildPass[])Enum.GetValues(typeof(BuildPass)))
			{
				enviorn.Pass = pass;
				base.ConvertEntries(ToPkg, plugins, enviorn, FromCsi);
			}
			if (myContainter.Security.HavePolicyData)
			{
				XElement xelement5 = Share.CreatePolicyXmlRoot(myContainter.Security.PolicyID);
				XElement xelement6 = new XElement(xelement5.Name.Namespace + "Rules");
				xelement5.Add(xelement6);
				this.AddPolicyData(xelement6, "File", myContainter.Security.FileACLs);
				this.AddPolicyData(xelement6, "Directory", myContainter.Security.DirACLs);
				this.AddPolicyData(xelement6, "RegKey", myContainter.Security.RegACLs);
				XDocument xdocument = new XDocument(new object[]
				{
					xelement5
				});
				string text = enviorn.Output;
				text = Regex.Replace(text, ".pkg.xml", ".policy.xml", RegexOptions.IgnoreCase);
				Console.WriteLine("writing {0}", text);
				xdocument.Save(text);
			}
			int num = myContainter.Files.Elements().Count<XElement>();
			int num2 = myContainter.RegKeys.Elements().Count<XElement>();
			int num3 = num + num2;
			if (num == 0)
			{
				myContainter.Files.Remove();
			}
			if (num2 == 0)
			{
				myContainter.RegKeys.Remove();
			}
			if (num3 == 0)
			{
				enviorn.ExitStatus = ExitStatus.SKIPPED;
				return;
			}
			enviorn.ExitStatus = ExitStatus.SUCCESS;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000022D8 File Offset: 0x000004D8
		private void AddPolicyData(XElement Rules, string RuleType, Dictionary<string, SDDL> SddlTable)
		{
			foreach (KeyValuePair<string, SDDL> keyValuePair in SddlTable)
			{
				string key = keyValuePair.Key;
				SDDL value = keyValuePair.Value;
				string text = null;
				if (key.StartsWith("$(", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("error: cant resolve policy path {0}", key);
				}
				else
				{
					XElement xelement = new XElement(Rules.Name.Namespace + RuleType);
					xelement.Add(new XAttribute("Path", key));
					if (value.Owner != null)
					{
						text = "O:" + value.Owner;
					}
					if (value.Group != null)
					{
						text = text + "G:" + value.Group;
					}
					if (text != null)
					{
						xelement.Add(new XAttribute("Owner", text));
					}
					if (value.Dacl != null)
					{
						xelement.Add(new XAttribute("DACL", value.Dacl));
					}
					if (value.Sacl != null)
					{
						xelement.Add(new XAttribute("SACL", value.Sacl));
					}
					string attributeHash = this.GetAttributeHash(key, RuleType, value);
					string elementId = this.GetElementId(key, RuleType);
					xelement.Add(new XAttribute("ElementID", elementId));
					xelement.Add(new XAttribute("AttributeHash", attributeHash));
					Rules.Add(xelement);
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002478 File Offset: 0x00000678
		private string GetAttributeHash(string Path, string RuleType, SDDL Ace)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(RuleType);
			stringBuilder.Append(Path);
			if (Ace.Owner != null)
			{
				stringBuilder.Append(Ace.Owner);
			}
			if (Ace.Group != null)
			{
				stringBuilder.Append(Ace.Group);
			}
			if (Ace.Dacl != null)
			{
				stringBuilder.Append(Ace.Dacl);
			}
			return Assembly.GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000024F0 File Offset: 0x000006F0
		private string GetElementId(string Path, string RuleType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(RuleType);
			stringBuilder.Append(Path);
			return Assembly.GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002528 File Offset: 0x00000728
		private static string GetSha256Hash(byte[] buffer)
		{
			return BitConverter.ToString(Assembly.Sha256Algorithm.ComputeHash(buffer)).Replace("-", string.Empty);
		}

		// Token: 0x04000001 RID: 1
		private static readonly HashAlgorithm Sha256Algorithm = HashAlgorithm.Create("SHA256");
	}
}
