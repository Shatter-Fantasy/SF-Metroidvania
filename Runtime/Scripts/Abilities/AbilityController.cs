using System.Collections.Generic;
using System.Linq;
using SF.Characters.Controllers;
using SF.Physics;
using SF.PhysicsLowLevel;
using UnityEngine;

namespace SF.AbilityModule
{
    // Setting the default execution order past Controller2D.
    // This guarantees the controller is already set up it's current physic struct in case any 
    
    /// <summary>
    /// This is the <see cref="PlayerController"/> specific controller for abilities that only players use.
    ///
    /// For NPC, Enemies, or Ally combatants not controlled by the Players use <see cref="SF.StateMachine.Core.StateMachineBrain"/>
    /// <remarks>
    /// The AbilityController default execution order is set one past the Controller2D.
    /// This guarantees the controller is already set up it's current physic struct in case any external force is starting to change it.
    /// <see cref="SF.Physics.PhysicsVolume"/> 
    /// </remarks>
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class AbilityController : MonoBehaviour
    {
        //The gameobject the abilities will control.
        public GameObject AbilityOwner;
        public List<AbilityCore> Abilities = new List<AbilityCore>();
        
        private PhysicController2D _physicController2D;

        private void Awake()
        {
            Abilities = GetComponents<AbilityCore>().ToList();
            
            _physicController2D = AbilityOwner != null 
                ? AbilityOwner.GetComponent<PhysicController2D>() 
                : GetComponent<PhysicController2D>();
            
            // Set the correct type of controller so the abilities can do different things based on if it is player, a enemy affected by gravity, or another type.
            _physicController2D = _physicController2D switch
            {
                PlayerController playerController => playerController,
                ControllerBody2D controllerBody2D => controllerBody2D,
                _ => _physicController2D
            };
        }
        private void Start()
        {
            for (int i = 0; i < Abilities.Count; i++)
            {
                Abilities[i].Initialize(_physicController2D);
            }
        }
    }
}
