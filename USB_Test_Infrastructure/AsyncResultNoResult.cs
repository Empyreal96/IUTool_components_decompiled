using System;
using System.Threading;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000003 RID: 3
	internal class AsyncResultNoResult : IAsyncResult
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002079 File Offset: 0x00000279
		public AsyncCallback AsyncCallback
		{
			get
			{
				return this.asyncCallback;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002081 File Offset: 0x00000281
		public AsyncResultNoResult(AsyncCallback asyncCallback, object state)
		{
			this.asyncCallback = asyncCallback;
			this.asyncState = state;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002098 File Offset: 0x00000298
		public void SetAsCompleted(Exception exception, bool completedSynchronously)
		{
			this.exception = exception;
			if (Interlocked.Exchange(ref this.completedState, completedSynchronously ? 1 : 2) != 0)
			{
				throw new InvalidOperationException("You can set a result only once");
			}
			if (this.asyncWaitHandle != null)
			{
				this.asyncWaitHandle.Set();
			}
			if (this.asyncCallback != null)
			{
				this.asyncCallback(this);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020F3 File Offset: 0x000002F3
		public void EndInvoke()
		{
			if (!this.IsCompleted)
			{
				this.AsyncWaitHandle.WaitOne();
				this.AsyncWaitHandle.Close();
				this.asyncWaitHandle = null;
			}
			if (this.exception != null)
			{
				throw this.exception;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000212A File Offset: 0x0000032A
		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002132 File Offset: 0x00000332
		public bool CompletedSynchronously
		{
			get
			{
				return Thread.VolatileRead(ref this.completedState) == 1;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002144 File Offset: 0x00000344
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.asyncWaitHandle == null)
				{
					bool isCompleted = this.IsCompleted;
					ManualResetEvent manualResetEvent = new ManualResetEvent(isCompleted);
					if (Interlocked.CompareExchange<ManualResetEvent>(ref this.asyncWaitHandle, manualResetEvent, null) != null)
					{
						manualResetEvent.Close();
					}
					else if (!isCompleted && this.IsCompleted)
					{
						this.asyncWaitHandle.Set();
					}
				}
				return this.asyncWaitHandle;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000219B File Offset: 0x0000039B
		public bool IsCompleted
		{
			get
			{
				return Thread.VolatileRead(ref this.completedState) != 0;
			}
		}

		// Token: 0x04000002 RID: 2
		private readonly AsyncCallback asyncCallback;

		// Token: 0x04000003 RID: 3
		private readonly object asyncState;

		// Token: 0x04000004 RID: 4
		private const int statePending = 0;

		// Token: 0x04000005 RID: 5
		private const int stateCompletedSynchronously = 1;

		// Token: 0x04000006 RID: 6
		private const int stateCompletedAsynchronously = 2;

		// Token: 0x04000007 RID: 7
		private int completedState;

		// Token: 0x04000008 RID: 8
		private ManualResetEvent asyncWaitHandle;

		// Token: 0x04000009 RID: 9
		private Exception exception;
	}
}
