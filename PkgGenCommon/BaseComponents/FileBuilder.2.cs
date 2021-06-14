using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200003E RID: 62
	public abstract class FileBuilder<T, V> where T : PkgFile, new() where V : FileBuilder<T, V>
	{
		// Token: 0x060000EF RID: 239 RVA: 0x0000562E File Offset: 0x0000382E
		internal FileBuilder(XElement fileElement)
		{
			this.pkgObject = fileElement.FromXElement<T>();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00005642 File Offset: 0x00003842
		internal FileBuilder(string source, string destinationDir)
		{
			this.pkgObject = Activator.CreateInstance<T>();
			this.pkgObject.SourcePath = source;
			this.pkgObject.DestinationDir = destinationDir;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00005678 File Offset: 0x00003878
		public V SetAttributes(string value)
		{
			value = new Regex("\\s+", RegexOptions.Compiled).Replace(value, ",");
			FileAttributes attributes;
			if (!Enum.TryParse<FileAttributes>(value, out attributes))
			{
				throw new ArgumentException("Argument cannot be parsed.");
			}
			return this.SetAttributes(attributes);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000056BC File Offset: 0x000038BC
		public V SetAttributes(FileAttributes attributes)
		{
			if ((attributes & ~(PkgConstants.c_validAttributes != (FileAttributes)0)) != (FileAttributes)0)
			{
				throw new ArgumentException(string.Format("Valid attributes for packaging are: {0}", PkgConstants.c_validAttributes.ToString()), "attributes");
			}
			this.pkgObject.Attributes = attributes;
			return (V)((object)this);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00005712 File Offset: 0x00003912
		public V SetDestinationDir(string destinationDir)
		{
			this.pkgObject.DestinationDir = destinationDir;
			return (V)((object)this);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000572B File Offset: 0x0000392B
		public V SetName(string name)
		{
			this.pkgObject.Name = name;
			return (V)((object)this);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00005744 File Offset: 0x00003944
		internal virtual T ToPkgObject()
		{
			return this.pkgObject;
		}

		// Token: 0x040000EA RID: 234
		protected T pkgObject;
	}
}
