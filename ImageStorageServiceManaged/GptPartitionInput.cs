using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200006E RID: 110
	public class GptPartitionInput
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x000149C4 File Offset: 0x00012BC4
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x000149CC File Offset: 0x00012BCC
		[XmlChoiceIdentifier("PartitionSpecifier")]
		[XmlElement("Name", typeof(string))]
		[XmlElement("Id", typeof(string))]
		public object DataType { get; set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x000149D5 File Offset: 0x00012BD5
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x000149DD File Offset: 0x00012BDD
		[XmlIgnore]
		public GptPartitionTypeChoice PartitionSpecifier { get; set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x000149E8 File Offset: 0x00012BE8
		[XmlIgnore]
		public Guid PartitionId
		{
			get
			{
				Guid result = Guid.Empty;
				if (this.PartitionSpecifier == GptPartitionTypeChoice.Id)
				{
					result = new Guid(this.DataType as string);
				}
				else
				{
					string text = this.DataType as string;
					if (string.Compare(ImageConstants.MAINOS_PARTITION_NAME, text, true, CultureInfo.InvariantCulture) == 0)
					{
						result = ImageConstants.MAINOS_PARTITION_ID;
					}
					else if (string.Compare(ImageConstants.SYSTEM_PARTITION_NAME, text, true, CultureInfo.InvariantCulture) == 0)
					{
						result = ImageConstants.SYSTEM_PARTITION_ID;
					}
					else
					{
						if (string.Compare(ImageConstants.MMOS_PARTITION_NAME, text, true, CultureInfo.InvariantCulture) != 0)
						{
							throw new ImageStorageException(string.Format("{0}: The partition name {1} is not currently supported.", MethodBase.GetCurrentMethod().Name, text));
						}
						result = ImageConstants.MMOS_PARTITION_ID;
					}
				}
				return result;
			}
		}
	}
}
