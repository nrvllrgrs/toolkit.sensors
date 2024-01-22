using System.Collections.Generic;
using UnityEngine;

namespace ToolkitEngine.Sensors
{
	public class MarkupFilter : BaseFilter
	{
		#region Fields

		[SerializeField]
		private List<MarkupType> m_includedMarkup = new();

		[SerializeField]
		private List<MarkupType> m_excludedMarkup = new();

		#endregion

		#region Methods

		protected override bool IsIncluded(GameObject actor, GameObject target, Vector3 position)
		{
			if (!target.TryGetComponent(out Markup markup))
				return false;

			if (m_excludedMarkup.Contains(markup.type))
				return false;

			if (m_includedMarkup.Count > 0)
				return m_includedMarkup.Contains(markup.type);

			return true;
		}

		#endregion
	}
}