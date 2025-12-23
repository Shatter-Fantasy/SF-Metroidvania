using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SF.Characters.Controllers;
using SF.Managers;
using SF.PhysicsLowLevel;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.RoomModule
{
    public class RoomController : MonoBehaviour, PhysicsCallbacks.ITriggerCallback
    {
        
        /* TODO List:
            Room Auto Align: Make a method that allows taking in two transforms.
            each transform is the floor of two connected rooms. 
            We can round the x/y values of the transform to make sure they align perfect.
            We might have to make one room round using ceiling and one round using floor depending on the values.         */
        
        
        /// <summary>
        /// The id for the room's spawned instance the RoomController is controlling.
        /// </summary>
        public int RoomID;
        [NonSerialized] public List<int> RoomIdsToLoadOnEnter = new();
        /// <summary>
        /// The camera confined to the room.
        /// </summary>
        public CinemachineCamera RoomCamera;

        /// <summary>
        /// These are optional transition ids for when room controller needs to keep track of fast travel points or using <see cref="TransitionTypes.Local"/>.
        /// </summary>
        public List<RoomTransition> RoomTransitions = new List<RoomTransition>();

        public Action OnRoomEnteredHandler;
        public Action OnRoomExitHandler;

        #region Room Extensions
        /// <summary>
        /// Unfiltered list of all <see cref="IRoomExtension"/> that are connected to this room. 
        /// </summary>
        private readonly List<IRoomExtension> _roomExtensions = new();

        private ReadOnlyCollection<IRoomExtension> _roomEnteredExtensions;
        private readonly List<IRoomExtension> _roomExitedExtensions = new();
        #endregion
        
        [SerializeReference] private SFShapeComponent _physicsShapeComponent;
        private void Awake()
        {
            if (TryGetComponent(out _physicsShapeComponent))
            {
                _physicsShapeComponent.CallbackTarget = this;
                _physicsShapeComponent.BodyDefinition.type      = PhysicsBody.BodyType.Static;
            }
             
            // This is the ignore ray cast physics layer.
            gameObject.layer = 2;
            
            // Non-allocating version when used with read only List<T>
            gameObject.GetComponents(_roomExtensions);
            
            var rooms  = _roomExtensions.Where((room => room.RoomExtensionType == RoomExtensionType.OnRoomEntered));
            _roomEnteredExtensions = new ReadOnlyCollection<IRoomExtension>(rooms.ToList());
        }

        private void Start()
        {
            if (RoomSystem.RoomDB == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no database set in the {nameof(RoomSystem)}");
                return;
#endif
            }
            if (RoomSystem.RoomDB[RoomID] == null)
            {
                Debug.LogWarning($"A room with the RoomID of {RoomID} was not found in the RoomDatabase. Check if there was a room with the id of {RoomID} set inside the RoomDatabase");
                return;
            }
            RoomIdsToLoadOnEnter = RoomSystem.RoomDB[RoomID].ConnectedRoomsIDs;
        
            RoomSystem.LoadRoomManually(RoomID, gameObject);
        }

        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }
        
        /// <summary>
        /// Changes the current room and invokes all the required CameraSystem, RoomSystem, and GameManagers calls. 
        /// </summary>
        public void MakeCurrentRoom()
        {
            if (!RoomSystem.IsRoomLoaded(RoomID))
            {
                return;
            }

            // Probably should put this if and for loop in the RoomSystem itself.
            if (RoomSystem.DynamicRoomLoading)
            {
                foreach (var roomID in RoomIdsToLoadOnEnter)
                {
                    RoomSystem.LoadRoom(roomID);
                }
            }

            RoomSystem.SetCurrentRoom(RoomID);
        }

       
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
     
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;

            var visitorTarget = beginEvent.visitorShape.callbackTarget;
            if (visitorTarget is not PlayerController body2D)
            {
                if (visitorTarget is not GameObject visitor)
                    return;

                if(!visitor.TryGetComponent(out body2D))
                    return;
            }

            if (!body2D.CollisionInfo.CollisionActivated)
                return;
            
            OnRoomEnteredHandler?.Invoke();
            for (int i = 0; i < _roomEnteredExtensions.Count; i++)
            {
                _roomEnteredExtensions[i].Process();
            }
            
            MakeCurrentRoom();
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
        {
            OnRoomExitHandler?.Invoke();
        }
    }
}