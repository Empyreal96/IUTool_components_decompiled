using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000032 RID: 50
	public static class CommonUtils
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00007A1C File Offset: 0x00005C1C
		public static string FindInPath(string filename)
		{
			string text;
			if (LongPathFile.Exists(LongPath.Combine(Environment.CurrentDirectory, filename)))
			{
				text = Environment.CurrentDirectory;
			}
			else
			{
				text = Environment.GetEnvironmentVariable("PATH").Split(new char[]
				{
					';'
				}).FirstOrDefault((string x) => LongPathFile.Exists(LongPath.Combine(x, filename)));
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Can't find file '{0}' anywhere in the %PATH%", new object[]
				{
					filename
				}));
			}
			return LongPath.Combine(text, filename);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00007ABC File Offset: 0x00005CBC
		public static int RunProcess(string workingDir, string command, string args, bool hiddenWindow)
		{
			string text = null;
			return CommonUtils.RunProcess(workingDir, command, args, hiddenWindow, false, out text);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00007AD8 File Offset: 0x00005CD8
		public static int RunProcess(string command, string args)
		{
			string value = null;
			int num = CommonUtils.RunProcess(null, command, args, true, true, out value);
			if (num != 0)
			{
				Console.WriteLine(value);
			}
			return num;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00007AFC File Offset: 0x00005CFC
		private static int RunProcess(string workingDir, string command, string args, bool hiddenWindow, bool captureOutput, out string processOutput)
		{
			int result = 0;
			processOutput = string.Empty;
			command = Environment.ExpandEnvironmentVariables(command);
			args = Environment.ExpandEnvironmentVariables(args);
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.CreateNoWindow = true;
			if (hiddenWindow)
			{
				processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}
			if (workingDir != null)
			{
				processStartInfo.WorkingDirectory = workingDir;
			}
			processStartInfo.RedirectStandardInput = false;
			processStartInfo.RedirectStandardOutput = captureOutput;
			processStartInfo.UseShellExecute = !captureOutput;
			if (!string.IsNullOrEmpty(command) && !LongPathFile.Exists(command))
			{
				CommonUtils.FindInPath(command);
			}
			processStartInfo.FileName = command;
			processStartInfo.Arguments = args;
			using (Process process = Process.Start(processStartInfo))
			{
				if (process != null)
				{
					if (captureOutput)
					{
						processOutput = process.StandardOutput.ReadToEnd();
					}
					process.WaitForExit();
					if (!process.HasExited)
					{
						throw new IUException("Process <{0}> didn't exit correctly", new object[]
						{
							command
						});
					}
					result = process.ExitCode;
				}
			}
			return result;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00007BE8 File Offset: 0x00005DE8
		public static string BytesToHexicString(byte[] bytes)
		{
			if (bytes == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture.NumberFormat));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00007C40 File Offset: 0x00005E40
		public static byte[] HexicStringToBytes(string text)
		{
			if (text == null)
			{
				return new byte[0];
			}
			if (text.Length % 2 != 0)
			{
				throw new IUException("Incorrect length of a hexic string:\"{0}\"", new object[]
				{
					text
				});
			}
			List<byte> list = new List<byte>(text.Length / 2);
			for (int i = 0; i < text.Length; i += 2)
			{
				string text2 = text.Substring(i, 2);
				byte item;
				if (!byte.TryParse(text2, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out item))
				{
					throw new IUException("Failed to parse hexic string: \"{0}\"", new object[]
					{
						text2
					});
				}
				list.Add(item);
			}
			return list.ToArray();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00007CDC File Offset: 0x00005EDC
		public static bool ByteArrayCompare(byte[] array1, byte[] array2)
		{
			if (array1 == array2)
			{
				return true;
			}
			if (array1 == null || array2 == null)
			{
				return false;
			}
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00007D1C File Offset: 0x00005F1C
		public static string GetCopyrightString()
		{
			string format = "Microsoft (C) {0} {1}";
			string processName = Process.GetCurrentProcess().ProcessName;
			string currentAssemblyFileVersion = FileUtils.GetCurrentAssemblyFileVersion();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(format, processName, currentAssemblyFileVersion);
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00007D5B File Offset: 0x00005F5B
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPrincipal)]
		public static bool IsCurrentUserAdmin()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole("BUILTIN\\\\Administrators");
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00007D71 File Offset: 0x00005F71
		public static string GetSha256Hash(byte[] buffer)
		{
			return BitConverter.ToString(CommonUtils.Sha256Algorithm.ComputeHash(buffer)).Replace("-", string.Empty);
		}

		// Token: 0x0400007F RID: 127
		private static readonly HashAlgorithm Sha256Algorithm = HashAlgorithm.Create("SHA256");
	}
}
