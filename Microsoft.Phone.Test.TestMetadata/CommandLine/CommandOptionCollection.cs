using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001E RID: 30
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	[Serializable]
	public class CommandOptionCollection : NameObjectCollectionBase
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00004002 File Offset: 0x00002202
		public CommandOptionCollection()
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000400C File Offset: 0x0000220C
		protected CommandOptionCollection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004018 File Offset: 0x00002218
		public void Add(CommandOption option)
		{
			bool flag = option == null;
			if (flag)
			{
				throw new ArgumentNullException("option");
			}
			base.BaseAdd(option.Name.ToLower(CultureInfo.CurrentUICulture), option);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004054 File Offset: 0x00002254
		public void Remove(CommandOption option)
		{
			bool flag = option == null;
			if (flag)
			{
				throw new ArgumentNullException("option");
			}
			this.Remove(option.Name);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004084 File Offset: 0x00002284
		public void Remove(string optionName)
		{
			bool flag = optionName == null;
			if (flag)
			{
				throw new ArgumentNullException("optionName");
			}
			base.BaseRemove(optionName.ToLower(CultureInfo.CurrentUICulture));
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000040B8 File Offset: 0x000022B8
		public bool Contains(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			return base.BaseGet(name.ToLower(CultureInfo.CurrentUICulture)) != null;
		}

		// Token: 0x1700002B RID: 43
		public CommandOption this[string name]
		{
			get
			{
				bool flag = name == null;
				if (flag)
				{
					throw new ArgumentNullException("name");
				}
				return (CommandOption)base.BaseGet(name.ToLower(CultureInfo.CurrentUICulture));
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004130 File Offset: 0x00002330
		public void CopyTo(CommandOption[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000413C File Offset: 0x0000233C
		public void Insert(int index, CommandOption option)
		{
			((IList)this).Insert(index, option);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004150 File Offset: 0x00002350
		public int IndexOf(CommandOption option)
		{
			return ((IList)this).IndexOf(option);
		}
	}
}
