using UnityEngine;

namespace ToolkitEngine.Sensors
{
	public class VacancyFilter : BaseFilter
	{
		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Markup markup))
				return false;

			return markup.vacant;
		}

		#endregion
	}
}