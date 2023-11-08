using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
	public class ReserverFilter : BaseFilter
	{
		#region Fields

		[SerializeField]
		private List<GameObject> m_validReservers = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Markup markup))
				return false;

			if (m_validReservers.Count == 0)
				return markup.reserved;

			if (!markup.reserved)
				return false;

			return m_validReservers.Contains(markup.reserver);
		}

		#endregion
	}
}