using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CompPlat.PkgBldr.Base.Tools;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000035 RID: 53
	public static class Run
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x00007C44 File Offset: 0x00005E44
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string RunSPkgGen(List<string> pkgGenArgs, PkgBldrCmd pkgBldrArgs, List<string> spkgList = null)
		{
			return Run.RunSPkgGen(pkgGenArgs, new Logger(), pkgBldrArgs, spkgList);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007C53 File Offset: 0x00005E53
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string RunSPkgGen(List<string> pkgGenArgs, IDeploymentLogger logger, PkgBldrCmd pkgBldrArgs, List<string> spkgList = null)
		{
			return Run.RunSPkgGen(pkgGenArgs, false, logger, pkgBldrArgs, spkgList);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00007C5F File Offset: 0x00005E5F
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string RunSPkgGen(List<string> pkgGenArgs, bool inWindows, PkgBldrCmd pkgBldrArgs, List<string> spkgList = null)
		{
			return Run.RunSPkgGen(pkgGenArgs, inWindows, new Logger(), pkgBldrArgs, spkgList);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007C70 File Offset: 0x00005E70
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string RunSPkgGen(List<string> pkgGenArgs, bool inWindows, IDeploymentLogger logger, PkgBldrCmd pkgBldrArgs, List<string> spkgList = null)
		{
			string text = null;
			if (spkgList != null)
			{
				text = Path.GetTempFileName();
				pkgGenArgs.Add(string.Format(CultureInfo.InvariantCulture, "/spkgsout:{0}", new object[]
				{
					text
				}));
				spkgList.Clear();
			}
			string command;
			string text2;
			string workingDir;
			if (inWindows)
			{
				command = Environment.ExpandEnvironmentVariables(pkgBldrArgs.toolPaths["urtrun"]);
				text2 = Environment.ExpandEnvironmentVariables(string.Format(CultureInfo.InvariantCulture, "4.0 {0} ", new object[]
				{
					pkgBldrArgs.toolPaths["spkggen"]
				}));
				workingDir = Environment.ExpandEnvironmentVariables(LongPath.GetDirectoryName(pkgBldrArgs.toolPaths["spkggen"]));
			}
			else
			{
				command = "SPkgGen.exe";
				text2 = null;
				workingDir = Directory.GetCurrentDirectory();
			}
			foreach (string str in pkgGenArgs)
			{
				text2 = text2 + str + " ";
			}
			string text3 = null;
			logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Running SPkgGen.exe {0}", new object[]
			{
				text2
			}), new object[0]);
			if (Run.RunProcess(workingDir, command, text2, false, true, out text3, null, null) != 0)
			{
				throw new PkgGenException(string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					text3
				}));
			}
			if (pkgGenArgs.Contains("/diagnostic"))
			{
				logger.LogSpkgGenOutput(text3);
			}
			if (spkgList != null)
			{
				List<string> collection = LongPathFile.ReadAllLines(text).ToList<string>();
				spkgList.AddRange(collection);
				LongPathFile.Delete(text);
			}
			return text3;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007DFC File Offset: 0x00005FFC
		public static string RunDsmConverter(string spkg, bool wow, bool ignoreConvertDsmError, bool testOnly)
		{
			return Run.RunDsmConverter(spkg, LongPath.GetDirectoryName(spkg), wow, ignoreConvertDsmError, testOnly);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007E10 File Offset: 0x00006010
		public static string RunDsmConverter(string input, string output, bool wow, bool ignoreConvertDsmError, bool testOnly)
		{
			uint num = 11U;
			if (testOnly)
			{
				num |= 4096U;
			}
			ConvertDSM.RunDsmConverter(input, output, wow, ignoreConvertDsmError, num);
			return string.Format(CultureInfo.InvariantCulture, "Completed converting package {0}.", new object[]
			{
				input
			});
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007E4F File Offset: 0x0000604F
		public static string RunProcess(string workingDirectory, string processName, string arguments)
		{
			return Run.RunProcess(workingDirectory, processName, arguments, new Logger());
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007E60 File Offset: 0x00006060
		[SuppressMessage("Microsoft.Design", "CA1011")]
		public static string RunProcess(string workingDirectory, string processName, string arguments, IDeploymentLogger logger)
		{
			string result = null;
			logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Running {0} {1}", new object[]
			{
				processName,
				arguments
			}), new object[0]);
			int num = Run.RunProcess(workingDirectory, processName, arguments, true, true, out result, null, null);
			if (num != 0)
			{
				throw new PkgGenException(string.Format(CultureInfo.InvariantCulture, "Call to {0} failed with error {1}", new object[]
				{
					processName,
					num
				}));
			}
			return result;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007ED3 File Offset: 0x000060D3
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string RunProcessQuiet(string workingDirectory, string processName, string arguments, string envName = null, string envValue = null)
		{
			return Run.RunProcessQuiet(workingDirectory, processName, arguments, new Logger(), envName, envValue);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007EE8 File Offset: 0x000060E8
		[SuppressMessage("Microsoft.Design", "CA1026")]
		[SuppressMessage("Microsoft.Design", "CA1011")]
		public static string RunProcessQuiet(string workingDirectory, string processName, string arguments, IDeploymentLogger logger, string envName = null, string envValue = null)
		{
			string text = null;
			int num = Run.RunProcess(workingDirectory, processName, arguments, true, true, out text, envName, envValue);
			if (num != 0)
			{
				logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Running {0} {1}", new object[]
				{
					processName,
					arguments
				}), new object[0]);
				logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "\n{0}\n", new object[]
				{
					text
				}), new object[0]);
				throw new PkgGenException(string.Format(CultureInfo.InvariantCulture, "Call to {0} failed with error {1}", new object[]
				{
					processName,
					num
				}));
			}
			return text;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007F82 File Offset: 0x00006182
		[SuppressMessage("Microsoft.Design", "CA1045")]
		public static string RunProcess(string workingDirectory, string processName, string arguments, ref int iExitCode)
		{
			return Run.RunProcess(workingDirectory, processName, arguments, new Logger(), ref iExitCode);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007F94 File Offset: 0x00006194
		[SuppressMessage("Microsoft.Design", "CA1045")]
		[SuppressMessage("Microsoft.Design", "CA1011")]
		public static string RunProcess(string workingDirectory, string processName, string arguments, IDeploymentLogger logger, ref int iExitCode)
		{
			string result = null;
			logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Running {0} {1}", new object[]
			{
				processName,
				arguments
			}), new object[0]);
			iExitCode = Run.RunProcess(workingDirectory, processName, arguments, true, true, out result, null, null);
			return result;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007FE0 File Offset: 0x000061E0
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static int RunProcess(string workingDir, string command, string args, bool hiddenWindow, bool captureOutput, out string processOutput, string envName = null, string envValue = null)
		{
			int result = 0;
			processOutput = string.Empty;
			command = Environment.ExpandEnvironmentVariables(command);
			args = Environment.ExpandEnvironmentVariables(args);
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			if (envName != null && envValue != null)
			{
				processStartInfo.EnvironmentVariables[envName] = envValue;
			}
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
						throw new PkgGenException("Run proccess failed");
					}
					result = process.ExitCode;
				}
			}
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000080D8 File Offset: 0x000062D8
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string BinPlace(string source, string rootDir, string path, PkgBldrCmd pkgBldrArgs, bool temporary = false)
		{
			return Run.BinPlace(source, rootDir, path, new Logger(), pkgBldrArgs, temporary);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000080EC File Offset: 0x000062EC
		[SuppressMessage("Microsoft.Design", "CA1026")]
		public static string BinPlace(string source, string rootDir, string path, IDeploymentLogger logger, PkgBldrCmd pkgBldrArgs, bool temporary = false)
		{
			rootDir = Environment.ExpandEnvironmentVariables(rootDir);
			if (!LongPathDirectory.Exists(rootDir))
			{
				throw new PkgGenException("Can't find root directory {0}", new object[]
				{
					rootDir
				});
			}
			if (!LongPathFile.Exists(source))
			{
				throw new PkgGenException("Can't find source file {0}", new object[]
				{
					source
				});
			}
			if (!temporary)
			{
				string arguments = string.Format(CultureInfo.InvariantCulture, "-LeaveBinaryAlone -r {0} -:DEST {1} {2}", new object[]
				{
					rootDir,
					path,
					source
				});
				Run.RunProcessQuiet(Environment.ExpandEnvironmentVariables(pkgBldrArgs.razzleToolPath), Environment.ExpandEnvironmentVariables(pkgBldrArgs.toolPaths["binplace"]), arguments, logger, null, null);
			}
			else
			{
				string text = LongPath.Combine(rootDir, path);
				LongPathDirectory.CreateDirectory(text);
				text = LongPath.Combine(text, LongPath.GetFileName(source));
				LongPathFile.Copy(source, text);
			}
			if (rootDir.Equals(Environment.ExpandEnvironmentVariables(pkgBldrArgs.buildNttree), StringComparison.OrdinalIgnoreCase))
			{
				rootDir = "$(build.nttree)";
			}
			return LongPath.Combine(rootDir, path);
		}
	}
}
