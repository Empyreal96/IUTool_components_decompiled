using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Diagnostics.Tracing;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x0200000A RID: 10
	internal class TelemetryEventSource : EventSource
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00002FD9 File Offset: 0x000011D9
		public TelemetryEventSource(string eventSourceName) : base(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002FE8 File Offset: 0x000011E8
		protected TelemetryEventSource() : base(EventSourceSettings.EtwSelfDescribingEventFormat, TelemetryEventSource.telemetryTraits)
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002FF8 File Offset: 0x000011F8
		public static EventSourceOptions TelemetryOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)35184372088832L
			};
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003020 File Offset: 0x00001220
		public static EventSourceOptions MeasuresOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)70368744177664L
			};
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003048 File Offset: 0x00001248
		public static EventSourceOptions CriticalDataOptions()
		{
			return new EventSourceOptions
			{
				Keywords = (EventKeywords)140737488355328L
			};
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000306E File Offset: 0x0000126E
		[NonEvent]
		public void WriteTelemetry<T>(T data)
		{
			if (base.IsEnabled())
			{
				base.Write<T>(null, ref TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options, ref data);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000308B File Offset: 0x0000128B
		[NonEvent]
		public void WriteTelemetry<T>(ref Guid activityId, ref Guid relatedActivityId, ref T data)
		{
			if (base.IsEnabled())
			{
				base.Write<T>(null, ref TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options, ref activityId, ref relatedActivityId, ref data);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000030A9 File Offset: 0x000012A9
		public static EventSourceOptions GetEventSourceOptionsForType<T>()
		{
			return TelemetryEventSource.EventDescriptionInfo<T>.GetInstance().Options;
		}

		// Token: 0x04000024 RID: 36
		public const EventKeywords Reserved44Keyword = (EventKeywords)17592186044416L;

		// Token: 0x04000025 RID: 37
		public const EventKeywords TelemetryKeyword = (EventKeywords)35184372088832L;

		// Token: 0x04000026 RID: 38
		public const EventKeywords MeasuresKeyword = (EventKeywords)70368744177664L;

		// Token: 0x04000027 RID: 39
		public const EventKeywords CriticalDataKeyword = (EventKeywords)140737488355328L;

		// Token: 0x04000028 RID: 40
		public const EventTags CostDeferredLatency = (EventTags)262144;

		// Token: 0x04000029 RID: 41
		public const EventTags CoreData = (EventTags)524288;

		// Token: 0x0400002A RID: 42
		public const EventTags InjectXToken = (EventTags)1048576;

		// Token: 0x0400002B RID: 43
		public const EventTags RealtimeLatency = (EventTags)2097152;

		// Token: 0x0400002C RID: 44
		public const EventTags NormalLatency = (EventTags)4194304;

		// Token: 0x0400002D RID: 45
		public const EventTags CriticalPersistence = (EventTags)8388608;

		// Token: 0x0400002E RID: 46
		public const EventTags NormalPersistence = (EventTags)16777216;

		// Token: 0x0400002F RID: 47
		public const EventTags DropPii = (EventTags)33554432;

		// Token: 0x04000030 RID: 48
		public const EventTags HashPii = (EventTags)67108864;

		// Token: 0x04000031 RID: 49
		public const EventTags MarkPii = (EventTags)134217728;

		// Token: 0x04000032 RID: 50
		public const EventFieldTags DropPiiField = (EventFieldTags)67108864;

		// Token: 0x04000033 RID: 51
		public const EventFieldTags HashPiiField = (EventFieldTags)134217728;

		// Token: 0x04000034 RID: 52
		private static readonly string[] telemetryTraits = new string[]
		{
			"ETW_GROUP",
			"{4f50731a-89cf-4782-b3e0-dce8c90476ba}"
		};

		// Token: 0x0200004E RID: 78
		private class EventDescriptionInfo<T>
		{
			// Token: 0x0600025B RID: 603 RVA: 0x0000A744 File Offset: 0x00008944
			private EventDescriptionInfo(Type type)
			{
				MemberInfo typeFromHandle = typeof(T);
				EventDescriptionAttribute eventDescriptionAttribute = null;
				object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				int num = 0;
				if (num < customAttributes.Length)
				{
					eventDescriptionAttribute = (customAttributes[num] as EventDescriptionAttribute);
				}
				if (eventDescriptionAttribute == null)
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

			// Token: 0x0600025C RID: 604 RVA: 0x0000A81C File Offset: 0x00008A1C
			public static TelemetryEventSource.EventDescriptionInfo<T> GetInstance()
			{
				if (TelemetryEventSource.EventDescriptionInfo<T>.instance == null)
				{
					TelemetryEventSource.EventDescriptionInfo<T> value = new TelemetryEventSource.EventDescriptionInfo<T>(typeof(T));
					Interlocked.CompareExchange<TelemetryEventSource.EventDescriptionInfo<T>>(ref TelemetryEventSource.EventDescriptionInfo<T>.instance, value, null);
				}
				return TelemetryEventSource.EventDescriptionInfo<T>.instance;
			}

			// Token: 0x0400017D RID: 381
			private static TelemetryEventSource.EventDescriptionInfo<T> instance;

			// Token: 0x0400017E RID: 382
			public EventSourceOptions Options;
		}
	}
}
