using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[UnitTitle("On Known")]
	public class OnPerceptionKnown : BasePerceptionEventUnit
	{
		public override Type MessageListenerType => typeof(OnPerceptionKnownMessageListener);
	}
}