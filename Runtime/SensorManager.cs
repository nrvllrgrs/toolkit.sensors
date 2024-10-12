using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
    [AddComponentMenu("Sensor/Sensor Manager")]
    public class SensorManager : Subsystem<SensorManager>
    {
        #region Fields

        private HashSet<Markup> m_markups;
        private HashSet<SignalBroadcaster> m_broadcasters;
        private HashSet<SignalSensor> m_signalSensors;

        private HashSet<IPulseableSensor> m_pulseableSensors;

		#endregion

		#region Methods

		protected override void Initialize()
		{
			base.Initialize();

			m_markups = new();
            m_broadcasters = new();
            m_signalSensors = new();
            m_pulseableSensors = new();

		    LifecycleSubsystem.Register(this, LifecycleSubsystem.Phase.Update);
			LifecycleSubsystem.Register(this, LifecycleSubsystem.Phase.FixedUpdate);
			LifecycleSubsystem.Register(this, LifecycleSubsystem.Phase.LateUpdate);
		}

		protected override void Terminate()
		{
			base.Terminate();

			LifecycleSubsystem.Unregister(this, LifecycleSubsystem.Phase.Update);
			LifecycleSubsystem.Unregister(this, LifecycleSubsystem.Phase.FixedUpdate);
			LifecycleSubsystem.Unregister(this, LifecycleSubsystem.Phase.LateUpdate);
		}

		public void Register(Markup markup)
        {
            if (markup == null)
                return;

            m_markups.Add(markup);
        }

        public void Unregister(Markup markup)
        {
            if (markup == null)
                return;

            m_markups.Remove(markup);
        }

        public void Register(SignalBroadcaster broadcaster)
        {
            m_broadcasters.Add(broadcaster);
        }

        public void Unregister(SignalBroadcaster broadcaster)
        {
            m_broadcasters.Remove(broadcaster);
        }

        public void Register(SignalSensor sensor)
        {
            m_signalSensors.Add(sensor);
        }

        public void Unregister(SignalSensor sensor)
        {
            m_signalSensors.Remove(sensor);
        }

        public void Register(IPulseableSensor sensor)
        {
            m_pulseableSensors.Add(sensor);
        }

        public void Unregister(IPulseableSensor sensor)
        {
            m_pulseableSensors.Remove(sensor);
        }

        public override void Update()
        {
            BroadcastSignals(IsUpdateBroadcaster);
            PulseSensors(IsUpdateSensor);
        }

		public override void FixedUpdate()
        {
            BroadcastSignals(IsFixedUpdateBroadcaster);
            PulseSensors(IsFixedUpdateSensor);
        }

        public override void LateUpdate()
        {
            if (m_signalSensors.Count > 0)
            {
                // Create new list as to parse because may be removing sensors
                var signalSensors = new HashSet<SignalSensor>(m_signalSensors);
                foreach (var sensor in signalSensors)
                {
                    sensor.Tick();
                }
            }
        }

        #endregion

        #region Markup Methods

        public Markup[] Get(GameObject obj, float distance, Func<Markup, bool> condition = null)
        {
            return Get(obj, distance, condition, null);
        }
        public Markup[] Get(GameObject obj, float distance, Func<Markup, bool> condition, params MarkupType[] args)
        {
            if (obj == null)
                return null;

            return Get(obj.transform.position, distance, condition, args);
        }

		public Markup[] Get(Vector3 point, float distance, Func<Markup, bool> condition = null)
        {
            return Get(point, distance, condition, null);
        }

		public Markup[] Get(Vector3 point, float distance, Func<Markup, bool> condition, params MarkupType[] args)
		{
			var list = new List<Markup>();
			var sqrDistance = distance * distance;

			foreach (var markup in m_markups)
			{
                if (args != null && args.Length > 0 && !args.Contains(markup.type))
                    continue;

                float testSqrDistance = markup.radius == 0
                    ? sqrDistance
                    : Mathf.Pow(distance + markup.radius, 2f);
                
				if ((point - markup.transform.position).sqrMagnitude < testSqrDistance
					&& (condition?.Invoke(markup) ?? true))
				{
					list.Add(markup);
				}

				if ((args == null || args.Length == 0 || args.Contains(markup.type))
					&& (point - markup.transform.position).sqrMagnitude < sqrDistance
					&& (condition?.Invoke(markup) ?? true))
				{
					list.Add(markup);
				}
			}

			return list.ToArray();
		}

		#endregion

		#region Broadcaster Methods

		public bool IsUpdateBroadcaster(SignalBroadcaster broadcaster) => broadcaster.PulseMode == PulseMode.EveryFrame;
        public bool IsFixedUpdateBroadcaster(SignalBroadcaster broadcaster) => broadcaster.PulseMode == PulseMode.FixedInterval;

        public void BroadcastSignals(System.Func<SignalBroadcaster, bool> predicate)
        {
            foreach (var broadcaster in m_broadcasters)
            {
                if (predicate(broadcaster))
                {
                    broadcaster.Broadcast();
                }
            }
        }

        #endregion

        #region Sensor Methods

        public bool IsUpdateSensor(IPulseableSensor sensor) => sensor.pulseMode == PulseMode.EveryFrame;
        public bool IsFixedUpdateSensor(IPulseableSensor sensor) => sensor.pulseMode == PulseMode.FixedInterval;

        private void PulseSensors(System.Func<IPulseableSensor, bool> predicate)
        {
            foreach (var sensor in m_pulseableSensors)
            {
                if (predicate(sensor))
                {
                    sensor.Pulse();
                }
            }
        }

        #endregion
    }
}