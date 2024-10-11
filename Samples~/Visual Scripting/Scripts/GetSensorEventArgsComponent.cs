using System;
using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[UnitCategory("Sensors")]
    public class GetSensorEventArgsComponent : Unit
    {
		#region Enumerators

		public enum Parameter
		{
			Detected,
			Sensor,
		}

		#endregion

		#region Fields

		[DoNotSerialize, PortLabelHidden]
		public ControlInput inputTrigger { get; private set; }

		[UnitHeaderInspectable]
		public Parameter parameter;

		[DoNotSerialize]
		public ValueInput sensorEventArgs;

		[DoNotSerialize]
		public ValueInput type;

		[DoNotSerialize]
		public ControlOutput validTrigger { get; private set; }

		[DoNotSerialize]
		public ControlOutput invalidTrigger { get; private set; }

		[DoNotSerialize, PortLabelHidden]
		public ValueOutput component;

		private Component m_component;

		#endregion

		#region Methods

		protected override void Definition()
		{
			inputTrigger = ControlInput(nameof(inputTrigger), Trigger);

			sensorEventArgs = ValueInput<SensorEventArgs>(nameof(sensorEventArgs));
			type = ValueInput(nameof(type), default(Type));

			component = ValueOutput(nameof(component), (x) => m_component);

			validTrigger = ControlOutput("Not Null");
			invalidTrigger = ControlOutput("Null");

			Requirement(sensorEventArgs, inputTrigger);
			Requirement(type, inputTrigger);

			Succession(inputTrigger, validTrigger);
			Succession(inputTrigger, invalidTrigger);
		}

		private ControlOutput Trigger(Flow flow)
		{
			var _sensorEventArgs = flow.GetValue<SensorEventArgs>(sensorEventArgs);
			if (_sensorEventArgs != null)
			{
				Transform transform = null;
				switch (parameter)
				{
					case Parameter.Detected:
						transform = _sensorEventArgs.signal.detected.transform;
						break;

					case Parameter.Sensor:
						transform = _sensorEventArgs.sensor.transform;
						break;
				}

				if (transform != null)
				{
					Type _type = flow.GetValue<Type>(type);

					m_component = GetComponent(flow, transform, _type);
					if (m_component != null)
					{
						return validTrigger;
					}
				}
			}

			m_component = null;
			return invalidTrigger;
		}

		protected virtual Component GetComponent(Flow flow, Transform transform, Type type)
		{
			return transform.GetComponent(type);
		}

		#endregion
	}
}