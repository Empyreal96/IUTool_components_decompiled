using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Composition.ToolBox.IO
{
	// Token: 0x02000019 RID: 25
	public class FileToolBox
	{
		// Token: 0x0600007C RID: 124 RVA: 0x000035AD File Offset: 0x000017AD
		public static bool Exists(string filepath)
		{
			return LongPathIO.Exists(filepath, LongPathIO.SystemType.File);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000035B6 File Offset: 0x000017B6
		public static string Extension(string filepath)
		{
			if (LongPathIO.IsDirectory(filepath))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Extension: The path '{0}' points to a directory not a file.", filepath));
			}
			return filepath.Substring(filepath.LastIndexOf('.'));
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000035E0 File Offset: 0x000017E0
		public static bool GlobalExists(string filepath)
		{
			if (LongPathIO.Exists(filepath, LongPathIO.SystemType.File))
			{
				return true;
			}
			string[] array = Environment.GetEnvironmentVariable("PATH").Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (LongPathIO.Exists(Path.Combine(array[i], filepath), LongPathIO.SystemType.File))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002434 File Offset: 0x00000634
		public static void Copy(string source, string dest)
		{
			FileToolBox.Copy(source, dest, true);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003638 File Offset: 0x00001838
		public static void Copy(string source, string dest, bool overrideExisting)
		{
			if (LongPathIO.IsDirectory(source))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Copy: The path '{0}' points to a directory not a file.", source));
			}
			if (!FileToolBox.Exists(source))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Copy: Cannot copy file '{0}'. The file was not found on disk.", source));
			}
			if (!FileToolBox.Exists(dest) && LongPathIO.IsDirectory(dest))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Copy: You cannot override the directory '{0}' with the file '{1}'.", dest, source));
			}
			string directoryFromFilePath = DirectoryToolBox.GetDirectoryFromFilePath(dest);
			if (!DirectoryToolBox.Exists(directoryFromFilePath))
			{
				DirectoryToolBox.Create(directoryFromFilePath);
			}
			int num = 0;
			while (num < FileToolBox.IntRetryCount && !NativeMethods.CopyFile(LongPathIO.UNCPath(source), LongPathIO.UNCPath(dest), !overrideExisting))
			{
				if (num == FileToolBox.IntRetryCount - 1)
				{
					throw new Exception(string.Format("FileToolBox::Copy: Native copy file failed to copy '{0}' to '{1}'. Error={2}", source, dest, Marshal.GetLastWin32Error()));
				}
				Thread.Sleep(FileToolBox.IntSleepTimeMs);
				num++;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003708 File Offset: 0x00001908
		public static void Delete(string filepath)
		{
			if (LongPathIO.IsDirectory(filepath))
			{
				throw new Exception(string.Format("FileToolBox::Delete: FileToolBox was used to delete directory '{0}'.", filepath));
			}
			if (!FileToolBox.Exists(filepath))
			{
				return;
			}
			NativeMethods.SetFileAttributes(LongPathIO.UNCPath(filepath), NativeMethods.EFileAttributes.Normal);
			if (!NativeMethods.DeleteFile(LongPathIO.UNCPath(filepath)))
			{
				throw new Exception(string.Format("FileToolBox::Delete: Native delete file failed to delete '{0}'. Error={1}", filepath, Marshal.GetLastWin32Error()));
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003770 File Offset: 0x00001970
		public static FileStream Stream(string filepath, FileAccess access)
		{
			string text = LongPathIO.UNCPath(filepath);
			if (!FileToolBox.Exists(text))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Stream: File '{0}' was not found on disk.", text));
			}
			NativeMethods.EFileShare dwShareMode = NativeMethods.EFileShare.Read | NativeMethods.EFileShare.Write | NativeMethods.EFileShare.Delete;
			NativeMethods.EFileAccess efileAccess = (NativeMethods.EFileAccess)0U;
			if (access == FileAccess.Read || access == FileAccess.ReadWrite)
			{
				efileAccess |= (NativeMethods.EFileAccess)2147483648U;
			}
			if (access == FileAccess.Write || access == FileAccess.ReadWrite)
			{
				efileAccess |= NativeMethods.EFileAccess.GenericWrite;
			}
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(text, efileAccess, dwShareMode, IntPtr.Zero, NativeMethods.ECreationDisposition.OpenExisting, (NativeMethods.EFileAttributes)0U, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			return new FileStream(safeFileHandle, access);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000037F0 File Offset: 0x000019F0
		public static long Size(string filepath)
		{
			string text = LongPathIO.UNCPath(filepath);
			if (!FileToolBox.Exists(text))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::Size File '{0}' was not found on disk.", text));
			}
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
			if (NativeMethods.FindFirstFile(text, out win32_FIND_DATA) == LongPathIO.InvalidHandleValue)
			{
				throw new Exception(string.Format("FileToolBox::Size: Native FindFirstFile failed to load '{0}'. Error={1}", filepath, Marshal.GetLastWin32Error()));
			}
			return (long)win32_FIND_DATA.nFileSizeLow + (long)win32_FIND_DATA.nFileSizeHigh * 4294967296L;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003870 File Offset: 0x00001A70
		public static void ImagingSizes(string filepath, out ulong fileSize, out ulong stagedSize, out ulong compressedSize)
		{
			string text = LongPathIO.UNCPath(filepath);
			if (!FileToolBox.Exists(text))
			{
				throw new FileNotFoundException(string.Format("FileToolBox::ImagingSizes: File '{0}' was not found on disk.", text));
			}
			NativeMethods.IU_GetStagedAndCompressedSize(text, out fileSize, out stagedSize, out compressedSize);
		}

		// Token: 0x04000062 RID: 98
		public static readonly int IntRetryCount = 20;

		// Token: 0x04000063 RID: 99
		public static readonly int IntSleepTimeMs = 100;
	}
}
