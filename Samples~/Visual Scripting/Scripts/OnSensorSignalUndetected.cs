using System;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [UnitTitle("On Signal Undetected"), UnitSurtitle("ISignalDetectable")]
    public class OnSensorSignalUndetected : BaseSensorEventUnit
    {
        public override Type MessageListenerType => typeof(OnSensorSignalUndetectedMessageListener);
    }
}