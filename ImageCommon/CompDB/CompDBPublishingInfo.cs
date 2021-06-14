using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;
using Microsoft.WindowsPhone.Imaging;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x0200000D RID: 13
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "CompDBPublishingInfo", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class CompDBPublishingInfo
	{
		// Token: 0x06000086 RID: 134 RVA: 0x000057B4 File Offset: 0x000039B4
		public CompDBPublishingInfo()
		{
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000057C8 File Offset: 0x000039C8
		public CompDBPublishingInfo(CompDBPublishingInfo srcInfo)
		{
			this.Version = srcInfo.Version;
			this.Packages = (from pkg in srcInfo.Packages
			select new CompDBPublishingPackageInfo(pkg)).ToList<CompDBPublishingPackageInfo>();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005828 File Offset: 0x00003A28
		public CompDBPublishingInfo(BuildCompDB srcDB, IULogger logger)
		{
			this._iuLogger = logger;
			this.Product = srcDB.Product;
			this.BuildID = srcDB.BuildID;
			this.BuildInfo = srcDB.BuildInfo;
			this.OSVersion = srcDB.OSVersion;
			this.BuildArch = srcDB.BuildArch;
			this.ReleaseType = srcDB.ReleaseType;
			if (srcDB is BSPCompDB)
			{
				BSPCompDB bspcompDB = srcDB as BSPCompDB;
				this.BSPProductName = bspcompDB.BSPProductName;
				this.BSPVersion = bspcompDB.BSPVersion;
			}
			this.Packages = new List<CompDBPublishingPackageInfo>();
			foreach (CompDBPackageInfo compDBPackageInfo in srcDB.Packages)
			{
				this.Packages.AddRange(compDBPackageInfo.GetPublishingPackages());
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005918 File Offset: 0x00003B18
		public static CompDBPublishingInfo ValidateAndLoad(string xmlFile, IULogger logger)
		{
			CompDBPublishingInfo result = new CompDBPublishingInfo();
			string text = string.Empty;
			string compDBPublishingInfoSchema = BuildPaths.CompDBPublishingInfoSchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(compDBPublishingInfoSchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ImageCommonException("ImageCommon::CompDBPublishingInfo!ValidateAndLoad: XSD resource was not found: " + compDBPublishingInfoSchema);
			}
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompDBPublishingInfo));
			try
			{
				result = (CompDBPublishingInfo)xmlSerializer.Deserialize(textReader);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::CompDBPublishingInfo!ValidateAndLoad: Unable to parse CompDB Publishing Info XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textReader.Close();
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				XsdValidator xsdValidator = new XsdValidator();
				try
				{
					xsdValidator.ValidateXsd(manifestResourceStream, xmlFile, logger);
				}
				catch (XsdValidatorException innerException2)
				{
					throw new ImageCommonException("ImageCommon::CompDBPublishingInfo!ValidateAndLoad: Unable to validate CompDB Publishing Info XSD for file '" + xmlFile + "'.", innerException2);
				}
			}
			logger.LogInfo("CompDBPublishingInfo: Successfully validated the CompDB Publishing Info XML: {0}", new object[]
			{
				xmlFile
			});
			return result;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005A68 File Offset: 0x00003C68
		public void WriteToFile(string xmlFile)
		{
			string directoryName = Path.GetDirectoryName(xmlFile);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			TextWriter textWriter = new StreamWriter(LongPathFile.OpenWrite(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompDBPublishingInfo));
			try
			{
				xmlSerializer.Serialize(textWriter, this);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon::CompDBPublishingInfo!WriteToFile: Unable to write Publishing Info CompDB XML file '" + xmlFile + "'", innerException);
			}
			finally
			{
				textWriter.Close();
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005AEC File Offset: 0x00003CEC
		public override string ToString()
		{
			return "Publishing Info: Count=" + ((this.Packages == null) ? "0" : (this.Packages.Count<CompDBPublishingPackageInfo>().ToString() + " (Version: " + this.Version + ")"));
		}

		// Token: 0x0400004F RID: 79
		private IULogger _iuLogger;

		// Token: 0x04000050 RID: 80
		public static string c_CompDBPublishingInfoFileIdentifier = "_publish";

		// Token: 0x04000051 RID: 81
		public const string c_CompDBPublishingInfoVersion = "1.2";

		// Token: 0x04000052 RID: 82
		[XmlAttribute]
		public string Version = "1.2";

		// Token: 0x04000053 RID: 83
		[XmlAttribute]
		public string Product;

		// Token: 0x04000054 RID: 84
		[XmlAttribute]
		public Guid BuildID;

		// Token: 0x04000055 RID: 85
		[XmlAttribute]
		public string BuildInfo;

		// Token: 0x04000056 RID: 86
		[XmlAttribute]
		public string OSVersion;

		// Token: 0x04000057 RID: 87
		[XmlAttribute]
		public string BuildArch;

		// Token: 0x04000058 RID: 88
		[XmlAttribute]
		[DefaultValue(ReleaseType.Invalid)]
		public ReleaseType ReleaseType;

		// Token: 0x04000059 RID: 89
		[XmlAttribute]
		public string BSPVersion;

		// Token: 0x0400005A RID: 90
		[XmlAttribute]
		public string BSPProductName;

		// Token: 0x0400005B RID: 91
		[XmlArrayItem(ElementName = "Package", Type = typeof(CompDBPublishingPackageInfo), IsNullable = false)]
		[XmlArray]
		public List<CompDBPublishingPackageInfo> Packages;
	}
}
