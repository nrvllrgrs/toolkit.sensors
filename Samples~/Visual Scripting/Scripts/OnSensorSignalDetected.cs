using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Signal Detected"), UnitSurtitle("ISignalDetectable")]
    public class OnSensorSignalDetected : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorSignalDetectedMessageListener);
    }
}