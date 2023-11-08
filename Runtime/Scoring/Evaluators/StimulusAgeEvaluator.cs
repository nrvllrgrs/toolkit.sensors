using UnityEngine;
using static ToolkitEngine.Sensors.Perception;

namespace ToolkitEngine.Sensors
{
    public class StimulusAgeEvaluator : BaseEvaluator
    {
		#region Fields

		[SerializeField]
		private string m_senseName;

		[SerializeField, MinMax(0f, float.PositiveInfinity, "Min", "Max")]
        private Vector2 m_age = new Vector2(0f, 60f);

		#endregion

		#region Properties

        public float min => m_age.x;
        public float max => m_age.y;

		#endregion

		#region Methods

		protected override float CalculateNormalizedScore(GameObject actor, GameObject target, Vector3 position)
        {
			Stimulus stimulus;
			if (actor.TryGetComponent(out Perception perception)
				&& ((!string.IsNullOrEmpty(m_senseName) && perception.TryGetStimulus(m_senseName, target, out stimulus))
				|| perception.TryGetStimulus(target, out stimulus)))
			{
				return MathUtil.GetPercent(stimulus.age, min, max);
			}

			return 0f;
		}

		#endregion
	}
}