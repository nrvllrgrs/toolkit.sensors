using System;
using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.Sensors.VisualScripting;

namespace ToolkitEngine.XR.VisualScripting
{
	[UnitCategory("Sensors")]
	public class GetSensorArgsComponentInParent : GetSensorEventArgsComponent
	{
		#region Methods

		protected override Component GetComponent(Flow flow, Transform transform, Type type)
		{
			return transform.GetComponentInParent(type);
		}

		#endregion
	}
}