using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Images;

namespace ImgDump
{
	// Token: 0x02000002 RID: 2
	internal class ImgDump
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool fSummary = false;
			ImageInfo imageInfo = null;
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			bool flag7 = false;
			bool flag8 = false;
			TextWriter textWriter = null;
			if (args.Count<string>() < 2)
			{
				ImgDump.DisplayUsage();
				return;
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			int i;
			for (i = 2; i < args.Count<string>(); i++)
			{
				if (args[i].ToLower(CultureInfo.InvariantCulture) == "all")
				{
					flag = true;
					break;
				}
				if (args[i].ToLower(CultureInfo.InvariantCulture) == "metadata")
				{
					flag = false;
					flag2 = true;
				}
				else if (args[i].ToLower(CultureInfo.InvariantCulture) == "partitions")
				{
					flag = false;
					flag3 = true;
				}
				else if (args[i].ToLower(CultureInfo.InvariantCulture) == "packages:summary")
				{
					flag = false;
					flag4 = true;
					fSummary = true;
				}
				else if (args[i].ToLower(CultureInfo.InvariantCulture) == "packages" || args[i].ToLower(CultureInfo.InvariantCulture) == "packages:details")
				{
					flag = false;
					flag4 = true;
				}
				else if (args[i].ToLower(CultureInfo.InvariantCulture) == "store")
				{
					flag = false;
					flag5 = true;
				}
				else if (args[i].ToLower(CultureInfo.InvariantCulture) == "registry")
				{
					flag = false;
					flag6 = true;
				}
				else
				{
					if (args[i].ToLower(CultureInfo.InvariantCulture) == "certificatesonly")
					{
						flag = false;
						flag7 = true;
						break;
					}
					if (!(args[i].ToLower(CultureInfo.InvariantCulture) == "subdirectories"))
					{
						Console.WriteLine("Unrecongized commandline parameter '{0}'", args[i]);
						ImgDump.DisplayUsage();
						return;
					}
					flag8 = true;
				}
			}
			if (i < args.Count<string>() && !flag8)
			{
				while (i < args.Count<string>())
				{
					if (args[i].ToLower(CultureInfo.InvariantCulture) == "subdirectories")
					{
						flag8 = true;
						break;
					}
					i++;
				}
			}
			string text2 = args[0];
			string searchPattern;
			string path;
			if (text2.Contains("\\") || text2.Contains(":"))
			{
				text2 = Path.GetFullPath(args[0]);
				int num = text2.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase);
				searchPattern = text2.Substring(num + 1);
				path = text2.Substring(0, num + 1);
			}
			else
			{
				searchPattern = args[0];
				path = Environment.CurrentDirectory;
			}
			string[] files;
			if (flag8)
			{
				files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
			}
			else
			{
				files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
			}
			if (files.Count<string>() == 0)
			{
				Console.WriteLine("Image file: \"" + args[0] + "\" could not be found.");
				Console.WriteLine("");
				ImgDump.DisplayUsage();
				return;
			}
			FileStream fileStream = new FileStream(args[1], FileMode.Create, FileAccess.Write, FileShare.Read);
			TextWriter textWriter2;
			textWriter = (textWriter2 = new StreamWriter(fileStream));
			try
			{
				try
				{
					foreach (string text3 in files)
					{
						Console.WriteLine("Loading image '{0}' .... ", text3);
						try
						{
							imageInfo = new ImageInfo(text3, text);
						}
						catch (Exception ex)
						{
							throw new Exception("Failed to load image: " + ex.Message, ex.InnerException);
						}
						textWriter.Write(imageInfo.DisplayFileName());
						if (flag || flag2)
						{
							Console.WriteLine("Writing metadata .... ");
							textWriter.Write(imageInfo.DisplayMetadata());
						}
						if (flag || flag5)
						{
							Console.WriteLine("Writing store info .... ");
							textWriter.Write(imageInfo.DisplayStore());
						}
						if (flag || flag3)
						{
							Console.WriteLine("Writing Partition info .... ");
							textWriter.Write(imageInfo.DisplayPartitions());
						}
						if (flag || flag4)
						{
							Console.WriteLine("Writing package info .... ");
							textWriter.Write(imageInfo.DisplayPackages(fSummary));
						}
						if (flag7)
						{
							Console.WriteLine("Writing Certificate report .... ");
							textWriter.Write(imageInfo.DisplayDefaultCerts());
						}
						if (flag || flag6)
						{
							Console.WriteLine("Writing registry info .... ");
							textWriter.Write(imageInfo.DisplayRegistry());
						}
					}
					Console.WriteLine("Completed writing to '{0}'", args[1]);
					Environment.ExitCode = 0;
				}
				catch (Exception ex2)
				{
					Console.WriteLine("Failed processing image: " + ex2.Message);
					Environment.ExitCode = 1;
				}
				finally
				{
					if (fileStream != null)
					{
						fileStream.Flush();
						textWriter.Close();
						fileStream.Close();
						fileStream = null;
					}
					if (imageInfo != null)
					{
						imageInfo.Dispose();
						imageInfo = null;
					}
				}
			}
			finally
			{
				if (textWriter2 != null)
				{
					((IDisposable)textWriter2).Dispose();
				}
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000252C File Offset: 0x0000072C
		private static void DisplayUsage()
		{
			Console.WriteLine("ImgDump <ImageFileName> <OutputFile> [All] [Metadata] [Partitions] [Packages] [Packages:Summary] [CertificatesOnly] [Subdirectories]");
			Console.WriteLine("");
			Console.WriteLine("ImageFileName- Image file can be a FFU or VHD");
			Console.WriteLine("OutputFile- File to output report");
			Console.WriteLine("[All]- Optional\\Default: Displays all information");
			Console.WriteLine("[Metadata]- Optional: Displays the Metadata section.");
			Console.WriteLine("[Store]- Optional: Displays the Store information.");
			Console.WriteLine("[Partitions]- Optional: Displays the Partition information.");
			Console.WriteLine("[Registry]- Optional: Displays the Registry information.");
			Console.WriteLine("[Packages]- Optional: Displays the Package information.");
			Console.WriteLine("[Packages:Summary]- Optional: Does not list all the file info for each package.");
			Console.WriteLine("[CertificatesOnly]- Optional: Only displays the Certs data");
			Console.WriteLine("[Subdirectories]- Optional: Process all images matching <ImageFileName> in subdirectories.");
			Console.WriteLine("\tExamples:");
			Console.WriteLine("\t\tImgDump flash.ffu Dump.txt");
			Console.WriteLine("\t\tImgDump flash.ffu Dump.txt Metadata");
			Console.WriteLine("\t\tImgDump flash.ffu Dump.txt Metadata Partitions");
			Console.WriteLine("\t\tImgDump flash.vhd Dump.txt Packages");
			Console.WriteLine("\t\tImgDump flash.vhd Dump.txt Partitions Packages:Summary");
			Console.WriteLine("\t\tImgDump *.ffu Dump.txt CertificatesOnly");
			Console.WriteLine("\t\tImgDump *.vhd Dump.txt CertificatesOnly Subdirectories");
		}
	}
}
