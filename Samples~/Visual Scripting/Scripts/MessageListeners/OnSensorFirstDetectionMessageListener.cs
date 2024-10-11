using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
    [AddComponentMenu("")]
    public class OnSensorFirstDetectionMessageListener : MessageListener
    {
        private void Start() => GetComponent<ISignalDetectable>()?.onFirstDetection.AddListener((value) =>
        {
            EventBus.Trigger(EventHooks.OnSensorFirstDetection, gameObject, value);
        });
    }
}