using UnityEngine;
using Unity.VisualScripting;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[AddComponentMenu("")]
	public class OnPerceptionStimulusConfidenceChangedMessageListener : MessageListener
	{
		private void Start() => GetComponent<Perception>()?.onStimulusConfidenceChanged.AddListener((value) =>
		{
			EventBus.Trigger(EventHooks.OnPerceptionStimulusConfidenceChanged, gameObject, value);
		});
	}
}