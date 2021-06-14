using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxApp
{
	// Token: 0x02000002 RID: 2
	[Export(typeof(IPkgPlugin))]
	public class InboxApp : PkgPlugin
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public InboxApp()
		{
			this.adapter = new InboxApp.InboxAppLibAdapter();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002063 File Offset: 0x00000263
		public override string Name
		{
			get
			{
				return "Inbox Application Component";
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000206A File Offset: 0x0000026A
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Source";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002071 File Offset: 0x00000271
		public override string XmlSchemaPath
		{
			get
			{
				return "Microsoft.WindowsPhone.ImageUpdate.InboxApp.InboxApp.Resources.Schema.xsd";
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002078 File Offset: 0x00000278
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			this.adapter.ValidateEntry(packageGenerator, componentEntry);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002087 File Offset: 0x00000287
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			return this.adapter.ProcessEntry(packageGenerator, componentEntry);
		}

		// Token: 0x04000001 RID: 1
		private InboxApp.IPkgPluginAdapter adapter;

		// Token: 0x02000003 RID: 3
		private interface IPkgPluginAdapter
		{
			// Token: 0x06000007 RID: 7
			void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry);

			// Token: 0x06000008 RID: 8
			IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry);
		}

		// Token: 0x02000004 RID: 4
		internal class InboxAppLibAdapter : InboxApp.IPkgPluginAdapter
		{
			// Token: 0x06000009 RID: 9 RVA: 0x00002098 File Offset: 0x00000298
			public void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
			{
				this.ValidateRequiredAttribute(componentEntry, "Source", "Source is required for InboxApp objects");
				this.ValidateRequiredAttribute(componentEntry, "ProvXML", "A provisioning xml file is required for InboxApp objects");
				this.ValidateAttributeValues(componentEntry, "InfuseIntoDataPartition", InboxApp.InboxAppLibAdapter.validInfuseIntoDataPartitionAttrValues, "The 'InfuseIntoDataPartition' attribute must be either 'true' or 'false'");
				this.ValidateAttributeValues(componentEntry, "Update", InboxApp.InboxAppLibAdapter.validUpdateAttrValues, "The 'Update' attribute must be either 'early' or 'normal'");
			}

			// Token: 0x0600000A RID: 10 RVA: 0x000020F4 File Offset: 0x000002F4
			public IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
			{
				this.GetInboxAttributes(packageGenerator, componentEntry);
				IInboxAppPackage inboxAppPackage = AppPackageFactory.CreateAppPackage(this._inboxAppParameters);
				LogUtil.Message("[InboxApp.ProcessEntry] Processing: {0}", new object[]
				{
					this._inboxAppParameters.ToString()
				});
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] app package type: \"{0}\" ", new object[]
				{
					inboxAppPackage.GetType()
				});
				inboxAppPackage.OpenPackage();
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] Successfully opened \"{0}\" ({1})", new object[]
				{
					this._sourceBasePath,
					inboxAppPackage.ToString()
				});
				IInboxAppManifest manifest = inboxAppPackage.GetManifest();
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] app manifest type: \"{0}\" ", new object[]
				{
					manifest.GetType()
				});
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] appManifest.Title: \"{0}\" ", new object[]
				{
					manifest.Title
				});
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] appManifest.Description: \"{0}\" ", new object[]
				{
					manifest.Description
				});
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] appManifest.Publisher: \"{0}\" ", new object[]
				{
					manifest.Publisher
				});
				IInboxAppToPkgObjectsMappingStrategy pkgObjectsMappingStrategy = inboxAppPackage.GetPkgObjectsMappingStrategy();
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] mapping strategy type: \"{0}\" ", new object[]
				{
					pkgObjectsMappingStrategy.GetType()
				});
				OSComponentBuilder osComponent = new OSComponentBuilder();
				List<PkgObject> list = pkgObjectsMappingStrategy.Map(inboxAppPackage, packageGenerator, osComponent);
				foreach (PkgObject pkgObject in list)
				{
					LogUtil.Diagnostic("[InboxApp.ProcessEntry] Added to package: {0}", new object[]
					{
						pkgObject.ToString()
					});
				}
				LogUtil.Diagnostic("[InboxApp.ProcessEntry] PkgObject count: {0}", new object[]
				{
					list.Count
				});
				return list;
			}

			// Token: 0x0600000B RID: 11 RVA: 0x00002288 File Offset: 0x00000488
			private void GetInboxAttributes(IPkgProject packageGenerator, XElement componentEntry)
			{
				XAttribute xattribute = componentEntry.LocalAttribute("Source");
				this._sourceBasePath = packageGenerator.MacroResolver.Resolve(xattribute.Value, MacroResolveOptions.ErrorOnUnknownMacro);
				LogUtil.Diagnostic("[InboxApp.ValidateEntry] {0}(BasePath) = \"{1}\"", new object[]
				{
					"Source",
					this._sourceBasePath
				});
				XAttribute xattribute2 = componentEntry.LocalAttribute("ProvXML");
				this._provXMLBasePath = packageGenerator.MacroResolver.Resolve(xattribute2.Value, MacroResolveOptions.ErrorOnUnknownMacro);
				LogUtil.Diagnostic("[InboxApp.ValidateEntry] {0}(BasePath) = \"{1}\"", new object[]
				{
					"ProvXML",
					this._provXMLBasePath
				});
				XAttribute xattribute3 = componentEntry.LocalAttribute("License");
				if (xattribute3 != null)
				{
					this._licenseBasePath = packageGenerator.MacroResolver.Resolve(xattribute3.Value, MacroResolveOptions.ErrorOnUnknownMacro);
					LogUtil.Diagnostic("[InboxApp.ValidateEntry] {0}(BasePath) = \"{1}\"", new object[]
					{
						"License",
						this._licenseBasePath
					});
				}
				XAttribute xattribute4 = componentEntry.LocalAttribute("InfuseIntoDataPartition");
				if (xattribute4 != null)
				{
					int num = InboxApp.InboxAppLibAdapter.validInfuseIntoDataPartitionAttrValues.IndexOf(xattribute4.Value.ToLower(CultureInfo.InvariantCulture).Trim());
					this._infuseIntoDataPartition = (num == 0);
					LogUtil.Diagnostic("[InboxApp.ValidateEntry] (OPTIONAL) {0} = \"{1}\"", new object[]
					{
						"InfuseIntoDataPartition",
						this._infuseIntoDataPartition.ToString()
					});
				}
				XAttribute xattribute5 = componentEntry.LocalAttribute("Update");
				if (xattribute5 != null)
				{
					int num2 = InboxApp.InboxAppLibAdapter.validUpdateAttrValues.IndexOf(xattribute5.Value.ToLower(CultureInfo.InvariantCulture).Trim());
					if (num2 != 0)
					{
						if (num2 != 1)
						{
							this._updateValue = UpdateType.UpdateNotNeeded;
						}
						else
						{
							this._updateValue = UpdateType.UpdateNormal;
						}
					}
					else
					{
						this._updateValue = UpdateType.UpdateEarly;
					}
					LogUtil.Diagnostic("[InboxApp.ValidateEntry] (OPTIONAL) {0} = \"{1}\"", new object[]
					{
						"Update",
						this._updateValue.ToString()
					});
				}
				if (this._infuseIntoDataPartition && this._updateValue != UpdateType.UpdateNotNeeded)
				{
					LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "Update is not supported while infusing to the Data Partition. Ignoring the Update attribute.", new object[0]));
					this._updateValue = UpdateType.UpdateNotNeeded;
				}
				string text = packageGenerator.MacroResolver.GetValue("PROVXMLTYPE");
				if (!string.IsNullOrWhiteSpace(text))
				{
					text = text.ToUpper(CultureInfo.InvariantCulture);
					if (text == "Microsoft".ToUpper(CultureInfo.InvariantCulture))
					{
						this._category = ProvXMLCategory.Microsoft;
					}
					else if (text == "Test".ToUpper(CultureInfo.InvariantCulture))
					{
						this._category = ProvXMLCategory.Test;
					}
					else if (!(text == "OEM".ToUpper(CultureInfo.InvariantCulture)))
					{
						throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "The value \"{0}\" is an invalid value for the parameter {1}. Valid values are: {2}", new object[]
						{
							text,
							"PROVXMLTYPE",
							string.Join(",", PkgGenConstants.ValidVariablePROVXMLTYPEValues)
						}));
					}
				}
				string workingBaseDir = Path.Combine(packageGenerator.TempDirectory, Path.GetFileName(Path.GetRandomFileName()));
				this._inboxAppParameters = new InboxAppParameters(this._sourceBasePath, this._licenseBasePath, this._provXMLBasePath, this._infuseIntoDataPartition, this._updateValue, this._category, workingBaseDir);
			}

			// Token: 0x0600000C RID: 12 RVA: 0x00002584 File Offset: 0x00000784
			private void ValidateRequiredAttribute(XElement componentEntry, string attributeName, string message)
			{
				XAttribute xattribute = componentEntry.LocalAttribute(attributeName);
				if (xattribute == null || string.IsNullOrWhiteSpace(xattribute.Value))
				{
					throw new PkgXmlException(componentEntry, message, new object[0]);
				}
			}

			// Token: 0x0600000D RID: 13 RVA: 0x000025B8 File Offset: 0x000007B8
			private void ValidateAttributeValues(XElement componentEntry, string attributeName, ReadOnlyCollection<string> validValues, string message)
			{
				XAttribute xattribute = componentEntry.LocalAttribute(attributeName);
				if (xattribute != null && validValues != null && validValues.Count > 0)
				{
					string value = xattribute.Value.ToLower(CultureInfo.InvariantCulture);
					if (!string.IsNullOrWhiteSpace(value) && !validValues.Contains(value))
					{
						throw new PkgXmlException(componentEntry, message, new object[0]);
					}
				}
			}

			// Token: 0x04000002 RID: 2
			private static readonly ReadOnlyCollection<string> validInfuseIntoDataPartitionAttrValues = PkgGenConstants.ValidAttrInfuseIntoDataPartitionValues;

			// Token: 0x04000003 RID: 3
			private static readonly ReadOnlyCollection<string> validUpdateAttrValues = PkgGenConstants.ValidAttrUpdateValues;

			// Token: 0x04000004 RID: 4
			private string _sourceBasePath = string.Empty;

			// Token: 0x04000005 RID: 5
			private string _licenseBasePath = string.Empty;

			// Token: 0x04000006 RID: 6
			private string _provXMLBasePath = string.Empty;

			// Token: 0x04000007 RID: 7
			private bool _infuseIntoDataPartition;

			// Token: 0x04000008 RID: 8
			private UpdateType _updateValue;

			// Token: 0x04000009 RID: 9
			private ProvXMLCategory _category;

			// Token: 0x0400000A RID: 10
			private InboxAppParameters _inboxAppParameters;
		}
	}
}
