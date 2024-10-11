using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
	public interface ISignalDetectable
    {
		UnityEvent<SensorEventArgs> onFirstDetection { get; }
		UnityEvent<SensorEventArgs> onSignalDetected { get; }
		UnityEvent<SensorEventArgs> onSignalUndetected { get; }
		UnityEvent<SensorEventArgs> onLastUndetection { get; }
	}
}