using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Mobile;
using Microsoft.Phone.TestInfra.Deployment;

namespace Microsoft.Phone.TestInfra.DeployTest
{
	// Token: 0x02000002 RID: 2
	public class DeployTest
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static int Main(string[] args)
		{
			DeployTest deployTest = new DeployTest();
			string rootPath = string.Empty;
			string alternateRoots = string.Empty;
			string packages = string.Empty;
			string packageFile = string.Empty;
			string empty = string.Empty;
			string text = string.Empty;
			string cacheRoot = string.Empty;
			int num = 24;
			TraceLevel traceLevel = TraceLevel.Info;
			TraceLevel traceLevel2 = TraceLevel.Info;
			bool recurse = false;
			bool sourceRootIsVolatile = false;
			bool flag = false;
			bool flag2 = false;
			List<string> list = new List<string>();
			string logFile = null;
			bool flag3 = args.Length == 0;
			int result;
			if (flag3)
			{
				DeployTest.Usage();
				result = 0;
			}
			else
			{
				try
				{
					int i = 0;
					while (i < args.Length)
					{
						bool flag4 = Regex.IsMatch(args[i], "^[/-]out$", RegexOptions.IgnoreCase);
						int num2;
						if (flag4)
						{
							num2 = i;
							i = num2 + 1;
							bool flag5 = i < args.Length;
							if (!flag5)
							{
								Console.Error.WriteLine("ERROR: -out requires path");
								DeployTest.Usage();
								return -1;
							}
							text = args[i];
						}
						else
						{
							bool flag6 = Regex.IsMatch(args[i], "^[/-]root$", RegexOptions.IgnoreCase);
							if (flag6)
							{
								num2 = i;
								i = num2 + 1;
								bool flag7 = i < args.Length;
								if (!flag7)
								{
									Console.Error.WriteLine("ERROR: -root requires path");
									DeployTest.Usage();
									return -2;
								}
								rootPath = args[i];
							}
							else
							{
								bool flag8 = Regex.IsMatch(args[i], "^[/-]altroot$", RegexOptions.IgnoreCase);
								if (flag8)
								{
									num2 = i;
									i = num2 + 1;
									bool flag9 = i < args.Length;
									if (!flag9)
									{
										Console.Error.WriteLine("ERROR: -altroot requires path");
										DeployTest.Usage();
										return -3;
									}
									alternateRoots = args[i];
								}
								else
								{
									bool flag10 = Regex.IsMatch(args[i], "^[/-]pkg$", RegexOptions.IgnoreCase);
									if (flag10)
									{
										num2 = i;
										i = num2 + 1;
										bool flag11 = i < args.Length;
										if (!flag11)
										{
											Console.Error.WriteLine("ERROR: -pkg requires path");
											DeployTest.Usage();
											return -4;
										}
										packages = args[i];
									}
									else
									{
										bool flag12 = Regex.IsMatch(args[i], "^[/-]pkgfile$", RegexOptions.IgnoreCase);
										if (flag12)
										{
											num2 = i;
											i = num2 + 1;
											bool flag13 = i < args.Length;
											if (!flag13)
											{
												Console.Error.WriteLine("ERROR: -pkgfile requires file");
												DeployTest.Usage();
												return -5;
											}
											packageFile = args[i];
										}
										else
										{
											bool flag14 = Regex.IsMatch(args[i], "^[/-]cache$", RegexOptions.IgnoreCase);
											if (flag14)
											{
												num2 = i;
												i = num2 + 1;
												bool flag15 = i < args.Length;
												if (!flag15)
												{
													Console.Error.WriteLine("ERROR: -cache requires path");
													DeployTest.Usage();
													return -6;
												}
												cacheRoot = args[i];
											}
											else
											{
												bool flag16 = Regex.IsMatch(args[i], "^[/-]robocopy$", RegexOptions.IgnoreCase);
												if (flag16)
												{
													num2 = i;
													i = num2 + 1;
													bool flag17 = i >= args.Length;
													if (flag17)
													{
														Console.Error.WriteLine("ERROR: -robocopy requires <option> parameter");
														DeployTest.Usage();
														return -7;
													}
												}
												else
												{
													bool flag18 = Regex.IsMatch(args[i], "^[/-]expiresIn$", RegexOptions.IgnoreCase);
													if (flag18)
													{
														num2 = i;
														i = num2 + 1;
														bool flag19 = i < args.Length;
														if (!flag19)
														{
															Console.Error.WriteLine("ERROR: -expiresIn requires <option> parameter");
															DeployTest.Usage();
															return -8;
														}
														int num3;
														bool flag20 = !int.TryParse(args[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out num3);
														if (!flag20)
														{
															Console.Error.WriteLine("ERROR: -expiresIn value cannot be parsed");
															DeployTest.Usage();
															return -8;
														}
														bool flag21 = num3 >= 1;
														if (!flag21)
														{
															Console.Error.WriteLine("ERROR: -expiresIn value should be at least 1 (hour)");
															DeployTest.Usage();
															return -8;
														}
														num = num3;
													}
													else
													{
														bool flag22 = Regex.IsMatch(args[i], "^[/-]ConsoleOutputLevel$", RegexOptions.IgnoreCase);
														if (flag22)
														{
															num2 = i;
															i = num2 + 1;
															bool flag23 = i < args.Length;
															if (!flag23)
															{
																Console.Error.WriteLine("ERROR: -ConsoleOutputLevel requires a TraceLevel");
																DeployTest.Usage();
																return -9;
															}
															bool flag24 = Enum.TryParse<TraceLevel>(args[i], true, out traceLevel);
															if (!flag24)
															{
																Console.Error.WriteLine("ERROR: {0} is not a valid TraceLevel", args[i]);
																DeployTest.Usage();
																return -9;
															}
														}
														else
														{
															bool flag25 = Regex.IsMatch(args[i], "^[/-]FileOutputLevel$", RegexOptions.IgnoreCase);
															if (flag25)
															{
																num2 = i;
																i = num2 + 1;
																bool flag26 = i < args.Length;
																if (!flag26)
																{
																	Console.Error.WriteLine("ERROR: -FileOutputLevel requires a TraceLevel");
																	DeployTest.Usage();
																	return -9;
																}
																bool flag27 = Enum.TryParse<TraceLevel>(args[i], true, out traceLevel2);
																if (!flag27)
																{
																	Console.Error.WriteLine("ERROR: {0} is not a valid TraceLevel", args[i]);
																	DeployTest.Usage();
																	return -9;
																}
															}
															else
															{
																bool flag28 = Regex.IsMatch(args[i], "^[/-]?verbose$", RegexOptions.IgnoreCase);
																if (flag28)
																{
																	Console.WriteLine("-verbose is deprecated.  Use '-ConsoleOutputLevel Verbose' for verbose console log.");
																}
																else
																{
																	bool flag29 = Regex.IsMatch(args[i], "^[/-]?fileverbose$", RegexOptions.IgnoreCase);
																	if (flag29)
																	{
																		Console.WriteLine("-fileverbose is deprecated.  Use '-FileOutputLevel Verbose' for verbose console log.");
																	}
																	else
																	{
																		bool flag30 = Regex.IsMatch(args[i], "^[/-]?recurse$", RegexOptions.IgnoreCase);
																		if (flag30)
																		{
																			recurse = true;
																		}
																		else
																		{
																			bool flag31 = Regex.IsMatch(args[i], "^[/-]?norobo$", RegexOptions.IgnoreCase);
																			if (!flag31)
																			{
																				bool flag32 = Regex.IsMatch(args[i], "^[/-]volatilesource", RegexOptions.IgnoreCase);
																				if (flag32)
																				{
																					sourceRootIsVolatile = true;
																				}
																				else
																				{
																					bool flag33 = Regex.IsMatch(args[i], "^[/-]RunConfigAction", RegexOptions.IgnoreCase);
																					if (flag33)
																					{
																						flag = true;
																					}
																					else
																					{
																						bool flag34 = Regex.IsMatch(args[i], "^[/-]GenerateGeneralCache", RegexOptions.IgnoreCase);
																						if (flag34)
																						{
																							flag2 = true;
																						}
																						else
																						{
																							bool flag35 = Regex.IsMatch(args[i], "^[/-]LogFile$", RegexOptions.IgnoreCase);
																							if (flag35)
																							{
																								num2 = i;
																								i = num2 + 1;
																								bool flag36 = i < args.Length;
																								if (!flag36)
																								{
																									Console.Error.WriteLine("ERROR: -LogFile requires path");
																									DeployTest.Usage();
																									return -10;
																								}
																								logFile = args[i];
																							}
																							else
																							{
																								bool flag37 = args[i].Contains("=");
																								if (!flag37)
																								{
																									Console.Error.WriteLine("ERROR: parameter '{0}' not supported", args[i]);
																									DeployTest.Usage();
																									return -12;
																								}
																								string[] array = args[i].Split(new char[]
																								{
																									'='
																								}, StringSplitOptions.RemoveEmptyEntries);
																								bool flag38 = array.Length == 2;
																								if (!flag38)
																								{
																									Console.Error.WriteLine("Macro {0} wrong length {1}", array[0], array.Length);
																									DeployTest.Usage();
																									return -11;
																								}
																								list.Add(args[i]);
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						IL_6B9:
						num2 = i;
						i = num2 + 1;
						continue;
						goto IL_6B9;
					}
				}
				catch (ArgumentException ex)
				{
					Console.Error.WriteLine("ERROR: Path has invalid characters. Check that there are no trailing backslashes in paths.");
					Logger.Error(ex.ToString(), new object[0]);
					DeployTest.Usage();
					return -13;
				}
				catch (Exception ex2)
				{
					Console.Error.WriteLine("ERROR: Unknown exception while parsing arguments.");
					Logger.Error(ex2.ToString(), new object[0]);
					DeployTest.Usage();
					return -14;
				}
				bool flag39 = flag2;
				if (flag39)
				{
					try
					{
						Logger.Configure(TraceLevel.Info, TraceLevel.Info, "RootCacheGenerate.log", true);
						Logger.Info("Start generating root cache files ...", new object[0]);
						GeneralCacheGenerator.DoWork(text, rootPath);
						Logger.Info("Root cache files are generated in {0}", new object[]
						{
							Path.GetFullPath(text)
						});
					}
					catch (Exception ex3)
					{
						Logger.Error("Error Occurred: {0}", new object[]
						{
							ex3.ToString()
						});
						return 1;
					}
					finally
					{
						Logger.Close();
					}
					result = 0;
				}
				else
				{
					string macros = string.Join(";", list.ToArray());
					PackageDeployerParameters packageDeployerParameters = new PackageDeployerParameters(text, rootPath)
					{
						Packages = packages,
						PackageFile = packageFile,
						AlternateRoots = alternateRoots,
						ExpiresIn = TimeSpan.FromHours((double)num),
						CacheRoot = cacheRoot,
						Macros = macros,
						SourceRootIsVolatile = sourceRootIsVolatile,
						Recurse = recurse,
						ConsoleTraceLevel = traceLevel,
						FileTraceLevel = traceLevel2,
						LogFile = logFile
					};
					PackageDeployer packageDeployer = new PackageDeployer(packageDeployerParameters);
					PackageDeployerOutput packageDeployerOutput = packageDeployer.Run();
					bool flag40 = !packageDeployerOutput.Success || !flag || packageDeployerOutput.ConfigurationCommands == null || packageDeployerOutput.ConfigurationCommands.Count == 0;
					if (flag40)
					{
						result = (packageDeployerOutput.Success ? 0 : 1);
					}
					else
					{
						bool flag41 = true;
						try
						{
							Logger.Configure(traceLevel, traceLevel2, packageDeployer.LogFile, true);
							Logger.Info("Found config commands to run after package is deployed.", new object[0]);
							foreach (ConfigCommand configCommand in packageDeployerOutput.ConfigurationCommands)
							{
								Logger.Info("Running config command {0}.", new object[]
								{
									configCommand.CommandLine
								});
								TimeSpan timeout = TimeSpan.FromMinutes(3.0);
								ProcessLauncher processLauncher = new ProcessLauncher("cmd.exe", "/c " + configCommand.CommandLine, delegate(string m)
								{
									bool flag44 = !string.IsNullOrWhiteSpace(m);
									if (flag44)
									{
										Logger.Info("Command Output: " + m, new object[0]);
									}
								}, delegate(string m)
								{
									bool flag44 = !string.IsNullOrWhiteSpace(m);
									if (flag44)
									{
										Logger.Info("Command Output: " + m, new object[0]);
									}
								}, delegate(string m)
								{
									bool flag44 = !string.IsNullOrWhiteSpace(m);
									if (flag44)
									{
										Logger.Info("Command Output: " + m, new object[0]);
									}
								})
								{
									TimeoutHandler = delegate(Process p)
									{
										throw new TimeoutException(string.Format("Process {0} did not exit in {1} minutes", p.StartInfo.FileName, timeout.Minutes));
									}
								};
								processLauncher.RunToExit(Convert.ToInt32(timeout.TotalMilliseconds, CultureInfo.InvariantCulture));
								bool flag42 = !processLauncher.Process.HasExited;
								if (flag42)
								{
									processLauncher.Process.Kill();
									Logger.Error("Error: Process {0} has not exited. Killed.", new object[]
									{
										processLauncher
									});
									flag41 = false;
								}
								else
								{
									bool flag43 = !configCommand.IgnoreExitCode && processLauncher.Process.ExitCode != configCommand.SuccessExitCode;
									if (flag43)
									{
										Logger.Error("{0} return an error exit code {1}", new object[]
										{
											configCommand.CommandLine,
											processLauncher.Process.ExitCode
										});
										flag41 = false;
									}
									else
									{
										Logger.Info("{0} returns.", new object[]
										{
											configCommand.CommandLine
										});
									}
								}
							}
							Logger.Info("Configuration commands are done.", new object[0]);
						}
						catch (Exception ex4)
						{
							flag41 = false;
							Logger.Error("Exception: {0}", new object[]
							{
								ex4.ToString()
							});
						}
						finally
						{
							Logger.Close();
						}
						result = (flag41 ? 0 : 1);
					}
				}
			}
			return result;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002C1C File Offset: 0x00000E1C
		private static void Usage()
		{
			Console.WriteLine("DeployTest");
			Console.WriteLine("    Deploys test packages and dependencies.");
			Console.WriteLine("\nUsage:");
			Console.WriteLine("    DeployTest -out <OutputPath> -root <BinaryRoot> -pkg <PkgName>");
			Console.WriteLine("               [-pkgfile <PkgFile>] [-altroot <AltRoot>] ");
			Console.WriteLine("               [-cache <CacheDir>] [MACRO=<value>] [MACRO2=<value2>] ...");
			Console.WriteLine("\nParameters:");
			Console.WriteLine("    -out <OutputPath>   : Directory for output files.");
			Console.WriteLine("    -root <BinaryRoot>  : Location of Binary Root. If you specify multiple paths,");
			Console.WriteLine("                          separate them with semicolons. Default is current");
			Console.WriteLine("                          $(BINARY_ROOT) value in Phone Build environment or .");
			Console.WriteLine("                          $(_NTTREE) value in Razzle Build environment or .");
			Console.WriteLine("    -pkg <PkgName>      : Name of package, will search under");
			Console.WriteLine("                          BinaryRoot\\Prebuilt to find the package.");
			Console.WriteLine("                          Can also be \"PkgName;PkgName2;PkgName3\".");
			Console.WriteLine("                          You can specify optional tags in each PkgName as");
			Console.WriteLine("                          \"PkgName[tags=Tag1,Tag2,Tag3]\".");
			Console.WriteLine("    -pkgfile <PkgFile>  : File with package names, one per line.");
			Console.WriteLine("    -altroot <AltRoot>  : Alternate location to search for packages");
			Console.WriteLine("                          (like \\\\build\\release\\<...>).");
			Console.WriteLine("    -cache <CacheDir>   : Location to cache local copies of packages, meant for");
			Console.WriteLine("                          lab test machines.");
			Console.WriteLine("    -expiresIn <value>  : Deployment folder expiration time, in hours.");
			Console.WriteLine("                          Default is 24.");
			Console.WriteLine("    -recurse            : Default is to only process the first level of dep.xml");
			Console.WriteLine("                          Add -recurse to process for all packages.");
			Console.WriteLine("    -RunConfigAction    : If the package contains configuration actions, whether run");
			Console.WriteLine("                          these actions. Default is no.");
			Console.WriteLine("    -ConsoleOutputLevel : Maximum verbosity level to output to console.");
			Console.WriteLine("                          Default is Info.  Values are Off, Error, Warning, Info, Verbose.");
			Console.WriteLine("    -FileOutputLevel    : Maximum verbosity level to output to the log file.");
			Console.WriteLine("                          Default is Info.  Values are Off, Error, Warning, Info, Verbose.");
			Console.WriteLine("    -LogFile            : The path to a log file to use.");
			Console.WriteLine("    -norobo             : [Ignored] Default is to use RoboCopy to copy files.");
			Console.WriteLine("    -robocopy <Options> : [Ignored] Overrides the default RoboCopy options.");
			Console.WriteLine("\nUsage: ");
			Console.WriteLine("   DeployTest -GenerateGeneralCache -out <OutputPath> -root <PhoneBuildPath>");
			Console.WriteLine("    -GenerateGeneralCache: The general cache files are generated when this option is on.");
			Console.WriteLine("                           A txt file containing binary names that should be appended to ");
			Console.WriteLine("                           pkgdep_supress.txt is also generated.");
			Console.WriteLine("                           This option is for TDX team internal use only.");
			Console.WriteLine("                           When this option is on, the options other than -out and -root are ignored");
			Console.WriteLine("                           Note that the root path must be a public phone build path. ");
			Console.WriteLine("We can deduce the winbuild path from phone build path, but cannot do it the other way.");
		}
	}
}
