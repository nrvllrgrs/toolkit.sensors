using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ToolkitEngine.Sensors.VisualScripting
{
	[UnitTitle("On Confidence Changed"), UnitSurtitle("Stimulus")]
	public class OnPerceptionStimulusConfidenceChanged : BasePerceptionEventUnit
	{
		#region Fields

		[DoNotSerialize]
		public ValueInput equals { get; private set; }

		#endregion

		#region Properties

		public override Type MessageListenerType => typeof(OnPerceptionStimulusConfidenceChangedMessageListener);

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();
			equals = ValueInput<GameObject>(nameof(equals), null);
		}

		protected override bool ShouldTrigger(Flow flow, PerceptionEventArgs args)
		{
			var compareTo = flow.GetValue(this.equals, typeof(GameObject));
			if (compareTo == null)
				return base.ShouldTrigger(flow, args);

			return Equals(compareTo, args.stimulus.gameObject);
		}

		#endregion
	}
}