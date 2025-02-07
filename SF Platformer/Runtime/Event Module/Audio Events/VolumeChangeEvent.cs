namespace SF.Events
{
    public enum VolumeChangeEventTypes
    {
        SetVolume,
        ChangeVolumeBy
    }
    public struct VolumeChangeEvent
    {
        public VolumeChangeEventTypes EventType;
        public float Volume;
        public string AudioChannelName;

        static VolumeChangeEvent volumeChangeEvent;

        public VolumeChangeEvent(VolumeChangeEventTypes eventType, float volume = 1, string audioChannelName = "Master")
        {
            EventType = eventType;
            Volume = volume;
            AudioChannelName = audioChannelName;
        }

        public static void Trigger(VolumeChangeEventTypes eventType, float volume)
        {
            volumeChangeEvent.EventType = eventType;
            volumeChangeEvent.Volume = volume;
            EventManager.TriggerEvent(volumeChangeEvent);
        }
        public static void Trigger(VolumeChangeEventTypes eventType, float volume, string audioChannelName)
        {
            volumeChangeEvent.EventType = eventType;
            volumeChangeEvent.Volume = volume;
            volumeChangeEvent.AudioChannelName = audioChannelName;
            EventManager.TriggerEvent(volumeChangeEvent);
        }

    }
}
