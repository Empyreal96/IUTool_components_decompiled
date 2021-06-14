using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000020 RID: 32
	[SuppressMessage("Microsoft.Design", "CA1058:TypesShouldNotExtendCertainBaseTypes")]
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	public class CommandSpecificationCollection : CollectionBase
	{
		// Token: 0x17000034 RID: 52
		public CommandSpecification this[string name]
		{
			get
			{
				foreach (object obj in base.InnerList)
				{
					CommandSpecification commandSpecification = (CommandSpecification)obj;
					bool flag = string.Compare(name, commandSpecification.Name, true, CultureInfo.CurrentCulture) == 0;
					if (flag)
					{
						return commandSpecification;
					}
					bool flag2 = commandSpecification.CommandAliases.Count > 0;
					if (flag2)
					{
						foreach (string strB in commandSpecification.CommandAliases)
						{
							bool flag3 = string.Compare(name, strB, true, CultureInfo.CurrentCulture) == 0;
							if (flag3)
							{
								return commandSpecification;
							}
						}
					}
				}
				return null;
			}
		}

		// Token: 0x17000035 RID: 53
		public CommandSpecification this[int index]
		{
			get
			{
				return base.InnerList[index] as CommandSpecification;
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00004130 File Offset: 0x00002330
		public void CopyTo(CommandSpecification[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000456C File Offset: 0x0000276C
		public int Add(CommandSpecification specification)
		{
			return ((IList)this).Add(specification);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004588 File Offset: 0x00002788
		public bool Contains(CommandSpecification specification)
		{
			return ((IList)this).Contains(specification);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000045A1 File Offset: 0x000027A1
		public void Insert(int index, CommandSpecification specification)
		{
			((IList)this).Insert(index, specification);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000045AD File Offset: 0x000027AD
		public void Remove(CommandSpecification specification)
		{
			((IList)this).Remove(specification);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000045B8 File Offset: 0x000027B8
		public int IndexOf(CommandSpecification specification)
		{
			return ((IList)this).IndexOf(specification);
		}
	}
}
