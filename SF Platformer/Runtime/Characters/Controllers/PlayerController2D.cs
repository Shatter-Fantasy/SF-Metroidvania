using Unity.Cinemachine;

using UnityEngine;

using SF.CameraModule;
using System.Collections.Generic;

namespace SF.Characters.Controllers
{
	public class PlayerController : GroundedController2D
    {

        protected override void OnAwake()
        {
            base.OnAwake();

            if(gameObject.TryGetComponentInChildren(out CinemachineCamera cinemachineCamera))
                CameraController.SetPlayerCMCamera(cinemachineCamera);
        }
    }

    public static class GameObjectExtension
    {
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();

            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();

            return component != null;
        }
    }
}