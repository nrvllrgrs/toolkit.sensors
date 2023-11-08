using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[UnitTitle("On Unknown")]
	public class OnPerceptionUnknown : BasePerceptionEventUnit
	{
		public override Type MessageListenerType => typeof(OnPerceptionUnknownMessageListener);
	}
}