using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Composition.ToolBox.IO
{
	// Token: 0x0200001A RID: 26
	public class DirectoryToolBox
	{
		// Token: 0x06000087 RID: 135 RVA: 0x000038B7 File Offset: 0x00001AB7
		public static bool Exists(string directory)
		{
			return LongPathIO.Exists(directory, LongPathIO.SystemType.Directory);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000038C0 File Offset: 0x00001AC0
		public static void Create(string dirpath)
		{
			if (DirectoryToolBox.Exists(dirpath))
			{
				return;
			}
			string[] array = dirpath.Split(new char[]
			{
				'\\'
			});
			dirpath = array[0] + "\\" + array[1];
			for (int i = 2; i < array.Length; i++)
			{
				dirpath = dirpath + "\\" + array[i];
				if (!DirectoryToolBox.Exists(dirpath))
				{
					int num = NativeMethods.CreateDirectory(LongPathIO.UNCPath(dirpath), IntPtr.Zero);
					if (num == 0)
					{
						throw new Exception(string.Format("DirectoryToolBox::Create: Kernel32 failed to create directory '{0}'. Error={1}", dirpath, num));
					}
				}
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000394C File Offset: 0x00001B4C
		public static void Delete(string dirpath)
		{
			string text = LongPathIO.UNCPath(dirpath);
			if (!DirectoryToolBox.Exists(text))
			{
				throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Delete: Directory '{0}' does not exist.", text));
			}
			if (!LongPathIO.IsDirectory(text))
			{
				throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Delete Target '{0}' is not a directory.", text));
			}
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
			IntPtr intPtr = NativeMethods.FindFirstFile(Path.Combine(text, "*.*"), out win32_FIND_DATA);
			try
			{
				if (intPtr == LongPathIO.InvalidHandleValue)
				{
					throw new Exception(string.Format("DirectoryToolBox::Paths Native FindFirstFile failed to load '{0}'. Error={1}", dirpath, Marshal.GetLastWin32Error()));
				}
				string text2;
				int num;
				for (;;)
				{
					if (!win32_FIND_DATA.cFileName.Equals(".") && !win32_FIND_DATA.cFileName.Equals(".."))
					{
						if (LongPathIO.IsFile(win32_FIND_DATA.dwFileAttributes))
						{
							FileToolBox.Delete(PathToolBox.Combine(dirpath, win32_FIND_DATA.cFileName));
						}
						else
						{
							text2 = PathToolBox.Combine(dirpath, win32_FIND_DATA.cFileName);
							DirectoryToolBox.Delete(text2);
							num = NativeMethods.RemoveDirectory(LongPathIO.UNCPath(text2));
							if (num == 0)
							{
								break;
							}
						}
					}
					if (!(NativeMethods.FindNextFile(intPtr, out win32_FIND_DATA) != LongPathIO.NullHandleValue))
					{
						goto Block_10;
					}
				}
				throw new Exception(string.Format("Kernel32 failed to delete directory '{0}'. Error={1}", text2, num));
				Block_10:;
			}
			finally
			{
				if (LongPathIO.ValidHandle(intPtr))
				{
					NativeMethods.FindClose(intPtr);
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003A90 File Offset: 0x00001C90
		public static string GetDirectoryFromFilePath(string path)
		{
			if (LongPathIO.IsDirectory(path))
			{
				return path;
			}
			return path.Substring(0, path.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase));
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003AB0 File Offset: 0x00001CB0
		public static void Copy(string source, string target, bool recursive)
		{
			if (!DirectoryToolBox.Exists(source))
			{
				throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Copy: Target directory '{0}' does not exist.", source));
			}
			if (!DirectoryToolBox.Exists(target))
			{
				DirectoryToolBox.Create(target);
			}
			List<string> list = DirectoryToolBox.Files(source, recursive, false);
			List<string> list2 = new List<string>(list);
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i] = PathToolBox.RelativePath(list2[i], source);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				string text = Path.Combine(target, list2[j]);
				string text2 = text.Substring(0, text.LastIndexOf('\\'));
				if (!DirectoryToolBox.Exists(text2))
				{
					DirectoryToolBox.Create(text2);
				}
				FileToolBox.Copy(list[j], text);
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00003B6A File Offset: 0x00001D6A
		public static List<string> Files(string source, bool recursive, bool relative)
		{
			return DirectoryToolBox.Paths(source, recursive, relative, false);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003B75 File Offset: 0x00001D75
		public static List<string> Directories(string source, bool recursive, bool relative)
		{
			return DirectoryToolBox.Paths(source, recursive, relative, true);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003B80 File Offset: 0x00001D80
		private static List<string> Paths(string source, bool recursive, bool relative, bool processDirectories)
		{
			string text = LongPathIO.UNCPath(source);
			if (!DirectoryToolBox.Exists(text))
			{
				throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Paths: Source directory '{0}' does not exist.", text));
			}
			if (!LongPathIO.IsDirectory(text))
			{
				throw new DirectoryNotFoundException(string.Format("DirectoryToolBox::Paths Target '{0}' is not a directory.", text));
			}
			List<string> list = new List<string>();
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
			IntPtr intPtr = NativeMethods.FindFirstFile(Path.Combine(text, "*.*"), out win32_FIND_DATA);
			try
			{
				if (intPtr == LongPathIO.InvalidHandleValue)
				{
					throw new Exception(string.Format("DirectoryToolBox::Paths Native FindFirstFile failed to load '{0}'. Error={1}", source, Marshal.GetLastWin32Error()));
				}
				do
				{
					if (!win32_FIND_DATA.cFileName.Equals(".") && !win32_FIND_DATA.cFileName.Equals(".."))
					{
						if (LongPathIO.IsFile(win32_FIND_DATA.dwFileAttributes))
						{
							if (!processDirectories)
							{
								list.Add(Path.Combine(source, win32_FIND_DATA.cFileName));
							}
						}
						else
						{
							if (processDirectories)
							{
								list.AddRange(DirectoryToolBox.Directories(Path.Combine(source, win32_FIND_DATA.cFileName), recursive, false));
								list.Add(Path.Combine(source, win32_FIND_DATA.cFileName));
							}
							if (recursive)
							{
								list.AddRange(DirectoryToolBox.Files(Path.Combine(source, win32_FIND_DATA.cFileName), recursive, false));
							}
						}
					}
				}
				while (NativeMethods.FindNextFile(intPtr, out win32_FIND_DATA) != LongPathIO.NullHandleValue);
			}
			finally
			{
				if (LongPathIO.ValidHandle(intPtr))
				{
					NativeMethods.FindClose(intPtr);
				}
			}
			if (relative)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i] = PathToolBox.RelativePath(list[i], source);
				}
			}
			return list;
		}
	}
}
