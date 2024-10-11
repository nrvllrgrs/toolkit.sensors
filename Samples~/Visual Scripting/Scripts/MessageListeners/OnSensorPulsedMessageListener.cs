using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
    public class OnSensorPulsedMessageListener : MessageListener
    {
        private void Start() => GetComponent<IPulseableSensor>()?.onPulsed.AddListener((value) =>
        {
            EventBus.Trigger(nameof(OnSensorPulsed), gameObject, value);
        });
    }
}