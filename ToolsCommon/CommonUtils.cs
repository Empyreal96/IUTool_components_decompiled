using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000024 RID: 36
	public static class CommonUtils
	{
		// Token: 0x0600012D RID: 301 RVA: 0x0000767C File Offset: 0x0000587C
		public static IntPtr MountVHD(string vhdPath, bool fReadOnly)
		{
			VIRTUAL_DISK_ACCESS_MASK accessMask = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_ALL;
			if (fReadOnly)
			{
				accessMask = VIRTUAL_DISK_ACCESS_MASK.VIRTUAL_DISK_ACCESS_READ;
			}
			OPEN_VIRTUAL_DISK_FLAG openFlags = OPEN_VIRTUAL_DISK_FLAG.OPEN_VIRTUAL_DISK_FLAG_NONE;
			ATTACH_VIRTUAL_DISK_FLAG attachFlags = ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_NONE;
			if (fReadOnly)
			{
				attachFlags = ATTACH_VIRTUAL_DISK_FLAG.ATTACH_VIRTUAL_DISK_FLAG_READ_ONLY;
			}
			return CommonUtils.MountVHD(vhdPath, accessMask, openFlags, attachFlags);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000076AC File Offset: 0x000058AC
		public static IntPtr MountVHD(string vhdPath, VIRTUAL_DISK_ACCESS_MASK accessMask, OPEN_VIRTUAL_DISK_FLAG openFlags, ATTACH_VIRTUAL_DISK_FLAG attachFlags)
		{
			IntPtr zero = IntPtr.Zero;
			VIRTUAL_STORAGE_TYPE virtual_STORAGE_TYPE = default(VIRTUAL_STORAGE_TYPE);
			virtual_STORAGE_TYPE.DeviceId = VHD_STORAGE_TYPE_DEVICE.VIRTUAL_STORAGE_TYPE_DEVICE_VHD;
			virtual_STORAGE_TYPE.VendorId = VIRTUAL_STORAGE_TYPE_VENDOR.VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT;
			OPEN_VIRTUAL_DISK_PARAMETERS open_VIRTUAL_DISK_PARAMETERS = default(OPEN_VIRTUAL_DISK_PARAMETERS);
			open_VIRTUAL_DISK_PARAMETERS.Version = OPEN_VIRTUAL_DISK_VERSION.OPEN_VIRTUAL_DISK_VERSION_1;
			open_VIRTUAL_DISK_PARAMETERS.RWDepth = 1U;
			int num = VirtualDiskLib.OpenVirtualDisk(ref virtual_STORAGE_TYPE, vhdPath, accessMask, openFlags, ref open_VIRTUAL_DISK_PARAMETERS, ref zero);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			ATTACH_VIRTUAL_DISK_PARAMETERS attach_VIRTUAL_DISK_PARAMETERS = default(ATTACH_VIRTUAL_DISK_PARAMETERS);
			attach_VIRTUAL_DISK_PARAMETERS.Version = ATTACH_VIRTUAL_DISK_VERSION.ATTACH_VIRTUAL_DISK_VERSION_1;
			num = VirtualDiskLib.AttachVirtualDisk(zero, IntPtr.Zero, attachFlags, 0U, ref attach_VIRTUAL_DISK_PARAMETERS, IntPtr.Zero);
			if (0 < num)
			{
				throw new Win32Exception(num);
			}
			return zero;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007740 File Offset: 0x00005940
		public static void DismountVHD(IntPtr hndlVirtDisk)
		{
			if (hndlVirtDisk == IntPtr.Zero)
			{
				return;
			}
			int num = VirtualDiskLib.DetachVirtualDisk(hndlVirtDisk, DETACH_VIRTUAL_DISK_FLAG.DETACH_VIRTUAL_DISK_FLAG_NONE, 0U);
			if (0 < num)
			{
				throw new Win32Exception();
			}
			VirtualDiskLib.CloseHandle(hndlVirtDisk);
		}

		// Token: 0x06000130 RID: 304
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int IU_MountWim(string WimPath, string MountPath, string TemporaryPath);

		// Token: 0x06000131 RID: 305
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int IU_DismountWim(string WimPath, string MountPath, int CommitMode);

		// Token: 0x06000132 RID: 306 RVA: 0x00007775 File Offset: 0x00005975
		public static bool MountWIM(string wimPath, string mountPoint, string tmpDir)
		{
			return CommonUtils.IU_MountWim(wimPath, mountPoint, tmpDir) == 0;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00007782 File Offset: 0x00005982
		public static bool DismountWIM(string wimPath, string mountPoint, bool commit)
		{
			return CommonUtils.IU_DismountWim(wimPath, mountPoint, commit ? 1 : 0) == 0;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00007798 File Offset: 0x00005998
		public static string FindInPath(string filename)
		{
			string text;
			if (LongPathFile.Exists(Path.Combine(Environment.CurrentDirectory, filename)))
			{
				text = Environment.CurrentDirectory;
			}
			else
			{
				text = Environment.GetEnvironmentVariable("PATH").Split(new char[]
				{
					';'
				}).FirstOrDefault((string x) => LongPathFile.Exists(Path.Combine(x, filename)));
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FileNotFoundException(string.Format("Can't find file '{0}' anywhere in the %PATH%", filename));
			}
			return Path.Combine(text, filename);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000782C File Offset: 0x00005A2C
		public static int RunProcess(string workingDir, string command, string args, bool hiddenWindow)
		{
			string text = null;
			return CommonUtils.RunProcess(workingDir, command, args, hiddenWindow, false, out text);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007848 File Offset: 0x00005A48
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

		// Token: 0x06000137 RID: 311 RVA: 0x0000786C File Offset: 0x00005A6C
		public static int RunProcessVerbose(string command, string args)
		{
			string value = null;
			int result = CommonUtils.RunProcess(null, command, args, true, true, out value);
			Console.WriteLine(value);
			return result;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007890 File Offset: 0x00005A90
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

		// Token: 0x06000139 RID: 313 RVA: 0x0000797C File Offset: 0x00005B7C
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

		// Token: 0x0600013A RID: 314 RVA: 0x000079D4 File Offset: 0x00005BD4
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

		// Token: 0x0600013B RID: 315 RVA: 0x00007A70 File Offset: 0x00005C70
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

		// Token: 0x0600013C RID: 316 RVA: 0x00007AB0 File Offset: 0x00005CB0
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

		// Token: 0x0600013D RID: 317 RVA: 0x00007AEF File Offset: 0x00005CEF
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPrincipal)]
		public static bool IsCurrentUserAdmin()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole("BUILTIN\\\\Administrators");
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00007B05 File Offset: 0x00005D05
		public static string GetSha256Hash(byte[] buffer)
		{
			return BitConverter.ToString(CommonUtils.Sha256Algorithm.ComputeHash(buffer)).Replace("-", string.Empty);
		}

		// Token: 0x04000065 RID: 101
		private const int S_OK = 0;

		// Token: 0x04000066 RID: 102
		private const int WimNoCommit = 0;

		// Token: 0x04000067 RID: 103
		private const int WimCommit = 1;

		// Token: 0x04000068 RID: 104
		private static readonly HashAlgorithm Sha256Algorithm = HashAlgorithm.Create("SHA256");
	}
}
