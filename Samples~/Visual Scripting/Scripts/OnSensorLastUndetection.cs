using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Last Undetection"), UnitSurtitle("Sensor")]
    public class OnSensorLastUndetection : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorLastUndetectionMessageListener);
    }
}