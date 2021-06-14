using System;
using System.Threading;

namespace FFUComponents
{
	// Token: 0x0200000C RID: 12
	internal class AsyncResultNoResult : IAsyncResult
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000030FB File Offset: 0x000012FB
		public AsyncCallback AsyncCallback
		{
			get
			{
				return this.asyncCallback;
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003103 File Offset: 0x00001303
		public AsyncResultNoResult(AsyncCallback asyncCallback, object state)
		{
			this.asyncCallback = asyncCallback;
			this.asyncState = state;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000311C File Offset: 0x0000131C
		public void SetAsCompleted(Exception exception, bool completedSynchronously)
		{
			this.exception = exception;
			if (Interlocked.Exchange(ref this.completedState, completedSynchronously ? 1 : 2) != 0)
			{
				throw new InvalidOperationException(Resources.ERROR_RESULT_ALREADY_SET);
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

		// Token: 0x06000053 RID: 83 RVA: 0x00003178 File Offset: 0x00001378
		public void EndInvoke()
		{
			if (!this.IsCompleted)
			{
				TimeSpan timeout = TimeSpan.FromMinutes(2.0);
				try
				{
					if (!this.AsyncWaitHandle.WaitOne(timeout, false))
					{
						throw new TimeoutException();
					}
				}
				finally
				{
					this.AsyncWaitHandle.Close();
					this.asyncWaitHandle = null;
				}
			}
			if (this.exception != null)
			{
				throw this.exception;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000031E8 File Offset: 0x000013E8
		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000031F0 File Offset: 0x000013F0
		public bool CompletedSynchronously
		{
			get
			{
				return Thread.VolatileRead(ref this.completedState) == 1;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003200 File Offset: 0x00001400
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003257 File Offset: 0x00001457
		public bool IsCompleted
		{
			get
			{
				return Thread.VolatileRead(ref this.completedState) != 0;
			}
		}

		// Token: 0x04000036 RID: 54
		private readonly AsyncCallback asyncCallback;

		// Token: 0x04000037 RID: 55
		private readonly object asyncState;

		// Token: 0x04000038 RID: 56
		private const int statePending = 0;

		// Token: 0x04000039 RID: 57
		private const int stateCompletedSynchronously = 1;

		// Token: 0x0400003A RID: 58
		private const int stateCompletedAsynchronously = 2;

		// Token: 0x0400003B RID: 59
		private int completedState;

		// Token: 0x0400003C RID: 60
		private ManualResetEvent asyncWaitHandle;

		// Token: 0x0400003D RID: 61
		private Exception exception;
	}
}
