using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
    public class OnSensorSignalDetectedMessageListener : MessageListener
    {
        private void Start() => GetComponent<ISignalDetectable>()?.onSignalDetected.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSensorSignalDetected, gameObject, value);
        });
    }
}