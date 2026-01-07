using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.DamageModule
{
	public class ZoneKill : MonoBehaviour, PhysicsCallbacks.ITriggerCallback
    {
		public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
		{
			if(((GameObject)beginEvent.visitorShape.callbackTarget)
				.TryGetComponent(out IDamagable damageable))
			{
				damageable.InstantKill();
			}
		}

		public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
		{
			// We only implement this because of the PhysicsCallbacks.ITriggerCallback interface.
		}
    }
}
