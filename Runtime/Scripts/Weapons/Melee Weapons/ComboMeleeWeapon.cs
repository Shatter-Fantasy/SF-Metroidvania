using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.Weapons
{
    using CombatModule;
    
    public class ComboMeleeWeapon : MeleeWeapon
    {
        [Header("Combo Properties")]
        [SerializeField] private int _comboIndex = 0;
        [SerializeField] protected ComboType _comboType;
        public List<ComboAttack> ComboAttacks = new();
        
        /// <summary>
        /// The timer to keep track of time between combo attacks to see if a combo should continue if not a lot of time has passed.
        /// </summary>
        [SerializeField] protected Timer _comboTimer;

        protected override void Awake()
        {
            _attackTimer = new Timer(ComboAttacks[0].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[0].HitBoxDelay, OnHitBoxDelay);
            _comboTimer  = new Timer(ComboAttacks[0].ComboInputDelay, OnComboReset);
            
            
            if(_controllerBody2D != null)
                _controllerBody2D.OnDirectionChanged += OnDirectionChange;
        }
        
        protected override void DoAttack()
        {
            if(_character2D != null)
                _character2D.SetAnimationState(
                        ComboAttacks[_comboIndex].Name, 
                        ComboAttacks[_comboIndex].AttackAnimationClip.length
                    );
            
            _attackTimer = new Timer(ComboAttacks[_comboIndex].AttackTimer, OnUseComplete);
            _hitBoxTimer = new Timer(ComboAttacks[_comboIndex].HitBoxDelay, OnHitBoxDelay);
            
            // Stop the previous combo timer.
            _comboTimer.StopTimer();
            _comboTimer = new Timer(ComboAttacks[_comboIndex].ComboInputDelay, OnComboReset);
            _           = _hitBoxTimer.StartTimerAsync();
            _           = _attackTimer.StartTimerAsync();
            _           = _comboTimer.StartTimerAsync();

            _comboIndex++;
            
            if (_comboIndex >= ComboAttacks.Count)
                _comboIndex = 0;
            
            OnCooldown = true;
        }
        
        protected override void OnUseComplete()
        {
            _comboTimer.StopTimer();
            base.OnUseComplete();
        }
        
        private void OnComboReset()
        {
            _comboIndex = 0;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Syncs all the attack timers to match the length of the animation clip length for that attack animation.
        /// </summary>
        [ContextMenu("Sync attack and animation timers.")]
        protected virtual void SetAllAttacksTimerViaAnimation()
        {
            AnimationClip clip;
            
            for (int i = 0; i < ComboAttacks.Count; i++)
            {
                clip = ComboAttacks[i].AttackAnimationClip;
                
                float targetFrameTimer = (ComboAttacks[i].HitBoxAnimationFrame + 1) * (float)Math.Round((1f / clip.frameRate), 3);
                ComboAttacks[i].AttackTimer = clip.length;
                ComboAttacks[i].HitBoxDelay = targetFrameTimer;
            }
        }
#endif
    }
}
