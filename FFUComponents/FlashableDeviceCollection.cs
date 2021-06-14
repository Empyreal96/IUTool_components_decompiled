using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000012 RID: 18
	[ComVisible(false)]
	public class FlashableDeviceCollection : IEnumerator
	{
		// Token: 0x0600006E RID: 110 RVA: 0x000033C8 File Offset: 0x000015C8
		public FlashableDeviceCollection(ICollection<IFFUDevice> aColl)
		{
			this.theList = new List<FlashableDevice>();
			for (int i = 0; i < aColl.Count; i++)
			{
				this.theList.Add(new FlashableDevice(aColl.ElementAt(i)));
			}
			this.theEnum = this.theList.GetEnumerator();
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00003424 File Offset: 0x00001624
		public object Current
		{
			get
			{
				return this.theEnum.Current;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003431 File Offset: 0x00001631
		public bool MoveNext()
		{
			return this.theEnum.MoveNext();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000343E File Offset: 0x0000163E
		public void Reset()
		{
			this.theEnum.Reset();
		}

		// Token: 0x04000040 RID: 64
		private List<FlashableDevice> theList;

		// Token: 0x04000041 RID: 65
		private IEnumerator<FlashableDevice> theEnum;
	}
}
