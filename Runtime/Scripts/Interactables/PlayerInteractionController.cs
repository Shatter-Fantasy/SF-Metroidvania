using System;
using UnityEngine;
using UnityEngine.InputSystem;

using SF.Characters.Controllers;
using SF.InputModule;
using SF.PhysicsLowLevel;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    public class PlayerInteractionController : InteractionController, ITriggerShapeCallback
    {
        private PlayerController _controller;
        
        private void Awake()
        {
            TryGetComponent(out _controller);
            TryGetComponent(out _hitShape);
        }
        
              
        private void OnEnable()
        {
            SFInputManager.Controls.Player.Interact.performed += OnInteractPerformed;
            if(_hitShape != null)
                _hitShape.AddTriggerCallbackTarget(this);
        }

        private void OnDisable()
        {
            SFInputManager.Controls.Player.Interact.performed -= OnInteractPerformed;
            if(_hitShape != null)
                _hitShape.RemoveTriggerCallbackTarget(this);
        }


        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            // Grab the body data.
            var shapeComponent = beginEvent.GetCallbackComponentOnVisitor<SFShapeComponent>();

            if (!shapeComponent.TryGetComponent(out IInteractable interactable)
                || interactable.InteractableMode != InteractableMode.Collision) 
                return;
            
            if(interactable is IInteractable<PlayerController> interactableController
               && _controller is not null)
                interactableController.Interact(_controller);
            else
                interactable.Interact();
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent) { }

        protected void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if(!_hitShape.Shape.isValid) return;

            _hitShapes            = new NativeArray<PhysicsShape>(5, Allocator.Temp);
            _castInput.shapeProxy = _hitShape.ShapeProxy;
            using var result = _hitShape.PhysicsWorld.OverlapShape(_hitShape.Shape, _interactableFilter);

            if (result.Length < 0)
                return;

            // This is a painful looking thing, but it is actually decent performance, so oh well.
            for (int i = 0; i < result.Length; i++)
            {
                // Grab the body data.
                var objectData = result[i].shape.body.userData.objectValue;

                if (objectData is not GameObject hitObject
                    || !hitObject.TryGetComponent(out IInteractable interactable)
                    || interactable.InteractableMode != InteractableMode.Input) 
                    continue;
                
                if(interactable is IInteractable<PlayerController> interactableController
                   && _controller is not null)
                    interactableController.Interact(_controller);
                else
                    interactable.Interact();
            }
        }
  
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            OnTriggerBegin2D(beginEvent);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // noop - No Operation
        }
    }
}
