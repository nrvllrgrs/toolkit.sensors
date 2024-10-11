using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On First Detection"), UnitSurtitle("ISignalDetectable")]
    public class OnSensorFirstDetection : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorFirstDetectionMessageListener);
    }
}