using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
	public class PerceptionEventArgs : EventArgs
	{
		#region Properties

		public Perception perception { get; private set; }
		public Perception.Sense sense { get; private set; }
		public Perception.Stimulus stimulus { get; private set; }
		public float value { get; private set; }
		public float delta { get; private set; }

		#endregion

		#region Constructors

		public PerceptionEventArgs(Perception perception, Perception.Stimulus stimulus)
			: this(perception, null, stimulus)
		{ }

		public PerceptionEventArgs(Perception perception, Perception.Sense sense, Perception.Stimulus stimulus)
		{
			this.perception = perception;
			this.sense = sense;
			this.stimulus = stimulus;
		}

		public PerceptionEventArgs(Perception.Stimulus stimulus, float value, float delta)
			: this(stimulus.m_sense.m_perception, stimulus.m_sense, stimulus)
		{
			this.value = value;
			this.delta = delta;
		}

		#endregion
	}

	public class Perception : MonoBehaviour
    {
		#region Enumerators

		public enum SenseType
		{
			Custom,
			Hearing,
			Sight,
			Smell,
			Taste,
			Touch,
		}

		#endregion

		#region Fields

		[SerializeField]
		private Sense[] m_senses;

		[SerializeField, Min(0f)]
		private float m_drainDelay;

		[SerializeField, MaxInfinity]
		private float m_drainRate;

		[SerializeField]
		private UnityElector<Stimulus> m_targetElector = new();

		/// <summary>
		/// Lookup table to find sense by name
		/// </summary>
		private Dictionary<string, Sense> m_senseMap = new();

		/// <summary>
		/// Set of sensors detecting stimuli
		/// </summary>
		private Dictionary<Stimulus, HashSet<BaseSensor>> m_perceivedMap = new();

		private Stimulus m_selectedStimulus;
		private int m_stimuliCount = 0;

		#endregion

		#region Events

		[SerializeField, Tooltip("Invoked when first stimulus is detected.")]
		private UnityEvent<PerceptionEventArgs> m_onFirstStimulusSensed;

		[SerializeField, Tooltip("Invoked when stimulus is detected.")]
		private UnityEvent<PerceptionEventArgs> m_onStimulusSensed;

		[SerializeField, Tooltip("Invoked when stimulus is undetected.")]
		private UnityEvent<PerceptionEventArgs> m_onStimulusUnsensed;

		[SerializeField, Tooltip("Invoked when last stimulus is undetected.")]
		private UnityEvent<PerceptionEventArgs> m_onLastStimulusUnsensed;

		[SerializeField, Tooltip("Invoked when stimulus confidence changed.")]
		private UnityEvent<PerceptionEventArgs> m_onStimulusConfidenceChanged;

		[SerializeField, Tooltip("Invoked when stimulus confidence is 1.")]
		private UnityEvent<PerceptionEventArgs> m_onStimulusKnown;

		[SerializeField, Tooltip("Invoked when stimulus confidence is 0.")]
		private UnityEvent<PerceptionEventArgs> m_onStimulusUnknown;

		[SerializeField, Tooltip("Invoked when target changed.")]
		private UnityEvent<PerceptionEventArgs> m_onTargetChanged;

		#endregion

		#region Properties

		public Stimulus selectedStimulus
		{
			get => m_selectedStimulus;
			private set
			{
				// No change, skip
				if (m_selectedStimulus == value)
					return;

				m_selectedStimulus = value;
				m_onTargetChanged?.Invoke(new PerceptionEventArgs(this, value));
			}
		}

		public GameObject target => m_selectedStimulus?.gameObject;
		public float confidence => m_selectedStimulus?.confidence ?? 0f;

		public UnityEvent<PerceptionEventArgs> onFirstStimulusSensed => m_onFirstStimulusSensed;
		public UnityEvent<PerceptionEventArgs> onStimulusSensed => m_onStimulusSensed;
		public UnityEvent<PerceptionEventArgs> onStimulusUnsensed => m_onStimulusUnsensed;
		public UnityEvent<PerceptionEventArgs> onLastStimulusSensed => m_onLastStimulusUnsensed;
		public UnityEvent<PerceptionEventArgs> onStimulusConfidenceChanged => m_onStimulusConfidenceChanged;
		public UnityEvent<PerceptionEventArgs> onStimulusKnown => m_onStimulusKnown;
		public UnityEvent<PerceptionEventArgs> onStimulusUnknown => m_onStimulusUnknown;
		public UnityEvent<PerceptionEventArgs> onTargetChanged => m_onTargetChanged;

		#endregion

		#region Methods

		private void Awake()
		{
			// Order by descending priority to have early exit during update
			m_senses = m_senses.OrderByDescending(x => x.priority).ToArray();

			// Create map to quickly find sense by key
			foreach (var sense in m_senses)
			{
				m_senseMap.Add(sense.name, sense);
				sense.m_perception = this;
			}
		}

		private void OnEnable()
		{
			foreach (var sense in m_senses)
			{
				sense.FirstSensed += Sense_FirstSensed;
				sense.Sensed += Sense_Sensed;
				sense.Unsensed += Sense_Unsensed;
				sense.LastUnsensed += Sense_LastUnsensed;

				foreach (var stimulus in sense.stimuli)
				{
					Register(stimulus);
				}
			}
		}

		private void OnDisable()
		{
			foreach (var sense in m_senses)
			{
				sense.FirstSensed -= Sense_FirstSensed;
				sense.Sensed -= Sense_Sensed;
				sense.Unsensed -= Sense_Unsensed;
				sense.LastUnsensed -= Sense_LastUnsensed;

				foreach (var stimulus in sense.stimuli)
				{
					Unregister(stimulus);
				}
			}

			selectedStimulus = null;
		}

		public void SetEnabled(string senseName, bool enabled)
		{
			if (m_senseMap.TryGetValue(senseName, out Sense sense))
			{
				sense.enabled = enabled;
			}
		}

		public bool TryGetStimulus(GameObject obj, out Stimulus stimulus)
		{
			foreach (var sense in m_senses)
			{
				if (sense.TryGetStimulus(obj, out stimulus))
					return true;
			}

			stimulus = default;
			return false;
		}

		public bool TryGetStimulus(string senseName, GameObject obj, out Stimulus stimulus)
		{
			if (m_senseMap.TryGetValue(senseName, out Sense sense)
				&& sense.TryGetStimulus(obj, out stimulus))
			{
				return true;
			}

			stimulus = default;
			return false;
		}

		private void Update()
		{
			Stimulus stimulus = null;
			float score = float.NegativeInfinity;
			int priority = int.MinValue;

			// Assume senses are ordered by descending priority
			foreach (var sense in m_senses)
			{
				bool skipElect = !sense.enabled
					|| (stimulus != null && sense.priority < priority);

				// Can skip elect, but still needs to be processed to update stimuli and modify confidence
				if (sense.TryProcessStimuli(gameObject, m_targetElector, skipElect, out Stimulus testStimulus, out float testScore)
					&& testScore > score)
				{
					stimulus = testStimulus;
					score = testScore;
					priority = sense.priority;
				}
			}

			selectedStimulus = stimulus;
		}

		public void Remove(Stimulus stimulus)
		{
			// Not being perceived, skip
			if (!m_perceivedMap.ContainsKey(stimulus))
				return;

			//stimulus.StopDrain(true);
			m_perceivedMap.Remove(stimulus);

			// Nothing is being perceived...
			if (m_perceivedMap.Count == 0)
			{
				selectedStimulus = null;
			}
		}

		private void Register(Stimulus stimulus)
		{
			if (stimulus == null)
				return;

			stimulus.ConfidenceChanged += Stimulus_ConfidenceChanged;
			stimulus.Known += Stimulus_Known;
			stimulus.Unknown += Stimulus_Unknown;
		}

		private void Unregister(Stimulus stimulus)
		{
			if (stimulus == null)
				return;

			stimulus.ConfidenceChanged -= Stimulus_ConfidenceChanged;
			stimulus.Known -= Stimulus_Known;
			stimulus.Unknown -= Stimulus_Unknown;
		}

		#endregion

		#region Sense Callbacks

		private void Sense_FirstSensed(object sender, PerceptionEventArgs e)
		{
			if (m_stimuliCount++ == 0)
			{
				m_onFirstStimulusSensed?.Invoke(e);
			}
		}

		private void Sense_Sensed(object sender, PerceptionEventArgs e)
		{
			m_onStimulusSensed?.Invoke(e);
			Register(e.stimulus);
		}

		private void Sense_Unsensed(object sender, PerceptionEventArgs e)
		{
			m_onStimulusUnsensed?.Invoke(e);
			Unregister(e.stimulus);
		}

		private void Sense_LastUnsensed(object sender, PerceptionEventArgs e)
		{
			if (--m_stimuliCount == 0)
			{
				m_onLastStimulusUnsensed?.Invoke(e);
			}
		}

		#endregion

		#region Stimulus Calbacks

		private void Stimulus_ConfidenceChanged(object sender, PerceptionEventArgs e)
		{
			m_onStimulusConfidenceChanged?.Invoke(e);
		}

		private void Stimulus_Known(object sender, PerceptionEventArgs e)
		{
			m_onStimulusKnown?.Invoke(e);
		}

		private void Stimulus_Unknown(object sender, PerceptionEventArgs e)
		{
			m_onStimulusUnknown?.Invoke(e);
		}

		#endregion

		#region Structures

		[Serializable]
		public class Sense
		{
			#region Fields

			[SerializeField]
			private bool m_enabled = true;

			[SerializeField]
			private string m_name;

			[SerializeField]
			private int m_priority;

			[SerializeField]
			private BaseSensor m_sensor;

			[SerializeField, Tooltip("Indicates whether pulseable sensors should automatically be pulsed.")]
			private bool m_autoPulse;

			[SerializeField, Range(0f, 1f), Tooltip("Minimum signal strength to perceive.")]
			private float m_strengthThreshold;

			[SerializeField, Tooltip("Evaluator to calculate how much confidence in stimulus is gained per tick.")]
			private UnityEvaluator m_confidence = new();

			[SerializeField, Tooltip("Indicates whether stimulus confidence is factored when determining score.")]
			private bool m_useConfidence;

			[SerializeField, Tooltip("Indicates whether signal strength is factored when determining score.")]
			private bool m_useStrength;

			internal Perception m_perception;
			private HashSet<Stimulus> m_stimuli = new();

			#endregion

			#region Properties

			public bool enabled { get => m_enabled; set => m_enabled = value; }
			public string name => m_name;
			public BaseSensor sensor => m_sensor;
			public int priority => m_priority;
			public Stimulus[] stimuli => m_stimuli.ToArray();
			public float strengthThreshold => m_strengthThreshold;

			#endregion

			#region Events

			public event EventHandler<PerceptionEventArgs> FirstSensed;
			public event EventHandler<PerceptionEventArgs> Sensed;
			public event EventHandler<PerceptionEventArgs> Unsensed;
			public event EventHandler<PerceptionEventArgs> LastUnsensed;

			#endregion

			#region Methods

			public bool TryGetStimulus(GameObject obj, out Stimulus stimulus)
			{
				stimulus = m_stimuli.FirstOrDefault(x => Equals(x.gameObject, obj));
				return stimulus != null;
			}

			public bool TryProcessStimuli(GameObject actor, UnityElector<Stimulus> calculator, bool skipElect, out Stimulus stimulus, out float score)
			{
				if (m_autoPulse && sensor is IPulseableSensor pulseableSensor)
				{
					pulseableSensor.Pulse();
				}

				// Remember stimuli during this evaluation
				HashSet<Stimulus> sensedStimuli = new();

				if (sensor.signals.Length > 0)
				{
					foreach (var signal in sensor.signals)
					{
						if (signal.strength < m_strengthThreshold)
							continue;

						// Need to create a new stimulus so it can be found in set
						var s = new Stimulus(this, signal);
						var args = new PerceptionEventArgs(m_perception, this, s);

						if (!m_stimuli.TryGetValue(s, out Stimulus storedStimulus))
						{
							bool wasEmpty = !m_stimuli.Any();

							// Add new stimulus
							m_stimuli.Add(s);
							storedStimulus = s;

							if (wasEmpty)
							{
								FirstSensed?.Invoke(this, args);
							}
							Sensed?.Invoke(this, args);
						}

						// Initialize (or reset) delay when it needs to be forgotten
						storedStimulus.remainingDelay = m_perception.m_drainDelay;

						// Remember all stimli processed so untracked stimuli can be forgotten
						sensedStimuli.Add(storedStimulus);

						if (storedStimulus.confidence < 1)
						{
							// Set confidence in signal
							// Not increase my some increment
							storedStimulus.confidence = m_confidence.Evaluate(actor, storedStimulus.gameObject);
						}
					}
				}

				// Reduce confidence in all lost signals
				foreach (var s in m_stimuli.Except(sensedStimuli).ToArray())
				{
					if (s.remainingDelay > 0)
					{
						s.remainingDelay -= Time.deltaTime;
					}
					else
					{
						s.confidence -= m_perception.m_drainRate * Time.deltaTime;
					}

					if (s.confidence <= 0f)
					{
						m_stimuli.Remove(s);

						var args = new PerceptionEventArgs(m_perception, this, s);
						Unsensed?.Invoke(this, args);

						if (!m_stimuli.Any())
						{
							LastUnsensed?.Invoke(this, args);
						}
					}
				}

				if (!skipElect && calculator.TryElect(m_stimuli, actor, GetTarget, GetWeight, out stimulus, out score))
					return true;

				stimulus = default;
				score = default;
				return false;
			}

			private GameObject GetTarget(Stimulus stimulus) => stimulus.gameObject;

			private float GetWeight(Stimulus stimulus)
			{
				float weight = 1f;
				int count = 0;

				if (m_useConfidence)
				{
					weight *= stimulus.confidence;
					++count;
				}

				if (m_useStrength)
				{
					weight *= stimulus.signal.strength;
					++count;
				}

				// Apply compensation factor
				// While not perfect because it is not included with the evaluables count, good enough
				return UnityEvaluator.GetCompensatedScore(weight, count);
			}

			#endregion
		}

		[Serializable]
		public class Stimulus : IEquatable<Stimulus>
		{
			#region Fields

			[SerializeField]
			private Signal m_signal;

			[SerializeField]
			private float m_confidence;

			private float m_ageTimestamp;

			internal Sense m_sense;

			#endregion

			#region Events

			public event EventHandler<PerceptionEventArgs> ConfidenceChanged;
			public event EventHandler<PerceptionEventArgs> Known;
			public event EventHandler<PerceptionEventArgs> Unknown;

			#endregion

			#region Properties

			public GameObject gameObject => m_signal?.detected;
			public Signal signal => m_signal;

			public float confidence
			{
				get => m_confidence;
				set
				{
					value = Mathf.Clamp01(value);

					// No change, skip
					if (m_confidence == value)
						return;

					//// Gaining confidence stops drain, if draining
					//if (m_confidence > value)
					//{
					//	StopDrain();
					//}
					//// Losing confidence starts drain, if not draining
					//else
					//{
					//	StartDrain();
					//}

					float delta = value - m_confidence;
					m_confidence = value;

					var args = new PerceptionEventArgs(this, value, delta);
					ConfidenceChanged?.Invoke(this, args);

					if (Mathf.Approximately(value, 1f))
					{
						Known?.Invoke(this, args);
					}
					else if (Mathf.Approximately(value, 0f))
					{
						Unknown?.Invoke(this, args);
					}
				}
			}

			public float age => Time.time - m_ageTimestamp;
			internal float remainingDelay { get; set; }

			#endregion

			#region Constructors

			public Stimulus(Sense sense, Signal signal)
			{
				m_sense = sense;
				m_signal = signal;
				m_ageTimestamp = Time.time;
			}

			public Stimulus(Sense sense, GameObject obj)
				: this(sense, new Signal(obj))
			{ }

			#endregion

			#region Methods

			//public void StartDrain()
			//{
			//	if (m_drainThread != null)
			//		return;

			//	m_drainThread = CoroutineUtil.GlobalStartCoroutine(AsyncDrain());
			//}

			//public void StopDrain(bool resetConfidence = false)
			//{
			//	if (m_drainThread == null)
			//		return;

			//	CoroutineUtil.GlobalStopCoroutine(m_drainThread);
			//	m_drainThread = null;

			//	if (resetConfidence)
			//	{
			//		confidence = 0f;
			//	}
			//}

			//private IEnumerator AsyncDrain()
			//{
			//	if (m_perception.m_drainDelay > 0)
			//	{
			//		yield return new WaitForSeconds(m_perception.m_drainDelay);
			//	}

			//	while (m_confidence > 0f)
			//	{
			//		confidence -= m_perception.m_drainRate * Time.deltaTime;
			//		yield return null;
			//	}

			//	EndDrain();
			//}

			//private void EndDrain()
			//{
			//	m_perception.Remove(this);

			//	// Reset for future perceptions
			//	m_perception = null;
			//	m_drainThread = null;
			//}

			#endregion

			#region IEquatable Methods

			public bool Equals(Stimulus other)
			{
				if (ReferenceEquals(null, other))
					return false;

				if (ReferenceEquals(this, other))
					return true;

				return Equals(GetHashCode(), other.GetHashCode());
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;

				if (ReferenceEquals(this, obj))
					return true;

				if (obj.GetType() != GetType())
					return false;

				return Equals((Stimulus)obj);
			}

			public override int GetHashCode()
			{
				return signal.GetHashCode();
			}

			#endregion
		}

		#endregion
	}
}