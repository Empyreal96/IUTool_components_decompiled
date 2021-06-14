using System;
using System.Globalization;
using System.Threading;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000007 RID: 7
	internal class TelemetryEventSource : EventSource
	{
		// Token: 0x06000027 RID: 39 RVA: 0x0000269D File Offset: 0x0000089D
		public TelemetryEventSource(string eventSourceName) : base(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000026B0 File Offset: 0x000008B0
		protected TelemetryEventSource() : base(EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000026C4 File Offset: 0x000008C4
		public static EventSourceOptions TelemetryOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)35184372088832L
			};
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000026F0 File Offset: 0x000008F0
		public static EventSourceOptions MeasuresOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)70368744177664L
			};
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000271C File Offset: 0x0000091C
		public static EventSourceOptions CriticalDataOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)140737488355328L
			};
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002748 File Offset: 0x00000948
		[NonEvent]
		public void WriteTelemetry<T>(T data)
		{
			bool flag = base.IsEnabled();
			if (flag)
			{
				base.Write<T>(null, ref TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options, ref data);
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002778 File Offset: 0x00000978
		[NonEvent]
		public void WriteTelemetry<T>(ref Guid activityId, ref Guid relatedActivityId, ref T data)
		{
			bool flag = base.IsEnabled();
			if (flag)
			{
				base.Write<T>(null, ref TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options, ref activityId, ref relatedActivityId, ref data);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000027A8 File Offset: 0x000009A8
		public static EventSourceOptions GetEventSourceOptionsForType<T>()
		{
			return TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options;
		}

		// Token: 0x04000013 RID: 19
		public const EventKeywords Reserved44Keyword = (EventKeywords)17592186044416L;

		// Token: 0x04000014 RID: 20
		public const EventKeywords TelemetryKeyword = (EventKeywords)35184372088832L;

		// Token: 0x04000015 RID: 21
		public const EventKeywords MeasuresKeyword = (EventKeywords)70368744177664L;

		// Token: 0x04000016 RID: 22
		public const EventKeywords CriticalDataKeyword = (EventKeywords)140737488355328L;

		// Token: 0x04000017 RID: 23
		public const EventTags CostDeferredLatency = (EventTags)262144;

		// Token: 0x04000018 RID: 24
		public const EventTags CoreData = (EventTags)524288;

		// Token: 0x04000019 RID: 25
		public const EventTags InjectXToken = (EventTags)1048576;

		// Token: 0x0400001A RID: 26
		public const EventTags RealtimeLatency = (EventTags)2097152;

		// Token: 0x0400001B RID: 27
		public const EventTags NormalLatency = (EventTags)4194304;

		// Token: 0x0400001C RID: 28
		public const EventTags CriticalPersistence = (EventTags)8388608;

		// Token: 0x0400001D RID: 29
		public const EventTags NormalPersistence = (EventTags)16777216;

		// Token: 0x0400001E RID: 30
		public const EventTags DropPii = (EventTags)33554432;

		// Token: 0x0400001F RID: 31
		public const EventTags HashPii = (EventTags)67108864;

		// Token: 0x04000020 RID: 32
		public const EventTags MarkPii = (EventTags)134217728;

		// Token: 0x04000021 RID: 33
		public const EventFieldTags DropPiiField = (EventFieldTags)67108864;

		// Token: 0x04000022 RID: 34
		public const EventFieldTags HashPiiField = (EventFieldTags)134217728;

		// Token: 0x04000023 RID: 35
		private static readonly string[] telemetryTraits = new string[]
		{
			"ETW_GROUP",
			"{4f50731a-89cf-4782-b3e0-dce8c90476ba}"
		};

		// Token: 0x02000038 RID: 56
		private class EventDescriptionInfo<T>
		{
			// Token: 0x06000266 RID: 614 RVA: 0x0000EC74 File Offset: 0x0000CE74
			private EventDescriptionInfo(Type type)
			{
				Type typeFromHandle = typeof(T);
				EventDescriptionAttribute eventDescriptionAttribute = null;
				object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					object obj = customAttributes[num];
					eventDescriptionAttribute = (obj as EventDescriptionAttribute);
				}
				bool flag = eventDescriptionAttribute == null;
				if (flag)
				{
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "WriteTelemetry requires the data type {0} to have an {1} attribute", new object[]
					{
						typeof(T).Name,
						typeof(EventDescriptionAttribute).Name
					}));
				}
				this.Options = new EventSourceOptions
				{
					Keywords = eventDescriptionAttribute.Keywords,
					Level = eventDescriptionAttribute.Level,
					Opcode = eventDescriptionAttribute.Opcode,
					Tags = eventDescriptionAttribute.Tags,
					ActivityOptions = eventDescriptionAttribute.ActivityOptions
				};
			}

			// Token: 0x06000267 RID: 615 RVA: 0x0000ED64 File Offset: 0x0000CF64
			public static TelemetryEventSource.EventDescriptionInfo<T> GetInstance()
			{
				bool flag = TelemetryEventSource.EventDescriptionInfo<T>.instance == null;
				if (flag)
				{
					TelemetryEventSource.EventDescriptionInfo<T> value = new TelemetryEventSource.EventDescriptionInfo<T>(typeof(T));
					Interlocked.CompareExchange<TelemetryEventSource.EventDescriptionInfo<T>>(ref TelemetryEventSource.EventDescriptionInfo<T>.instance, value, null);
				}
				return TelemetryEventSource.EventDescriptionInfo<T>.instance;
			}

			// Token: 0x04000104 RID: 260
			private static TelemetryEventSource.EventDescriptionInfo<T> instance;

			// Token: 0x04000105 RID: 261
			public EventSourceOptions Options;
		}
	}
}
