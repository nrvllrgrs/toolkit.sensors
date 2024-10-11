using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Pulsed"), UnitSurtitle("IPulseableSensor")]
    public class OnSensorPulsed : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorPulsedMessageListener);
    }
}