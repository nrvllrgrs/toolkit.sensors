using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPerceptionDisturbanceMessageListener : MessageListener
	{
		private void Start() => GetComponent<Perception>()?.onStimulusSensed.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPerceptionDisturbance, gameObject, value);
		});
	}
}
