using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000027 RID: 39
	public class ImageSigner
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x0000ED8E File Offset: 0x0000CF8E
		public ImageSigner()
		{
			this._sha256 = new SHA256CryptoServiceProvider();
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000EDA1 File Offset: 0x0000CFA1
		public void Initialize(FullFlashUpdateImage ffuImage, string catalogFile, IULogger logger)
		{
			this._logger = logger;
			if (logger == null)
			{
				this._logger = new IULogger();
			}
			this._ffuImage = ffuImage;
			this._catalogFileName = catalogFile;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000EDC8 File Offset: 0x0000CFC8
		public void SignFFUImage()
		{
			if (this._ffuImage == null)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: ImageSigner has not been initialized.");
			}
			if (!File.Exists(Environment.ExpandEnvironmentVariables(this._catalogFileName)))
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: Unable to generate signed image - missing Catalog file: " + this._catalogFileName);
			}
			if (!ImageSigner.IsCatalogFile(IntPtr.Zero, this._catalogFileName))
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: The file '" + this._catalogFileName + "' is not a catalog file.");
			}
			if (!ImageSigner.HasSignature(this._catalogFileName, true))
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage:  The file '" + this._catalogFileName + "' is not signed.");
			}
			try
			{
				if (!ImageSigner.VerifyCatalogData(this._catalogFileName, this._ffuImage.HashTableData))
				{
					throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: The catalog provided does not match the image.");
				}
				this._ffuImage.CatalogData = File.ReadAllBytes(this._catalogFileName);
			}
			catch (ImageCommonException)
			{
				throw;
			}
			catch (Exception ex)
			{
				this._logger.LogError("ImageCommon!ImageSigner::SignFFUImage: Error while signing FFU image: {0}", new object[]
				{
					ex.Message
				});
				throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: Exception occurred.", ex);
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000EEF0 File Offset: 0x0000D0F0
		public void VerifyCatalog()
		{
			if (this._ffuImage == null)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyCatalog: ImageSigner has not been initialized.");
			}
			if (this._ffuImage.CatalogData == null || this._ffuImage.CatalogData.Length == 0)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyCatalog: The FFU does not contain a catalog.");
			}
			if (!this.VerifyCatalogData(this._ffuImage.CatalogData, this._ffuImage.HashTableData))
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyCatalog: The Catalog in the image does not match the Hash Table in the image.  The image appears to be corrupt or modified outside ImageApp.");
			}
			if (!this.VerifyHashTable())
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyCatalog: The Hash Table in the image does not match the payload.  The image appears to be corrupt or modified outside ImageApp.");
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000EF74 File Offset: 0x0000D174
		private bool VerifyCatalogData(byte[] catalogData, byte[] hashTableData)
		{
			this._logger.LogInfo("ImageCommon: Verfiying Hash Table against catalog...", new object[0]);
			string tempFileName = Path.GetTempFileName();
			File.WriteAllBytes(tempFileName, catalogData);
			bool result = ImageSigner.VerifyCatalogData(tempFileName, hashTableData);
			File.Delete(tempFileName);
			return result;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000EFB4 File Offset: 0x0000D1B4
		public static bool VerifyCatalogData(string catalogFile, byte[] hashTableData)
		{
			SHA1Managed sha1Managed = new SHA1Managed();
			byte[] catalogHash = ImageSigner.GetCatalogHash(catalogFile);
			byte[] array = sha1Managed.ComputeHash(hashTableData);
			if (catalogHash.Length != array.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (catalogHash[i] != array[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000EFFC File Offset: 0x0000D1FC
		internal bool VerifyHashTable()
		{
			int num = 0;
			int num2 = 0;
			if (this._ffuImage == null)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyHashTable: ImageSigner has not been initialized.");
			}
			this._logger.LogInfo("ImageCommon: Verfiying Hash Table entries...", new object[0]);
			this._logger.LogInfo("ImageCommon: Using Chunksize: {0}KB", new object[]
			{
				this._ffuImage.ChunkSize
			});
			try
			{
				byte[] hashTableData = this._ffuImage.HashTableData;
				using (FileStream imageStream = this._ffuImage.GetImageStream())
				{
					imageStream.Position = (long)((ulong)this._ffuImage.StartOfImageHeader);
					byte[] array = this.GetFirstChunkHash(imageStream);
					num2++;
					while (array != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (num > hashTableData.Length)
							{
								throw new ImageCommonException("ImageCommon!ImageSigner::VerifyHashTable: Hash Table too small for this FFU.");
							}
							if (array[i] != hashTableData[num])
							{
								this._logger.LogInfo("ImageCommon!ImageSigner::VerifyHashTable: Failed to match Chunk {0} Hash value [{1}]: {2} with {3}", new object[]
								{
									num2,
									i,
									array[i].ToString("X2"),
									hashTableData[num].ToString("X2")
								});
								throw new ImageCommonException("ImageCommon!ImageSigner::VerifyHashTable: Hash Table entry does not match hash of FFU.");
							}
							num++;
						}
						array = this.GetNextChunkHash(imageStream);
						num2++;
					}
				}
				this._logger.LogInfo("ImageCommon: The Hash Table has been sucessfully verified..", new object[0]);
			}
			catch (Exception innerException)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::VerifyHashTable: Error while retrieving Hash Table from FFU", innerException);
			}
			return true;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000F1B0 File Offset: 0x0000D3B0
		public static byte[] GenerateCatalogFile(byte[] hashData)
		{
			string tempFileName = Path.GetTempFileName();
			string tempFileName2 = Path.GetTempFileName();
			string tempFileName3 = Path.GetTempFileName();
			File.WriteAllBytes(tempFileName3, hashData);
			using (StreamWriter streamWriter = new StreamWriter(tempFileName2))
			{
				streamWriter.WriteLine("[CatalogHeader]");
				streamWriter.WriteLine("Name={0}", tempFileName);
				streamWriter.WriteLine("[CatalogFiles]");
				streamWriter.WriteLine("{0}={1}", "HashTable.blob", tempFileName3);
			}
			using (Process process = new Process())
			{
				process.StartInfo.FileName = "MakeCat.exe";
				process.StartInfo.Arguments = string.Format("\"{0}\"", tempFileName2);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				try
				{
					process.Start();
					process.WaitForExit();
				}
				catch (Exception innerException)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("CDF File: {0}\n", tempFileName2);
					if (!File.Exists(tempFileName2))
					{
						stringBuilder.AppendFormat("CDF File could not be found.\n", new object[0]);
					}
					try
					{
						stringBuilder.AppendFormat("Arguments : {0}\n", process.StartInfo.Arguments);
					}
					catch
					{
					}
					try
					{
						stringBuilder.AppendFormat("StandardError : {0}\n", process.StandardError);
					}
					catch
					{
					}
					try
					{
						stringBuilder.AppendFormat("StandardOutput : {0}\n", process.StandardOutput);
					}
					catch
					{
					}
					throw new ImageCommonException(stringBuilder.ToString(), innerException);
				}
				if (process.ExitCode != 0)
				{
					throw new ImageCommonException("ImageCommon!ImageSigner::GenerateCatalogFile: Failed call to MakeCat.");
				}
			}
			byte[] result = File.ReadAllBytes(tempFileName);
			File.Delete(tempFileName);
			File.Delete(tempFileName3);
			File.Delete(tempFileName2);
			return result;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000F3E8 File Offset: 0x0000D5E8
		private uint GetSecurityDataSize()
		{
			return this._ffuImage.GetSecureHeader.ByteCount + this._ffuImage.GetSecureHeader.CatalogSize + this._ffuImage.GetSecureHeader.HashTableSize + this._ffuImage.SecurityPadding;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000F43C File Offset: 0x0000D63C
		private byte[] GetFirstChunkHash(Stream stream)
		{
			stream.Position = (long)((ulong)this.GetSecurityDataSize());
			return this.GetNextChunkHash(stream);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000F454 File Offset: 0x0000D654
		private byte[] GetNextChunkHash(Stream stream)
		{
			byte[] array = new byte[this._ffuImage.ChunkSizeInBytes];
			if (stream.Position == stream.Length)
			{
				return null;
			}
			stream.Read(array, 0, array.Length);
			return this._sha256.ComputeHash(array);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000F49C File Offset: 0x0000D69C
		public static bool HasSignature(string filename, bool EnsureMicrosoftIssuer)
		{
			bool flag = false;
			try
			{
				X509Certificate2 x509Certificate = new X509Certificate2(filename);
				if (EnsureMicrosoftIssuer)
				{
					if (!ImageSigner.certPublicKeys.TryGetValue(x509Certificate.Thumbprint, out flag))
					{
						X509Chain x509Chain = new X509Chain(true);
						x509Chain.Build(x509Certificate);
						bool ignoreCase = true;
						foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
						{
							if (string.Compare("3B1EFD3A66EA28B16697394703A72CA340A05BD5", x509ChainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) == 0 || string.Compare("9E594333273339A97051B0F82E86F266B917EDB3", x509ChainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) == 0 || string.Compare("5f444a6740b7ca2434c7a5925222c2339ee0f1b7", x509ChainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) == 0 || string.Compare("8A334AA8052DD244A647306A76B8178FA215F344", x509ChainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) == 0)
							{
								flag = true;
								break;
							}
						}
						foreach (X509ChainElement x509ChainElement2 in x509Chain.ChainElements)
						{
							ImageSigner.certPublicKeys[x509ChainElement2.Certificate.Thumbprint] = flag;
						}
					}
				}
				else
				{
					flag = (x509Certificate != null && !string.IsNullOrEmpty(x509Certificate.Subject));
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000F5F8 File Offset: 0x0000D7F8
		public static bool HasValidSignature(string filename, List<string> validRootThumbprints)
		{
			bool result = false;
			try
			{
				X509Certificate2 certificate = new X509Certificate2(filename);
				X509Chain x509Chain = new X509Chain(true);
				x509Chain.Build(certificate);
				X509ChainElementEnumerator enumerator = x509Chain.ChainElements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					X509ChainElement element = enumerator.Current;
					if (validRootThumbprints.Any((string tp) => tp.Equals(element.Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)))
					{
						result = true;
						break;
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000F678 File Offset: 0x0000D878
		public static string GetSignatureIssuer(string filename)
		{
			string result = "File not found";
			if (File.Exists(filename))
			{
				result = "Not signed";
				X509Certificate2 x509Certificate = null;
				try
				{
					x509Certificate = new X509Certificate2(filename);
				}
				catch
				{
				}
				if (x509Certificate != null)
				{
					result = x509Certificate.Subject;
				}
			}
			return result;
		}

		// Token: 0x060001B5 RID: 437
		[DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CryptCATOpen(string pwszFileName, uint fdwOpenFlags, IntPtr hProv, uint dwPublicVersion, uint dwEncodingType);

		// Token: 0x060001B6 RID: 438
		[DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCATClose(IntPtr hCatalog);

		// Token: 0x060001B7 RID: 439
		[DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CryptCATEnumerateMember(IntPtr hCatalog, IntPtr pPrevMember);

		// Token: 0x060001B8 RID: 440
		[DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool IsCatalogFile(IntPtr hFile, string pwszFileName);

		// Token: 0x060001B9 RID: 441 RVA: 0x0000F6C4 File Offset: 0x0000D8C4
		internal static byte[] GetCatalogHash(string catalogFile)
		{
			IntPtr intPtr = new IntPtr(-1);
			IntPtr intPtr2 = intPtr;
			IntPtr zero = IntPtr.Zero;
			byte[] array = null;
			try
			{
				intPtr2 = ImageSigner.CryptCATOpen(catalogFile, 2U, IntPtr.Zero, 0U, 0U);
				IntPtr intPtr3 = ImageSigner.CryptCATEnumerateMember(intPtr2, IntPtr.Zero);
				if (intPtr3 == IntPtr.Zero)
				{
					throw new ImageCommonException("ImageCommon!ImageSigner::GetCatalogHash: Failed to get the Hash Table Hash from the Catalog '" + catalogFile + "'.  The catalog appears to be corrupt.");
				}
				ImageSigner.CRYPTCATMEMBER cryptcatmember = (ImageSigner.CRYPTCATMEMBER)Marshal.PtrToStructure(intPtr3, typeof(ImageSigner.CRYPTCATMEMBER));
				array = new byte[20];
				int offset = (int)(cryptcatmember.sEncodedIndirectData.cbData - (uint)array.Length);
				Marshal.Copy(IntPtr.Add(cryptcatmember.sEncodedIndirectData.pbData, offset), array, 0, array.Length);
			}
			catch (Exception ex)
			{
				throw new ImageCommonException("ImageCommon!ImageSigner::GetCatalogHash: Failed to get the Hash Table Hash from the Catalog: " + ex.Message);
			}
			finally
			{
				if (intPtr2 != intPtr)
				{
					ImageSigner.CryptCATClose(intPtr2);
					intPtr2 = intPtr;
				}
			}
			return array;
		}

		// Token: 0x04000116 RID: 278
		private FullFlashUpdateImage _ffuImage;

		// Token: 0x04000117 RID: 279
		private string _catalogFileName;

		// Token: 0x04000118 RID: 280
		private IULogger _logger;

		// Token: 0x04000119 RID: 281
		private SHA256 _sha256;

		// Token: 0x0400011A RID: 282
		private static Dictionary<string, bool> certPublicKeys = new Dictionary<string, bool>();

		// Token: 0x0400011B RID: 283
		public const string ProdCertRootThumbprint = "3B1EFD3A66EA28B16697394703A72CA340A05BD5";

		// Token: 0x0400011C RID: 284
		public const string TestCertRootThumbprint = "8A334AA8052DD244A647306A76B8178FA215F344";

		// Token: 0x0400011D RID: 285
		public const string FlightCertPCAThumbprint = "9E594333273339A97051B0F82E86F266B917EDB3";

		// Token: 0x0400011E RID: 286
		public const string FlightCertWindowsThumbprint = "5f444a6740b7ca2434c7a5925222c2339ee0f1b7";

		// Token: 0x0200007C RID: 124
		[CLSCompliant(false)]
		public struct CRYPT_ATTR_BLOB
		{
			// Token: 0x040002C1 RID: 705
			public uint cbData;

			// Token: 0x040002C2 RID: 706
			public IntPtr pbData;
		}

		// Token: 0x0200007D RID: 125
		[CLSCompliant(false)]
		public struct CRYPTCATMEMBER
		{
			// Token: 0x040002C3 RID: 707
			private uint cbStruct;

			// Token: 0x040002C4 RID: 708
			[MarshalAs(UnmanagedType.LPWStr)]
			private string pwszReferenceTag;

			// Token: 0x040002C5 RID: 709
			[MarshalAs(UnmanagedType.LPWStr)]
			private string pwszFileName;

			// Token: 0x040002C6 RID: 710
			private Guid gSubjectType;

			// Token: 0x040002C7 RID: 711
			private uint fdwMemberFlags;

			// Token: 0x040002C8 RID: 712
			private IntPtr pIndirectData;

			// Token: 0x040002C9 RID: 713
			private uint dwCertVersion;

			// Token: 0x040002CA RID: 714
			private uint dwReserved;

			// Token: 0x040002CB RID: 715
			private IntPtr hReserved;

			// Token: 0x040002CC RID: 716
			public ImageSigner.CRYPT_ATTR_BLOB sEncodedIndirectData;

			// Token: 0x040002CD RID: 717
			private ImageSigner.CRYPT_ATTR_BLOB sEncodedMemberInfo;
		}
	}
}
