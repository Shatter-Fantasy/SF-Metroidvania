using UnityEngine;
using SF.Events;
using System;
using SF.DamageModule;
using Unity.Properties;

namespace SF.SpawnModule
{
    /// <summary>
    /// Adds a health system to anything. 
    /// This does not need to be on a character. You can add this to a crate or anything that wants to be damaged. There are checks in do stuff for character specific objects if you want to.
    /// </summary>
    public class Health : MonoBehaviour, IDamagable, EventListener<RespawnEvent>
    {
        /// <summary>
        /// Is the character health being controlled by an external script to 
        /// add specific event handling logic.
        ///
        /// Example SF.Metroidvania package CharacterStats relays event data after 
        /// damage calculations to the character health script.
        /// </summary>
        public IDamageController DamageController;

        [SerializeField] private int _currentHealth;
        [CreateProperty] public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                int previousValue = _currentHealth;
                _currentHealth = value;

                if(previousValue != _currentHealth)
                    HealthChangedCallback?.Invoke(_currentHealth);
            }
        }
        public Action<int> HealthChangedCallback;
        
        public int MaxHealth = 10;

        [Header("SFX")]
        [SerializeField] protected AudioClip _deathSFX;

        public virtual void TakeDamage(int damage, Vector2 knockback = new Vector2())
        {
            if(DamageController != null)
                damage = DamageController.CalculateDamage(damage);

            if(!gameObject.activeSelf)
                return;

            CurrentHealth -= damage;

            if(CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Kill();
            }
        }

        public virtual void FullHeal()
        {
           
            CurrentHealth = MaxHealth;
        }

        public virtual void InstantKill()
        {
            CurrentHealth = 0;
            Kill();
		}

        protected virtual void Kill()
        {
            if(_deathSFX != null)
                AudioManager.Instance.PlayOneShot(_deathSFX);

            gameObject.SetActive(false);
        }

		public void OnEvent(RespawnEvent respawnEvent)
		{
			switch (respawnEvent.EventType) 
            {
                case RespawnEventTypes.PlayerRespawn:
                    Respawn();
                    break;
            }
		}

        protected virtual void Respawn()
        {
            CurrentHealth = MaxHealth;
            gameObject.SetActive(true);
        }

		protected virtual void OnEnable()
		{
            this.EventStartListening<RespawnEvent>();
		}
		protected virtual void OnDestroy()
        {
			this.EventStopListening<RespawnEvent>();
		}
    }
}