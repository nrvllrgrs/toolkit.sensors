using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPerceptionUnknownMessageListener : MessageListener
	{
		private void Start() => GetComponent<Perception>()?.onStimulusUnknown.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPerceptionUnknown, gameObject, value);
		});
	}
}
