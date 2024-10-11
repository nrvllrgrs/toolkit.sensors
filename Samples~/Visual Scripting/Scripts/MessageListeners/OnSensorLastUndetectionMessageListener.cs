using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSensorLastUndetectionMessageListener : MessageListener
    {
        private void Start() => GetComponent<ISignalDetectable>()?.onLastUndetection.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSensorLastUndetection, gameObject, value);
        });
    }
}