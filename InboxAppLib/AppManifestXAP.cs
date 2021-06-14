using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000005 RID: 5
	public class AppManifestXAP : IInboxAppManifest
	{
		// Token: 0x0600001A RID: 26 RVA: 0x0000282C File Offset: 0x00000A2C
		public AppManifestXAP(string manifestBasePath)
		{
			this._manifestBasePath = InboxAppUtils.ValidateFileOrDir(manifestBasePath, false);
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000028A4 File Offset: 0x00000AA4
		public string Filename
		{
			get
			{
				return Path.GetFileName(this._manifestBasePath);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000028B1 File Offset: 0x00000AB1
		public string Title
		{
			get
			{
				return this._title;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000028B9 File Offset: 0x00000AB9
		public string Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000028C1 File Offset: 0x00000AC1
		public string Publisher
		{
			get
			{
				return this._publisher;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000028C9 File Offset: 0x00000AC9
		public List<string> Capabilities
		{
			get
			{
				return this._capabilities;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000028D1 File Offset: 0x00000AD1
		public string ProductID
		{
			get
			{
				return this._productID;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000028D9 File Offset: 0x00000AD9
		public string Version
		{
			get
			{
				return this._version;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000028E4 File Offset: 0x00000AE4
		public void ReadManifest()
		{
			XDocument node = XDocument.Load(this._manifestBasePath);
			LogUtil.Message("Parsing XAP Manifest: {0}", new object[]
			{
				this._manifestBasePath
			});
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			xmlNamespaceManager.AddNamespace("xap2009", "http://schemas.microsoft.com/windowsphone/2009/deployment");
			xmlNamespaceManager.AddNamespace("xap2012", "http://schemas.microsoft.com/windowsphone/2012/deployment");
			StringBuilder stringBuilder = new StringBuilder();
			string str = "/xap2009:";
			IEnumerable<XElement> source = node.XPathSelectElements(str + "Deployment/App", xmlNamespaceManager);
			if (source.Count<XElement>() == 0)
			{
				str = "/xap2012:";
				source = node.XPathSelectElements(str + "Deployment/App", xmlNamespaceManager);
			}
			if (source.Count<XElement>() > 0)
			{
				if (source.Count<XElement>() > 1)
				{
					LogUtil.Warning("XAP Manifest has {0} App nodes, only the first will be processed", new object[]
					{
						source.Count<XElement>()
					});
				}
				foreach (XAttribute xattribute in source.First<XElement>().Attributes())
				{
					object obj = xattribute.Name.ToString();
					string value = xattribute.Value;
					string a = obj.ToString().ToUpper(CultureInfo.InvariantCulture);
					if (!(a == "PRODUCTID"))
					{
						if (!(a == "TITLE"))
						{
							if (a == "VERSION")
							{
								this._version = xattribute.Value;
								LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting version: '{0}'", new object[]
								{
									this._version
								}));
								continue;
							}
							if (a == "PUBLISHER")
							{
								this._publisher = xattribute.Value;
								LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting publisher: '{0}'", new object[]
								{
									this._publisher
								}));
								continue;
							}
							if (!(a == "DESCRIPTION"))
							{
								LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "Ignoring '{0}'='{1}'", new object[]
								{
									xattribute.Name,
									xattribute.Value
								}));
								continue;
							}
							this._description = xattribute.Value;
							LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting description: '{0}'", new object[]
							{
								this._description
							}));
							continue;
						}
					}
					else
					{
						try
						{
							Guid guid = new Guid(value);
							guid.ToString();
							this._productID = value;
							LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting productID: '{0}'", new object[]
							{
								this._productID
							}));
							continue;
						}
						catch (FormatException)
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ProductID \"{0}\" is in an invalid GUID format.", new object[]
							{
								value
							}));
							continue;
						}
						catch (OverflowException)
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ProductID \"{0}\" is in an invalid GUID format.", new object[]
							{
								value
							}));
							continue;
						}
						catch (ArgumentNullException)
						{
							stringBuilder.AppendLine("ProductID is null or blank.");
							continue;
						}
					}
					this._title = xattribute.Value;
					LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Setting title: '{0}'", new object[]
					{
						this._title
					}));
				}
				foreach (XElement xelement in node.XPathSelectElements(str + "Deployment/App/Capabilities/Capability", xmlNamespaceManager))
				{
					foreach (XAttribute xattribute2 in xelement.Attributes())
					{
						if (xattribute2.Name.ToString().ToUpper(CultureInfo.InvariantCulture).Equals("NAME"))
						{
							if (!this._capabilities.Contains(xattribute2.Value))
							{
								LogUtil.Message(string.Format(CultureInfo.InvariantCulture, "Adding capability: {0}", new object[]
								{
									xattribute2.Value
								}));
								this._capabilities.Add(xattribute2.Value);
							}
							else
							{
								LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "Ignoring capability attribute named: {0}, value: {1}", new object[]
								{
									xattribute2.Name,
									xattribute2.Value
								}));
							}
						}
					}
				}
			}
			if (string.IsNullOrEmpty(this._productID))
			{
				stringBuilder.AppendLine("ProductID not defined in the manifest");
			}
			if (string.IsNullOrEmpty(this._title))
			{
				stringBuilder.AppendLine("Title is not defined in the manifest");
			}
			if (string.IsNullOrEmpty(this._version))
			{
				stringBuilder.AppendLine("Version is not defined in the manifest");
			}
			if (string.IsNullOrEmpty(this._publisher))
			{
				stringBuilder.AppendLine("Publisher is not defined in the manifest");
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				LogUtil.Error(stringBuilder.ToString());
				throw new InvalidDataException(stringBuilder.ToString());
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002E44 File Offset: 0x00001044
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "XAP manifest: (Filename): \"{0}\", (Title): \"{1}\", (ProductID): \"{2}\" ", new object[]
			{
				this.Filename,
				this.Title,
				this.ProductID
			});
		}

		// Token: 0x04000014 RID: 20
		private string _title = string.Empty;

		// Token: 0x04000015 RID: 21
		private string _description = string.Empty;

		// Token: 0x04000016 RID: 22
		private string _publisher = string.Empty;

		// Token: 0x04000017 RID: 23
		private List<string> _capabilities = new List<string>();

		// Token: 0x04000018 RID: 24
		private string _manifestBasePath = string.Empty;

		// Token: 0x04000019 RID: 25
		private string _manifestDestinationPath = string.Empty;

		// Token: 0x0400001A RID: 26
		private string _productID = string.Empty;

		// Token: 0x0400001B RID: 27
		private string _version = string.Empty;
	}
}
