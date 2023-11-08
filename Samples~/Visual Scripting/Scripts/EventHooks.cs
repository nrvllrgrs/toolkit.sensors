namespace ToolkitEngine.Sensors
{
    public static class EventHooks
    {
        public const string OnSensorFirstDetection = nameof(OnSensorFirstDetection);
        public const string OnSensorLastUndetection = nameof(OnSensorLastUndetection);
        public const string OnSensorSignalDetected = nameof(OnSensorSignalDetected);
        public const string OnSensorSignalUndetected = nameof(OnSensorSignalUndetected);

		// Perception
		public const string OnPerceptionDisturbance = nameof(OnPerceptionDisturbance);
        public const string OnPerceptionKnown = nameof(OnPerceptionKnown);
        public const string OnPerceptionUnknown = nameof(OnPerceptionUnknown);
		public const string OnPerceptionStimulusConfidenceChanged = nameof(OnPerceptionStimulusConfidenceChanged);
    }
}