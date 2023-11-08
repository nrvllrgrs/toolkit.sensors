using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPerceptionKnownMessageListener : MessageListener
	{
		private void Start() => GetComponent<Perception>()?.onStimulusKnown.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPerceptionKnown, gameObject, value);
		});
	}
}
