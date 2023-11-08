using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
	public class OccupantFilter : BaseFilter
    {
		#region Fields

		[SerializeField]
		private List<GameObject> m_validOccupants = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Markup markup))
				return false;

			if (m_validOccupants.Count == 0)
				return !markup.vacant;

			if (markup.vacant)
				return false;

			return m_validOccupants.Contains(markup.occupant);
		}

		#endregion
	}
}