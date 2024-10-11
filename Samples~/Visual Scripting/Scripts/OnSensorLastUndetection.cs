using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Last Undetection"), UnitSurtitle("ISignalDetectable")]
    public class OnSensorLastUndetection : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorLastUndetectionMessageListener);
    }
}