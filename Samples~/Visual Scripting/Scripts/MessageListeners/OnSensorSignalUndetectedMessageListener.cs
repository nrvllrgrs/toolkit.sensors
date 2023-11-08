using UnityEngine;
using Unity.VisualScripting;
using EventHooks = ToolkitEngine.Sensors.EventHooks;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSensorSignalUndetectedMessageListener : MessageListener
    {
        private void Start() => GetComponent<BaseSensor>()?.onSignalUndetected.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSensorSignalUndetected, gameObject, value);
        });
    }
}