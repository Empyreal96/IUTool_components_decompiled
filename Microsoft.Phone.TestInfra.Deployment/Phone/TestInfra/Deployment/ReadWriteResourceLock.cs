using System;
using System.Security.Permissions;
using System.Threading;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002E RID: 46
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public sealed class ReadWriteResourceLock : IDisposable
	{
		// Token: 0x0600021E RID: 542 RVA: 0x0000DED4 File Offset: 0x0000C0D4
		public ReadWriteResourceLock(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			this.syncRoot = new object();
			this.mutex = new Mutex(false, "RWRL_M_" + name);
			this.semaphore = new Semaphore(ReadWriteResourceLock.MaxReaders, ReadWriteResourceLock.MaxReaders, "RWRL_S_" + name);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000DF44 File Offset: 0x0000C144
		~ReadWriteResourceLock()
		{
			this.Dispose(false);
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000DF78 File Offset: 0x0000C178
		public bool IsReadLockHeld
		{
			get
			{
				return this.readLockCount > 0;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000DF98 File Offset: 0x0000C198
		public bool IsWriteLockHeld
		{
			get
			{
				return this.writeLockCount > 0;
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000DFB5 File Offset: 0x0000C1B5
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000DFC7 File Offset: 0x0000C1C7
		public void AcquireReadLock()
		{
			this.AcquireReadLock(TimeoutHelper.InfiniteTimeSpan);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000DFD6 File Offset: 0x0000C1D6
		public void AcquireReadLock(int timeoutMilliseconds)
		{
			this.AcquireReadLock(TimeSpan.FromMilliseconds((double)timeoutMilliseconds));
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000DFE8 File Offset: 0x0000C1E8
		public void AcquireReadLock(TimeSpan timeout)
		{
			PerformanceCounters.Instance.TimeWaitingOnReadlock.Start();
			try
			{
				object obj = this.syncRoot;
				lock (obj)
				{
					this.VerifyNotDisposed();
					bool flag2 = this.writeLockCount > 0;
					if (!flag2)
					{
						bool flag3 = this.readLockCount > 0;
						if (flag3)
						{
							int num = this.readLockCount;
							this.readLockCount = num + 1;
						}
						else
						{
							TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
							bool flag4 = false;
							try
							{
								this.mutex.Acquire(timeoutHelper);
								flag4 = true;
								this.semaphore.Acquire(timeoutHelper);
							}
							finally
							{
								bool flag5 = flag4;
								if (flag5)
								{
									this.mutex.ReleaseMutex();
								}
							}
							int num = this.readLockCount;
							this.readLockCount = num + 1;
						}
					}
				}
			}
			finally
			{
				PerformanceCounters.Instance.TimeWaitingOnReadlock.Stop();
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E108 File Offset: 0x0000C308
		public bool TryToAcquireReadLock()
		{
			try
			{
				this.AcquireReadLock(TimeSpan.Zero);
			}
			catch (TimeoutException)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000E144 File Offset: 0x0000C344
		public void ReleaseReadLock()
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				this.VerifyNotDisposed();
				bool flag2 = this.readLockCount == 0 || this.writeLockCount > 0;
				if (flag2)
				{
					throw new SynchronizationLockException("The current application has not entered the lock in read mode.");
				}
				int num = this.readLockCount;
				this.readLockCount = num - 1;
				bool flag3 = this.readLockCount == 0;
				if (flag3)
				{
					this.semaphore.Release();
				}
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
		public void AcquireWriteLock()
		{
			this.AcquireWriteLock(TimeoutHelper.InfiniteTimeSpan);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000E1F7 File Offset: 0x0000C3F7
		public void AcquireWriteLock(int timeoutMilliseconds)
		{
			this.AcquireWriteLock(TimeSpan.FromMilliseconds((double)timeoutMilliseconds));
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000E208 File Offset: 0x0000C408
		public void AcquireWriteLock(TimeSpan timeout)
		{
			PerformanceCounters.Instance.TimeWaitingOnWritelock.Start();
			try
			{
				object obj = this.syncRoot;
				lock (obj)
				{
					this.VerifyNotDisposed();
					bool flag2 = this.writeLockCount > 0;
					if (flag2)
					{
						int num = this.writeLockCount;
						this.writeLockCount = num + 1;
					}
					else
					{
						TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
						while (this.readLockCount > 0)
						{
							this.ReleaseReadLock();
						}
						bool flag3 = false;
						int i = 0;
						int num;
						try
						{
							this.mutex.Acquire(timeoutHelper);
							flag3 = true;
							for (i = 0; i < ReadWriteResourceLock.MaxReaders; i = num + 1)
							{
								this.semaphore.Acquire(timeoutHelper);
								num = i;
							}
						}
						catch
						{
							bool flag4 = i > 0;
							if (flag4)
							{
								this.semaphore.Release(i);
							}
							throw;
						}
						finally
						{
							bool flag5 = flag3;
							if (flag5)
							{
								this.mutex.ReleaseMutex();
							}
						}
						num = this.writeLockCount;
						this.writeLockCount = num + 1;
					}
				}
			}
			finally
			{
				PerformanceCounters.Instance.TimeWaitingOnWritelock.Stop();
			}
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000E3B0 File Offset: 0x0000C5B0
		public bool TryToAcquireWriteLock()
		{
			try
			{
				this.AcquireWriteLock(TimeSpan.Zero);
			}
			catch (TimeoutException)
			{
				return false;
			}
			return true;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000E3EC File Offset: 0x0000C5EC
		public void ReleaseWriteLock()
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				this.VerifyNotDisposed();
				bool flag2 = this.writeLockCount == 0;
				if (flag2)
				{
					throw new SynchronizationLockException("The current application has not acquired write lock.");
				}
				int num = this.writeLockCount;
				this.writeLockCount = num - 1;
				bool flag3 = this.writeLockCount == 0;
				if (flag3)
				{
					this.semaphore.Release(ReadWriteResourceLock.MaxReaders);
				}
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000E488 File Offset: 0x0000C688
		private void VerifyNotDisposed()
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = this.disposed;
				if (flag2)
				{
					throw new ObjectDisposedException("Current instance is already disposed");
				}
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000E4E0 File Offset: 0x0000C6E0
		private void Dispose(bool disposing)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = this.disposed;
				if (!flag2)
				{
					bool flag3 = this.writeLockCount > 0;
					if (flag3)
					{
						this.ReleaseWriteLock();
					}
					bool flag4 = this.readLockCount > 0;
					if (flag4)
					{
						this.ReleaseReadLock();
					}
					bool flag5 = this.mutex != null;
					if (flag5)
					{
						this.mutex.Dispose();
						this.mutex = null;
					}
					bool flag6 = this.semaphore != null;
					if (flag6)
					{
						this.semaphore.Dispose();
						this.semaphore = null;
					}
					this.disposed = true;
				}
			}
		}

		// Token: 0x040000EC RID: 236
		private static readonly int MaxReaders = Settings.Default.MaxConcurrentReaders;

		// Token: 0x040000ED RID: 237
		private readonly object syncRoot;

		// Token: 0x040000EE RID: 238
		private Mutex mutex;

		// Token: 0x040000EF RID: 239
		private Semaphore semaphore;

		// Token: 0x040000F0 RID: 240
		private volatile int readLockCount;

		// Token: 0x040000F1 RID: 241
		private volatile int writeLockCount;

		// Token: 0x040000F2 RID: 242
		private volatile bool disposed;
	}
}
