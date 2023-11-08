using UnityEngine;
using Unity.VisualScripting;
using EventHooks = ToolkitEngine.Sensors.EventHooks;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSensorSignalDetectedMessageListener : MessageListener
    {
        private void Start() => GetComponent<BaseSensor>()?.onSignalDetected.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSensorSignalDetected, gameObject, value);
        });
    }
}