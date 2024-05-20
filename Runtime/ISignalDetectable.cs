using UnityEngine.Events;

namespace ToolkitEngine.Sensors
{
	public interface ISignalDetectable
    {
		UnityEvent<SensorEventArgs> onSignalDetected { get; }
		UnityEvent<SensorEventArgs> onSignalUndetected { get; }
	}
}