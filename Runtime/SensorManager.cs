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

        public Markup[] Get(GameObject obj, float radius, float height, Func<Markup, bool> condition = null)
        {
            return Get(obj, radius, height, condition, null);
        }
        public Markup[] Get(GameObject obj, float radius, float height, Func<Markup, bool> condition, params MarkupType[] args)
        {
            if (obj == null)
                return null;

            return Get(obj.transform.position, radius, height, condition, args);
        }

		public Markup[] Get(Vector3 point, float radius, float height, Func<Markup, bool> condition = null)
        {
            return Get(point, radius, height, condition, null);
        }

		public Markup[] Get(Vector3 point, float radius, float height, Func<Markup, bool> condition, params MarkupType[] args)
		{
			var list = new List<Markup>();
			var sqrDistance = radius * radius;

            float lowerSensor = 0f, upperSensor = 0f;
            if (height > 0f)
            {
                GetVerticalRange(point.y, height, out lowerSensor, out upperSensor);
            }

			foreach (var markup in m_markups)
			{
                // Markup type does not exist as valid in sensor, skip
                if (args != null && args.Length > 0 && !args.Contains(markup.type))
                    continue;

				// Sensor is spherical...
				if (height == 0f)
                {
                    // Markup is spherical (or a point)...
                    if (markup.height == 0f)
                    {
                        float testSqrDistance = markup.radius == 0
                            ? sqrDistance
                            : Mathf.Pow(radius + markup.radius, 2f);

                        if ((point - markup.transform.position).sqrMagnitude < testSqrDistance
                            && (condition?.Invoke(markup) ?? true))
                        {
                            list.Add(markup);
                        }
                    }
					// Markup is cylindrical (or a line)...
					else
					{
						GetVerticalRange(markup.transform.position.y, markup.height, out float lowerMarkup, out float upperMarkup);
						if (!IntersectSphereAndCylinder(point, radius, markup.transform.position, markup.radius, lowerMarkup, upperMarkup))
							continue;

						if (condition?.Invoke(markup) ?? true)
						{
							list.Add(markup);
						}
					}
				}
				// Sensor is cylindrical..
				else
                {
					// Markup is cylindrical (or a point or line)...
					if (markup.height > 0f || markup.radius == 0f)
					{
                        // Outside of vertical range, skip
                        GetVerticalRange(markup.transform.position.y, markup.height, out float lowerMarkup, out float upperMarkup);
                        if (!VerticalRangeOverlaps(lowerSensor, upperSensor, lowerMarkup, upperMarkup))
                            continue;

						// Circles on X-Z plane do not intersect, skip
						var point2D = new Vector2(point.x, point.z);
						var markupPosition2D = new Vector2(markup.transform.position.x, markup.transform.position.z);
						if (!IntersectCircles(point2D, radius, markupPosition2D, markup.radius))
                            continue;

                        if (condition?.Invoke(markup) ?? true)
                        {
                            list.Add(markup);
                        }
                    }
                    // Markup is spherical...
                    else
                    {
                        if (!IntersectSphereAndCylinder(markup.transform.position, markup.radius, point, radius, lowerSensor, upperSensor))
                            continue;

						if (condition?.Invoke(markup) ?? true)
						{
							list.Add(markup);
						}
					}
				}
			}
			return list.ToArray();
		}

		private bool IntersectCircles(Vector2 a, float r, Vector2 b, float q)
		{
			return (a - b).sqrMagnitude < Mathf.Pow(r + q, 2f);
		}

		private bool IntersectSphereAndCylinder(Vector3 spherePosition, float sphereRadius, Vector3 cylinderPosition, float cylinderRadius, float cylinderLower, float cylinderUpper)
        {
            float capRadius = 0f;

            // Cylinder intersects through center of sphere
            if (cylinderLower <= spherePosition.y && spherePosition.y <= cylinderUpper)
            {
                capRadius = sphereRadius;
            }
            // Cylinder only intersects through bottom of sphere
            else if (cylinderLower <= spherePosition.y - sphereRadius && spherePosition.y - sphereRadius <= cylinderUpper)
            {
                // Calculate distance between top of cylinder to center
                float h = sphereRadius - Math.Abs(spherePosition.y - cylinderUpper);
                capRadius = Mathf.Sqrt(h * (2f * sphereRadius - h));
            }
            // Cylinder only intersect through top of sphere
            else if (cylinderLower <= spherePosition.y + sphereRadius && spherePosition.y + sphereRadius <= cylinderUpper)
            {
				// Calculate distance between bottom of cylinder to center
				float h = sphereRadius - Math.Abs(spherePosition.y - cylinderLower);
				capRadius = Mathf.Sqrt(h * (2f * sphereRadius - h));
			}

            if (capRadius == 0f)
                return false;

            var spherePosition2D = new Vector2(spherePosition.x, spherePosition.z);
            var cylinderPosition2D = new Vector2(cylinderPosition.x, cylinderPosition.z);
            return IntersectCircles(spherePosition2D, capRadius, cylinderPosition2D, cylinderRadius);
        }

        private bool VerticalRangeOverlaps(float lowerSensor, float upperSensor, float lowerMarkup, float upperMarkup)
        {
            return Mathf.Max(lowerSensor, lowerMarkup) <= Mathf.Min(upperSensor, upperMarkup);
		}

        private void GetVerticalRange(float y, float height, out float min, out float max)
        {
            min = y - height * 0.5f;
            max = y + height * 0.5f;
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