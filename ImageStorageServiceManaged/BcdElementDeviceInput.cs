using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200006C RID: 108
	public class BcdElementDeviceInput
	{
		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x0001460E File Offset: 0x0001280E
		// (set) Token: 0x060004AC RID: 1196 RVA: 0x00014616 File Offset: 0x00012816
		[XmlChoiceIdentifier("DeviceType")]
		[XmlElement("GPTDevice", typeof(BcdElementDeviceGptInput))]
		[XmlElement("MBRDevice", typeof(BcdElementDeviceMbrInput))]
		[XmlElement("RamdiskDevice", typeof(BcdElementDeviceRamdiskInput))]
		public object DeviceValue { get; set; }

		// Token: 0x060004AD RID: 1197 RVA: 0x00014620 File Offset: 0x00012820
		public void SaveAsRegFile(TextWriter writer, string elementName)
		{
			switch (this.DeviceType)
			{
			case DeviceTypeChoice.GPTDevice:
			{
				BcdElementDevice bcdElementDevice = BcdElementDeviceGptInput.CreateGptBootDevice(this.DeviceValue as BcdElementDeviceGptInput);
				byte[] array = new byte[bcdElementDevice.BinarySize];
				MemoryStream memoryStream = new MemoryStream(array);
				try
				{
					bcdElementDevice.WriteToStream(memoryStream);
				}
				finally
				{
					memoryStream.Flush();
					memoryStream = null;
				}
				BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", array);
				writer.WriteLine();
				array = null;
				return;
			}
			case DeviceTypeChoice.MBRDevice:
			{
				BcdElementDevice bcdElementDevice2 = BcdElementDevice.CreateBaseBootDevice();
				byte[] array2 = new byte[bcdElementDevice2.BinarySize];
				MemoryStream memoryStream2 = new MemoryStream(array2);
				try
				{
					bcdElementDevice2.WriteToStream(memoryStream2);
				}
				finally
				{
					memoryStream2.Flush();
					memoryStream2 = null;
				}
				BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", array2);
				writer.WriteLine();
				array2 = null;
				return;
			}
			case DeviceTypeChoice.RamdiskDevice:
			{
				BcdElementDeviceRamdiskInput bcdElementDeviceRamdiskInput = this.DeviceValue as BcdElementDeviceRamdiskInput;
				DeviceTypeChoice deviceType = bcdElementDeviceRamdiskInput.ParentDevice.DeviceType;
				BcdElementBootDevice parentDevice;
				if (deviceType != DeviceTypeChoice.GPTDevice)
				{
					if (deviceType != DeviceTypeChoice.MBRDevice)
					{
						throw new ImageStorageException(string.Format("{0}: The given Ramdisk parent type is not supported.", MethodBase.GetCurrentMethod().Name));
					}
					parentDevice = BcdElementBootDevice.CreateBaseBootDevice();
				}
				else
				{
					parentDevice = BcdElementDeviceGptInput.CreateGptBootDevice(bcdElementDeviceRamdiskInput.ParentDevice.DeviceValue as BcdElementDeviceGptInput).BootDevice;
				}
				BcdElementDevice bcdElementDevice3 = BcdElementDevice.CreateBaseRamdiskDevice(bcdElementDeviceRamdiskInput.FilePath, parentDevice);
				MemoryStream memoryStream3 = new MemoryStream();
				byte[] array3;
				try
				{
					bcdElementDevice3.WriteToStream(memoryStream3);
					array3 = new byte[memoryStream3.ToArray().Length];
					Array.Copy(memoryStream3.ToArray(), array3, memoryStream3.Length);
				}
				finally
				{
					memoryStream3.Flush();
					memoryStream3 = null;
				}
				BcdElementValueTypeInput.WriteByteArray(writer, elementName, "\"Element\"=hex:", array3);
				writer.WriteLine();
				array3 = null;
				return;
			}
			default:
				throw new ImageStorageException(string.Format("{0}: Unsupported partition type: {1}.", MethodBase.GetCurrentMethod().Name, this.DeviceType));
			}
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00014804 File Offset: 0x00012A04
		public void SaveAsRegData(BcdRegData bcdRegData, string path)
		{
			switch (this.DeviceType)
			{
			case DeviceTypeChoice.GPTDevice:
			{
				BcdElementDevice bcdElementDevice = BcdElementDeviceGptInput.CreateGptBootDevice(this.DeviceValue as BcdElementDeviceGptInput);
				byte[] array = new byte[bcdElementDevice.BinarySize];
				MemoryStream memoryStream = new MemoryStream(array);
				try
				{
					bcdElementDevice.WriteToStream(memoryStream);
				}
				finally
				{
					memoryStream.Flush();
					memoryStream = null;
				}
				BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, array);
				array = null;
				return;
			}
			case DeviceTypeChoice.MBRDevice:
			{
				BcdElementDevice bcdElementDevice2 = BcdElementDevice.CreateBaseBootDevice();
				byte[] array2 = new byte[bcdElementDevice2.BinarySize];
				MemoryStream memoryStream2 = new MemoryStream(array2);
				try
				{
					bcdElementDevice2.WriteToStream(memoryStream2);
				}
				finally
				{
					memoryStream2.Flush();
					memoryStream2 = null;
				}
				BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, array2);
				array2 = null;
				return;
			}
			case DeviceTypeChoice.RamdiskDevice:
			{
				BcdElementDeviceRamdiskInput bcdElementDeviceRamdiskInput = this.DeviceValue as BcdElementDeviceRamdiskInput;
				DeviceTypeChoice deviceType = bcdElementDeviceRamdiskInput.ParentDevice.DeviceType;
				BcdElementBootDevice parentDevice;
				if (deviceType != DeviceTypeChoice.GPTDevice)
				{
					if (deviceType != DeviceTypeChoice.MBRDevice)
					{
						throw new ImageStorageException(string.Format("{0}: The given Ramdisk parent type is not supported.", MethodBase.GetCurrentMethod().Name));
					}
					parentDevice = BcdElementBootDevice.CreateBaseBootDevice();
				}
				else
				{
					parentDevice = BcdElementDeviceGptInput.CreateGptBootDevice(bcdElementDeviceRamdiskInput.ParentDevice.DeviceValue as BcdElementDeviceGptInput).BootDevice;
				}
				BcdElementDevice bcdElementDevice3 = BcdElementDevice.CreateBaseRamdiskDevice(bcdElementDeviceRamdiskInput.FilePath, parentDevice);
				MemoryStream memoryStream3 = new MemoryStream();
				byte[] array3;
				try
				{
					bcdElementDevice3.WriteToStream(memoryStream3);
					array3 = new byte[memoryStream3.ToArray().Length];
					Array.Copy(memoryStream3.ToArray(), array3, memoryStream3.Length);
				}
				finally
				{
					memoryStream3.Flush();
					memoryStream3 = null;
				}
				BcdElementValueTypeInput.WriteByteArray(bcdRegData, path, array3);
				array3 = null;
				return;
			}
			default:
				throw new ImageStorageException(string.Format("{0}: Unsupported partition type: {1}.", MethodBase.GetCurrentMethod().Name, this.DeviceType));
			}
		}

		// Token: 0x0400028F RID: 655
		[XmlIgnore]
		public DeviceTypeChoice DeviceType;
	}
}
