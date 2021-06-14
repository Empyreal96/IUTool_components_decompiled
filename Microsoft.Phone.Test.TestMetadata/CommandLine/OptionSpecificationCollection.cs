using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x02000027 RID: 39
	[SuppressMessage("Microsoft.Design", "CA1058:TypesShouldNotExtendCertainBaseTypes")]
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	public class OptionSpecificationCollection : CollectionBase
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00004CF4 File Offset: 0x00002EF4
		public void Add(OptionSpecification specification)
		{
			bool flag = specification == null;
			if (flag)
			{
				throw new OptionSpecificationException("Specification passed to add is null");
			}
			bool flag2 = this.Contains(specification.OptionName);
			if (flag2)
			{
				throw new OptionSpecificationException(string.Format(CultureInfo.CurrentCulture, "Option {0} is specified twice", new object[]
				{
					specification.OptionName
				}));
			}
			base.InnerList.Add(specification);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00004D58 File Offset: 0x00002F58
		public bool Contains(string optionName)
		{
			foreach (object obj in base.InnerList)
			{
				OptionSpecification optionSpecification = (OptionSpecification)obj;
				bool flag = string.Compare(optionSpecification.OptionName, optionName, true, CultureInfo.CurrentCulture) == 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00004DD8 File Offset: 0x00002FD8
		public bool IsDisjoint(OptionSpecificationCollection otherCollection)
		{
			bool flag = otherCollection == null;
			if (flag)
			{
				throw new OptionSpecificationException("Collection specified is null");
			}
			foreach (object obj in base.InnerList)
			{
				OptionSpecification optionSpecification = (OptionSpecification)obj;
				bool flag2 = otherCollection.Contains(optionSpecification.OptionName);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x17000046 RID: 70
		public OptionSpecification this[string optionName]
		{
			get
			{
				foreach (object obj in base.InnerList)
				{
					OptionSpecification optionSpecification = (OptionSpecification)obj;
					bool flag = string.Compare(optionSpecification.OptionName, optionName, true, CultureInfo.CurrentUICulture) == 0;
					if (flag)
					{
						return optionSpecification;
					}
				}
				return null;
			}
		}

		// Token: 0x17000047 RID: 71
		public OptionSpecification this[int index]
		{
			get
			{
				return (OptionSpecification)((IList)this)[index];
			}
			set
			{
				((IList)this)[index] = value;
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004130 File Offset: 0x00002330
		public void CopyTo(OptionSpecification[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000045A1 File Offset: 0x000027A1
		public void Insert(int index, OptionSpecification specification)
		{
			((IList)this).Insert(index, specification);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000045AD File Offset: 0x000027AD
		public void Remove(OptionSpecification specification)
		{
			((IList)this).Remove(specification);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00004F10 File Offset: 0x00003110
		public int IndexOf(OptionSpecification specification)
		{
			return ((IList)this).IndexOf(specification);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00004F2C File Offset: 0x0000312C
		public IList<OptionSpecification> GetPartial(string partialOptionName)
		{
			bool flag = partialOptionName == null;
			if (flag)
			{
				throw new OptionSpecificationException("Option name cannot be null");
			}
			List<OptionSpecification> list = new List<OptionSpecification>();
			foreach (object obj in base.InnerList)
			{
				OptionSpecification optionSpecification = (OptionSpecification)obj;
				bool flag2 = string.Compare(partialOptionName, 0, optionSpecification.OptionName, 0, partialOptionName.Length, true, CultureInfo.CurrentCulture) == 0;
				if (flag2)
				{
					list.Add(optionSpecification);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00004FDC File Offset: 0x000031DC
		public void LoadFromType(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new OptionSpecificationException("Type passed is null");
			}
			object[] customAttributes = type.GetCustomAttributes(typeof(OptionAttribute), true);
			foreach (OptionAttribute attribute in customAttributes)
			{
				OptionSpecification specification = new OptionSpecification(attribute, null);
				this.Add(specification);
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				object[] customAttributes2 = propertyInfo.GetCustomAttributes(typeof(OptionAttribute), true);
				bool flag2 = customAttributes2.Length == 1;
				if (flag2)
				{
					bool flag3 = propertyInfo.GetSetMethod() == null && !typeof(IList).IsAssignableFrom(propertyInfo.PropertyType);
					if (flag3)
					{
						throw new OptionSpecificationException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' has no setter so it cannot be used as an option.", new object[]
						{
							propertyInfo.Name
						}));
					}
					OptionAttribute attribute2 = customAttributes2[0] as OptionAttribute;
					OptionSpecification specification2 = new OptionSpecification(attribute2, propertyInfo);
					this.Add(specification2);
				}
				else
				{
					bool flag4 = customAttributes2.Length == 0;
					if (!flag4)
					{
						throw new OptionSpecificationException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' has been marked with two or more options", new object[]
						{
							propertyInfo.Name
						}));
					}
				}
			}
		}
	}
}
