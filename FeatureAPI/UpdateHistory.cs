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
	// Token: 0x02000007 RID: 7
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "UpdateHistory", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class UpdateHistory
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00003194 File Offset: 0x00001394
		public static UpdateHistory ValidateUpdateHistory(string xmlFile, IULogger logger)
		{
			UpdateHistory updateHistory = new UpdateHistory();
			string text = string.Empty;
			string updateHistorySchema = DevicePaths.UpdateHistorySchema;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			foreach (string text2 in executingAssembly.GetManifestResourceNames())
			{
				if (text2.Contains(updateHistorySchema))
				{
					text = text2;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new XsdValidatorException("FeatureAPI!ValidateUpdateHistory: XSD resource was not found: " + updateHistorySchema);
			}
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
			{
				try
				{
					new XsdValidator().ValidateXsd(manifestResourceStream, xmlFile, logger);
				}
				catch (XsdValidatorException innerException)
				{
					throw new FeatureAPIException("FeatureAPI!ValidateUpdateHistory: Unable to validate Update History XSD.", innerException);
				}
			}
			logger.LogInfo("FeatureAPI: Successfully validated the Update History XML", new object[0]);
			TextReader textReader = new StreamReader(LongPathFile.OpenRead(xmlFile));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateHistory));
			try
			{
				updateHistory = (UpdateHistory)xmlSerializer.Deserialize(textReader);
				foreach (UpdateEvent updateEvent in updateHistory.UpdateEvents)
				{
					foreach (UpdateOSOutputPackage updateOSOutputPackage in updateEvent.UpdateResults.Packages)
					{
						updateOSOutputPackage.CpuType = CpuIdParser.Parse(updateOSOutputPackage.CpuTypeStr);
					}
				}
			}
			catch (Exception innerException2)
			{
				throw new FeatureAPIException("FeatureAPI!ValidateUpdateHistory: Unable to parse Update History XML file.", innerException2);
			}
			finally
			{
				textReader.Close();
			}
			return updateHistory;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003354 File Offset: 0x00001554
		public UpdateOSOutput GetPackageList()
		{
			UpdateOSOutput updateOSOutput = new UpdateOSOutput();
			List<UpdateEvent> list = (from ue in this.UpdateEvents
			where ue.UpdateResults.UpdateState.Equals("Update Complete", StringComparison.OrdinalIgnoreCase)
			orderby ue.Sequence
			select ue).ToList<UpdateEvent>();
			Dictionary<string, UpdateOSOutputPackage> dictionary = new Dictionary<string, UpdateOSOutputPackage>(StringComparer.OrdinalIgnoreCase);
			foreach (UpdateEvent updateEvent in list)
			{
				foreach (UpdateOSOutputPackage updateOSOutputPackage in updateEvent.UpdateResults.Packages)
				{
					if (updateOSOutputPackage.IsRemoval)
					{
						dictionary.Remove(updateOSOutputPackage.Name);
					}
					else
					{
						dictionary[updateOSOutputPackage.Name] = updateOSOutputPackage;
					}
				}
				updateOSOutput.OverallResult = updateEvent.UpdateResults.OverallResult;
				updateOSOutput.UpdateState = updateEvent.UpdateResults.UpdateState;
			}
			updateOSOutput.Packages = new List<UpdateOSOutputPackage>(dictionary.Values);
			updateOSOutput.Description = "List of packages currently in this image or on this device";
			return updateOSOutput;
		}

		// Token: 0x04000029 RID: 41
		[XmlArrayItem(ElementName = "UpdateEvent", Type = typeof(UpdateEvent), IsNullable = false)]
		[XmlArray]
		public List<UpdateEvent> UpdateEvents;
	}
}
