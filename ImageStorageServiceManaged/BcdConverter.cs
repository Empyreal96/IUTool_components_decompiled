using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005E RID: 94
	public class BcdConverter
	{
		// Token: 0x0600044F RID: 1103 RVA: 0x000134FE File Offset: 0x000116FE
		[CLSCompliant(false)]
		public BcdConverter(IULogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00013510 File Offset: 0x00011710
		public void ProcessInputXml(string bcdLayoutFile, string bcdLayoutSchema)
		{
			if (!File.Exists(bcdLayoutSchema))
			{
				throw new ImageStorageException(string.Format("{0}: BCD layout schema file is not found: {1}.", MethodBase.GetCurrentMethod().Name, bcdLayoutSchema));
			}
			using (FileStream fileStream = new FileStream(bcdLayoutSchema, FileMode.Open, FileAccess.Read))
			{
				this.ProcessInputXml(bcdLayoutFile, fileStream);
			}
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00013570 File Offset: 0x00011770
		public void ProcessInputXml(string bcdLayoutFile, Stream bcdLayoutSchema)
		{
			if (!File.Exists(bcdLayoutFile))
			{
				throw new ImageStorageException(string.Format("{0}: BCD layout file is not found: {1}.", MethodBase.GetCurrentMethod().Name, bcdLayoutFile));
			}
			BCDXsdValidator bcdxsdValidator = new BCDXsdValidator();
			try
			{
				bcdxsdValidator.ValidateXsd(bcdLayoutSchema, bcdLayoutFile, this._logger);
			}
			catch (BCDXsdValidatorException innerException)
			{
				throw new ImageStorageException(string.Format("{0}: Failed to validate the BCD layout file: {1}.", MethodBase.GetCurrentMethod().Name, bcdLayoutFile), innerException);
			}
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(bcdLayoutFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				XmlReader xmlReader = XmlReader.Create(fileStream);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(BcdInput));
				this._bcdInput = (BcdInput)xmlSerializer.Deserialize(xmlReader);
			}
			catch (SecurityException innerException2)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to access the BCD layout file.", MethodBase.GetCurrentMethod().Name), innerException2);
			}
			catch (UnauthorizedAccessException innerException3)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to open the BCD layout file for reading.", MethodBase.GetCurrentMethod().Name), innerException3);
			}
			catch (InvalidOperationException innerException4)
			{
				throw new ImageStorageException(string.Format("{0}: Unable to deserialize the BCD layout file.", MethodBase.GetCurrentMethod().Name), innerException4);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				fileStream = null;
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x000136BC File Offset: 0x000118BC
		public void SaveToRegFile(Stream stream)
		{
			StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode);
			try
			{
				if (this._bcdInput.IncludeRegistryHeader)
				{
					streamWriter.WriteLine("{0}", BcdConverter.Header);
					streamWriter.WriteLine();
				}
				if (this._bcdInput.SaveKeyToRegistry)
				{
					streamWriter.WriteLine("[{0}]", BcdConverter.HiveBase);
					streamWriter.WriteLine();
				}
				if (this._bcdInput.IncludeDescriptions)
				{
					streamWriter.WriteLine("[{0}\\Description]", BcdConverter.HiveBase);
					streamWriter.WriteLine("\"KeyName\"=\"BCD00000000\"");
					streamWriter.WriteLine("\"System\"=dword:{0:x8}", 1);
					streamWriter.WriteLine("\"TreatAsSystem\"=dword:{0:x8}", 1);
					streamWriter.WriteLine();
				}
				this._bcdInput.SaveAsRegFile(streamWriter, BcdConverter.HiveBase);
			}
			finally
			{
				streamWriter.Flush();
				streamWriter = null;
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00013798 File Offset: 0x00011998
		public void SaveToRegData(BcdRegData bcdRegData)
		{
			if (this._bcdInput.SaveKeyToRegistry)
			{
				bcdRegData.AddRegKey(BcdConverter.HiveBase);
			}
			if (this._bcdInput.IncludeDescriptions)
			{
				string regKey = string.Format("{0}\\Description", BcdConverter.HiveBase);
				bcdRegData.AddRegKey(regKey);
				bcdRegData.AddRegValue(regKey, "KeyName", "BCD00000000", "REG_SZ");
				bcdRegData.AddRegValue(regKey, "System", string.Format("{0:x8}", 1), "REG_DWORD");
				bcdRegData.AddRegValue(regKey, "TreatAsSystem", string.Format("{0:x8}", 1), "REG_DWORD");
			}
			this._bcdInput.SaveAsRegData(bcdRegData, BcdConverter.HiveBase);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001384C File Offset: 0x00011A4C
		public static void ConvertBCD(string inputFile, string outputFile)
		{
			BcdConverter bcdConverter = new BcdConverter(new IULogger());
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BcdLayout.xsd"))
			{
				bcdConverter.ProcessInputXml(inputFile, manifestResourceStream);
			}
			using (FileStream fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite))
			{
				bcdConverter.SaveToRegFile(fileStream);
			}
		}

		// Token: 0x04000264 RID: 612
		public static readonly string HiveBase = "HKEY_LOCAL_MACHINE\\BCD";

		// Token: 0x04000265 RID: 613
		public static readonly string Header = "Windows Registry Editor Version 5.00";

		// Token: 0x04000266 RID: 614
		private IULogger _logger;

		// Token: 0x04000267 RID: 615
		private BcdInput _bcdInput;
	}
}
