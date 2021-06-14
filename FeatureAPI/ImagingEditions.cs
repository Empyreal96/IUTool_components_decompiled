using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000028 RID: 40
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "ImagingEditions", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class ImagingEditions
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000082F0 File Offset: 0x000064F0
		public static List<Edition> Editions
		{
			get
			{
				if (ImagingEditions._editionsXML == null)
				{
					object lockEditionsXML = ImagingEditions._lockEditionsXML;
					lock (lockEditionsXML)
					{
						if (ImagingEditions._editionsXML == null)
						{
							ImagingEditions._editionsXML = ImagingEditions.ValidateEditions(new IULogger());
						}
					}
				}
				return ImagingEditions._editionsXML.EditionList;
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008350 File Offset: 0x00006550
		public static Edition GetProductEdition(string product)
		{
			Edition result = null;
			foreach (Edition edition in ImagingEditions.Editions)
			{
				if (product.Equals(edition.Name, StringComparison.OrdinalIgnoreCase) || product.Equals(edition.AlternateName, StringComparison.OrdinalIgnoreCase))
				{
					result = edition;
					break;
				}
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000083C0 File Offset: 0x000065C0
		public static List<CpuId> GetWowGuestCpuTypes(List<string> fmFiles, CpuId cpuType)
		{
			List<CpuId> result = new List<CpuId>();
			List<string> fmfilenamesOnly = (from fm in fmFiles
			select Path.GetFileName(fm)).ToList<string>();
			Func<EditionPackage, bool> <>9__2;
			Edition edition = ImagingEditions.Editions.FirstOrDefault(delegate(Edition ed)
			{
				IEnumerable<EditionPackage> coreFeatureManifestPackages = ed.CoreFeatureManifestPackages;
				Func<EditionPackage, bool> predicate;
				if ((predicate = <>9__2) == null)
				{
					predicate = (<>9__2 = ((EditionPackage fm) => fmfilenamesOnly.Contains(Path.GetFileName(fm.FMDevicePath), StringComparer.OrdinalIgnoreCase)));
				}
				return coreFeatureManifestPackages.Any(predicate);
			});
			if (edition != null)
			{
				result = edition.GetSupportedWowCpuTypes(cpuType);
			}
			return result;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000842C File Offset: 0x0000662C
		public static ImagingEditions ValidateEditions(IULogger logger)
		{
			XsdValidator xsdValidator = new XsdValidator();
			ImagingEditions result = new ImagingEditions();
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = "ImagingEditions.xml";
				string text4 = "ImagingEditions.xsd";
				foreach (string text5 in manifestResourceNames)
				{
					if (text5.Contains(text3))
					{
						text = text5;
					}
					else if (text5.Contains(text4))
					{
						text2 = text5;
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					throw new XsdValidatorException("FeatureAPI!ValidateEditions: XML resource was not found: " + text3);
				}
				if (string.IsNullOrEmpty(text2))
				{
					throw new XsdValidatorException("FeatureAPI!ValidateEditions: XSD resource was not found: " + text4);
				}
				using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
				{
					if (manifestResourceStream == null)
					{
						throw new XsdValidatorException("FeatureAPI!ValidateEditions: Failed to load the editions file: " + text);
					}
					using (Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream(text2))
					{
						if (manifestResourceStream2 == null)
						{
							throw new XsdValidatorException("FeatureAPI!ValidateEditions: Failed to load the embeded schema file: " + text2);
						}
						xsdValidator.ValidateXsd(manifestResourceStream2, manifestResourceStream, text3, logger);
						manifestResourceStream.Seek(0L, SeekOrigin.Begin);
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImagingEditions));
						try
						{
							result = (ImagingEditions)xmlSerializer.Deserialize(manifestResourceStream);
						}
						catch (Exception innerException)
						{
							throw new FeatureAPIException("FeatureAPI!ValidateEditions: Unable to parse editions XML file.", innerException);
						}
					}
				}
			}
			catch (XsdValidatorException innerException2)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateEditions: Unable to validate editions XSD.", innerException2);
			}
			return result;
		}

		// Token: 0x040000C9 RID: 201
		private static readonly object _lockEditionsXML = new object();

		// Token: 0x040000CA RID: 202
		private static ImagingEditions _editionsXML = null;

		// Token: 0x040000CB RID: 203
		[XmlArrayItem(ElementName = "Edition", Type = typeof(Edition), IsNullable = false)]
		[XmlArray("Editions")]
		public List<Edition> EditionList;
	}
}
