using System;
using UnityEngine;
using UnityEngine.InputSystem;

using SF.Characters.Controllers;
using SF.InputModule;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    public class PlayerInteractionController : InteractionController, PhysicsCallbacks.ITriggerCallback
    {
        private PlayerController _controller;
        
        private void Awake()
        {
            TryGetComponent(out _controller);
            
            if (!TryGetComponent(out _hitShape))
                return;
            
            _hitShape.ShapeCreatedHandler   += OnShapeCreated;
            _hitShape.ShapeDestroyedHandler += OnShapeDestroyed;
        }

        private void OnDestroy()
        {
            if (_hitShape == null)
                return;
            
            _hitShape.ShapeCreatedHandler   -= OnShapeCreated;
            _hitShape.ShapeDestroyedHandler -= OnShapeDestroyed;
        }
        
        private void OnShapeCreated()
        {
            Debug.Log("Creating Shape");
           // if (_hitShape != null)
                //_hitShape.SetCallbackTarget(gameObject);
        }

        private void OnShapeDestroyed()
        {
            //if (_hitShape != null)
               // _hitShape.ClearCallbackTarget();
        }
  

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            Debug.Log("Player Interaction Controller");
            
            if (beginEvent.visitorShape.callbackTarget is GameObject hitObject
                && hitObject.TryGetComponent(out IInteractable interactable)
                && interactable.InteractableMode == InteractableMode.Collision)
            {
                if(interactable is IInteractable<PlayerController> interactableController
                   && _controller is not null)
                    interactableController.Interact(_controller);
                else
                    interactable.Interact();
            }
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
                if (result[i].shape.callbackTarget is GameObject hitObject 
                    && hitObject.TryGetComponent(out IInteractable interactable)
                    && interactable.InteractableMode == InteractableMode.Input)
                {
                    if(interactable is IInteractable<PlayerController> interactableController
                            && _controller is not null)
                        interactableController.Interact(_controller);
                    else
                        interactable.Interact();
                }
            }
        }
        
        private void OnEnable()
        {
            SFInputManager.Controls.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            SFInputManager.Controls.Player.Interact.performed -= OnInteractPerformed;
        }
        
    }
}
