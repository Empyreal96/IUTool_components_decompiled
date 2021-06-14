using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Interfaces;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000031 RID: 49
	public class PkgBldrLoader
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000074E9 File Offset: 0x000056E9
		public Dictionary<string, IPkgPlugin> Plugins
		{
			get
			{
				return this.m_plugins;
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000074F4 File Offset: 0x000056F4
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public PkgBldrLoader(PluginType pluginType, PkgBldrCmd pkgBldrArgs, IDeploymentLogger logger = null)
		{
			if (pkgBldrArgs == null)
			{
				throw new ArgumentNullException("pkgBldrArgs");
			}
			this.m_logger = (logger ?? new Logger());
			this.m_pkgBldrArgs = pkgBldrArgs;
			this.m_csiXsdPath = Environment.ExpandEnvironmentVariables(pkgBldrArgs.csiXsdPath);
			this.m_pkgXsdPath = Environment.ExpandEnvironmentVariables(pkgBldrArgs.pkgXsdPath);
			this.m_sharedXsdPath = Environment.ExpandEnvironmentVariables(pkgBldrArgs.sharedXsdPath);
			this.m_wmXsdPath = Environment.ExpandEnvironmentVariables(pkgBldrArgs.wmXsdPath);
			this.m_pluginType = pluginType;
			this.LoadPlugins();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000757D File Offset: 0x0000577D
		public List<XmlSchema> WmSchemaSet()
		{
			return this.m_wmSchemaValidator.GetMergedSchemas();
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000758A File Offset: 0x0000578A
		public List<XmlSchema> CsiSchemaSet()
		{
			return this.m_csiSchemaValidator.GetMergedSchemas();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007598 File Offset: 0x00005798
		private void LoadPlugins()
		{
			this.m_plugins = null;
			this.m_wmPlugins = null;
			this.m_plugins = this.LoadPackagePlugins(this.m_pluginType);
			if (this.m_pluginType == PluginType.PkgFilter)
			{
				return;
			}
			if (this.m_pluginType == PluginType.WmToCsi)
			{
				this.m_wmPlugins = this.m_plugins;
			}
			else
			{
				this.m_wmPlugins = this.LoadPackagePlugins(PluginType.WmToCsi);
			}
			switch (this.m_pluginType)
			{
			case PluginType.WmToCsi:
			case PluginType.CsiToWm:
				this.m_wmSchemaValidator = null;
				this.m_csiSchemaValidator = null;
				this.LoadWmSchemas();
				this.LoadCsiSchemas();
				return;
			case PluginType.CsiToCsi:
				this.m_csiSchemaValidator = null;
				this.LoadCsiSchemas();
				return;
			case PluginType.PkgToWm:
				this.m_pkgSchemaValidator = null;
				this.m_wmSchemaValidator = null;
				this.LoadPkgSchemas();
				this.LoadWmSchemas();
				return;
			case PluginType.Csi2Pkg:
				this.m_csiSchemaValidator = null;
				this.m_pkgSchemaValidator = null;
				this.LoadCsiSchemas();
				this.LoadPkgSchemas();
				return;
			case PluginType.CsiToCab:
				this.m_csiSchemaValidator = null;
				this.LoadCsiSchemas();
				return;
			default:
				throw new PkgGenException("stream loading not supported for this plugin type");
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007690 File Offset: 0x00005890
		private void LoadWmSchemas()
		{
			this.m_wmSchemaValidator = new SchemaSet(this.m_logger, this.m_pkgBldrArgs);
			this.m_wmSchemaValidator.LoadSchemasFromPlugins(this.m_wmPlugins.Values);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000076C0 File Offset: 0x000058C0
		private void LoadCsiSchemas()
		{
			this.m_csiSchemaValidator = new SchemaSet(this.m_logger, null);
			List<string> list = new List<string>();
			foreach (string path in LongPathDirectory.GetFiles(this.m_csiXsdPath, "*.xsd"))
			{
				list.Add(LongPath.GetFullPath(path));
			}
			foreach (string path2 in LongPathDirectory.GetFiles(this.m_sharedXsdPath, "*.xsd"))
			{
				list.Add(LongPath.GetFullPath(path2));
			}
			this.m_csiSchemaValidator.LoadSchemasFromFiles(list);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007754 File Offset: 0x00005954
		private void LoadPkgSchemas()
		{
			this.m_pkgSchemaValidator = new SchemaSet(this.m_logger, null);
			List<string> list = new List<string>();
			foreach (string item in Directory.GetFiles(this.m_pkgXsdPath, "*.xsd"))
			{
				list.Add(item);
			}
			this.m_pkgSchemaValidator.LoadSchemasFromFiles(list);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000077B0 File Offset: 0x000059B0
		public void ValidateInput(XDocument xmlDoc)
		{
			switch (this.m_pluginType)
			{
			case PluginType.WmToCsi:
				this.m_wmSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.CsiToWm:
			case PluginType.CsiToCsi:
			case PluginType.Csi2Pkg:
			case PluginType.CsiToCab:
				this.m_csiSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.PkgToWm:
				this.m_pkgSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.PkgFilter:
				return;
			default:
				throw new PkgGenException("invalid plugin type, can't validate input schema");
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000781C File Offset: 0x00005A1C
		public void ValidateOutput(XDocument xmlDoc)
		{
			switch (this.m_pluginType)
			{
			case PluginType.WmToCsi:
			case PluginType.CsiToCsi:
				this.m_csiSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.CsiToWm:
			case PluginType.PkgToWm:
				this.m_wmSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.Csi2Pkg:
				this.m_pkgSchemaValidator.ValidateXmlDoc(xmlDoc);
				return;
			case PluginType.CsiToCab:
			case PluginType.PkgFilter:
				return;
			default:
				throw new PkgGenException("invalid plugin type, can't validate output schema");
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007888 File Offset: 0x00005A88
		private Dictionary<string, IPkgPlugin> LoadPackagePlugins(PluginType pType)
		{
			CompositionContainer compositionContainer = null;
			try
			{
				string directoryName = LongPath.GetDirectoryName(typeof(IPkgPlugin).Assembly.Location);
				AggregateCatalog aggregateCatalog = new AggregateCatalog();
				if (pType == PluginType.WmToCsi)
				{
					aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IPkgPlugin).Assembly));
				}
				string searchPattern = null;
				switch (pType)
				{
				case PluginType.WmToCsi:
					searchPattern = "PkgBldr.Plugin.WmToCsi.*.dll";
					break;
				case PluginType.CsiToWm:
					searchPattern = "PkgBldr.Plugin.CsiToWm.*.dll";
					break;
				case PluginType.CsiToCsi:
					searchPattern = "PkgBldr.Plugin.CsiToCsi.*.dll";
					break;
				case PluginType.PkgToWm:
				case PluginType.PkgFilter:
					searchPattern = "PkgBldr.Plugin.PkgToWm.*.dll";
					break;
				case PluginType.Csi2Pkg:
					searchPattern = "PkgBldr.Plugin.CsiToPkg.*.dll";
					break;
				case PluginType.CsiToCab:
					searchPattern = "PkgBldr.Plugin.CsiToCab.*.dll";
					break;
				}
				if (LongPathDirectory.Exists(directoryName))
				{
					aggregateCatalog.Catalogs.Add(new DirectoryCatalog(directoryName, searchPattern));
				}
				CompositionBatch batch = new CompositionBatch();
				compositionContainer = new CompositionContainer(aggregateCatalog, new ExportProvider[0]);
				compositionContainer.Compose(batch);
			}
			catch (CompositionException innerException)
			{
				throw new PkgGenException(innerException, "Failed to load package plugins.", new object[0]);
			}
			Dictionary<string, IPkgPlugin> dictionary = new Dictionary<string, IPkgPlugin>(StringComparer.OrdinalIgnoreCase);
			foreach (IPkgPlugin pkgPlugin in compositionContainer.GetExportedValues<IPkgPlugin>())
			{
				if (string.IsNullOrEmpty(pkgPlugin.XmlElementName))
				{
					throw new PkgGenException("Failed to load package plugin '{0}'. Invalid XmlElementName.", new object[]
					{
						pkgPlugin.Name
					});
				}
				try
				{
					dictionary.Add(pkgPlugin.XmlElementName, pkgPlugin);
				}
				catch (ArgumentException innerException2)
				{
					string fileName = LongPath.GetFileName(pkgPlugin.GetType().Assembly.Location);
					IPkgPlugin pkgPlugin2 = dictionary[pkgPlugin.XmlElementName];
					string fileName2 = LongPath.GetFileName(pkgPlugin2.GetType().Assembly.Location);
					throw new PkgGenException(innerException2, "Failed to load package plugin '{0}' ({1}). Uses a duplicate XmlElementName with '{2}' ({3}).", new object[]
					{
						pkgPlugin.Name,
						fileName,
						pkgPlugin2.Name,
						fileName2
					});
				}
			}
			return dictionary;
		}

		// Token: 0x04000032 RID: 50
		private Dictionary<string, IPkgPlugin> m_plugins;

		// Token: 0x04000033 RID: 51
		private Dictionary<string, IPkgPlugin> m_wmPlugins;

		// Token: 0x04000034 RID: 52
		private PluginType m_pluginType;

		// Token: 0x04000035 RID: 53
		private SchemaSet m_csiSchemaValidator;

		// Token: 0x04000036 RID: 54
		private SchemaSet m_wmSchemaValidator;

		// Token: 0x04000037 RID: 55
		private SchemaSet m_pkgSchemaValidator;

		// Token: 0x04000038 RID: 56
		private IDeploymentLogger m_logger;

		// Token: 0x04000039 RID: 57
		private PkgBldrCmd m_pkgBldrArgs;

		// Token: 0x0400003A RID: 58
		private string m_csiXsdPath;

		// Token: 0x0400003B RID: 59
		private string m_pkgXsdPath;

		// Token: 0x0400003C RID: 60
		private string m_sharedXsdPath;

		// Token: 0x0400003D RID: 61
		private string m_wmXsdPath;
	}
}
