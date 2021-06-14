using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Mock
{
	// Token: 0x0200000A RID: 10
	[ComVisible(true)]
	internal abstract class EventInfo : MemberInfo
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000BF RID: 191
		public abstract EventAttributes Attributes { get; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00002087 File Offset: 0x00000287
		public virtual Type EventHandlerType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00003300 File Offset: 0x00001500
		public bool IsMulticast
		{
			get
			{
				return this.GetIsMulticastImpl();
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00002087 File Offset: 0x00000287
		protected virtual bool GetIsMulticastImpl()
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00003318 File Offset: 0x00001518
		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & EventAttributes.SpecialName) > EventAttributes.None;
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000333C File Offset: 0x0000153C
		public MethodInfo GetAddMethod()
		{
			return this.GetAddMethod(false);
		}

		// Token: 0x060000C5 RID: 197
		public abstract MethodInfo GetAddMethod(bool nonPublic);

		// Token: 0x060000C6 RID: 198 RVA: 0x00003358 File Offset: 0x00001558
		public MethodInfo GetRaiseMethod()
		{
			return this.GetRaiseMethod(false);
		}

		// Token: 0x060000C7 RID: 199
		public abstract MethodInfo GetRaiseMethod(bool nonPublic);

		// Token: 0x060000C8 RID: 200 RVA: 0x00003374 File Offset: 0x00001574
		public MethodInfo GetRemoveMethod()
		{
			return this.GetRemoveMethod(false);
		}

		// Token: 0x060000C9 RID: 201
		public abstract MethodInfo GetRemoveMethod(bool nonPublic);

		// Token: 0x060000CA RID: 202 RVA: 0x00003390 File Offset: 0x00001590
		public MethodInfo[] GetOtherMethods()
		{
			return this.GetOtherMethods(false);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00002087 File Offset: 0x00000287
		public virtual MethodInfo[] GetOtherMethods(bool nonPublic)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000033A9 File Offset: 0x000015A9
		public void AddEventHandler(object target, Delegate handler)
		{
			this.AddEventHandlerImpl(target, handler);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00002087 File Offset: 0x00000287
		protected virtual void AddEventHandlerImpl(object target, Delegate handler)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000033B5 File Offset: 0x000015B5
		public void RemoveEventHandler(object target, Delegate handler)
		{
			this.RemoveEventHandlerImpl(target, handler);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00002087 File Offset: 0x00000287
		protected virtual void RemoveEventHandlerImpl(object target, Delegate handler)
		{
			throw new NotImplementedException();
		}
	}
}
