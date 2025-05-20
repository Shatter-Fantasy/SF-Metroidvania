namespace SF.SwitchesModule
{
    public class PSwitch : UnityEngine.MonoBehaviour, IStompable
    {
        public float PTimeAmount = 3f;
        public static PSwitchTimer PTimer;
        public static bool IsActivated = false;

        public void Stomp()
        {
            // Could put the new PTimer call here for custom timer passing.
            PTimer = new(PTimeAmount);
            PTimer.StartTimer();
        }
    }
}
