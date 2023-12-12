using UnityEngine;
using static ToolkitEngine.Sensors.Perception;

namespace ToolkitEngine.Sensors
{
	public class StimulusStrengthEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField]
		private string m_senseName;

		#endregion

		#region Properties

#if UNITY_EDITOR
		public override bool showCurve => false;
#endif

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
		{
			Stimulus stimulus;
			if (actor.TryGetComponent(out Perception perception)
				&& ((!string.IsNullOrEmpty(m_senseName) && perception.TryGetStimulus(m_senseName, target, out stimulus))
				|| perception.TryGetStimulus(target, out stimulus)))
			{
				return stimulus.signal.strength;
			}

			return 0f;
		}

		#endregion
	}
}
