using System.Collections.Generic;
using System.Linq;
using SF.Characters.Controllers;
using UnityEngine;

namespace SF.AbilityModule
{
    // Setting the default execution order past Controller2D.
    // This gurantees the controller is already set up it's current physics struct.
    [DefaultExecutionOrder(1)]
    public class AbilityController : MonoBehaviour
    {
        //The gameobject the abilities will control.
        public GameObject AbilityOwner;
        public List<AbilityCore> Abilities = new List<AbilityCore>();

        private Controller2D _controller2D;

        private void Awake()
        {
            Abilities = GetComponents<AbilityCore>().ToList();

            
            _controller2D = AbilityOwner != null ? AbilityOwner.GetComponent<Controller2D>() : GetComponent<Controller2D>();
            
            // Set the correct type of controller so the abilities can do different things based on if it is player, a enemy affected by gravity, or another type.
            _controller2D = _controller2D switch
            {
                PlayerController playerController => playerController,
                GroundedController2D groundedController2D => groundedController2D,
                _ => _controller2D
            };
        }
        private void Start()
        {
            for (int i = 0; i < Abilities.Count; i++)
            {
                Abilities[i].Initialize(_controller2D);
            }
        }
    }
}
