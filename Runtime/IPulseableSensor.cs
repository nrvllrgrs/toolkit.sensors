using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
    public interface IPulseableSensor
    {
        public UnityEvent<SensorEventArgs> onPulsed { get; }
        public PulseMode pulseMode { get; }
        public void Pulse();
    }
}