using System;
using System.Collections.Generic;
using UnityEditor;
using ToolkitEngine.Sensors;

namespace ToolkitEditor.Sensors.VisualScripting
{
	[InitializeOnLoad]
	public static class Setup
	{
		static Setup()
		{
			var types = new List<Type>()
			{
				typeof(BaseSensor),
				typeof(FilterSensor),
				typeof(BaseDetectionSensor),
				typeof(BasePulseableSensor),
				typeof(RangeSensor),
				typeof(RaySensor),
				typeof(BaseColliderSensor),
				typeof(ColliderSensor),
				typeof(TriggerSensor),
				typeof(SignalSensor),
				typeof(SignalType),
				typeof(SignalBroadcaster),
				typeof(Signal),
				typeof(MarkupSensor),
				typeof(MarkupType),
				typeof(Markup),
				typeof(Perception),
				typeof(SensorEventArgs),
				typeof(MarkupEventArgs),
				typeof(PerceptionEventArgs),
				typeof(ISignalDetectable),
			};

			ToolkitEditor.VisualScripting.Setup.Initialize("ToolkitEngine.Sensors", types);
		}
	}
}