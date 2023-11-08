using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[UnitTitle("On Disturbance")]
	public class OnPerceptionDisturbance : BasePerceptionEventUnit
	{
		public override Type MessageListenerType => typeof(OnPerceptionDisturbanceMessageListener);
	}
}