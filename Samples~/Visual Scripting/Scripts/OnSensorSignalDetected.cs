using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Signal Detected"), UnitSurtitle("Sensor")]
    public class OnSensorSignalDetected : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorSignalDetectedMessageListener);
    }
}