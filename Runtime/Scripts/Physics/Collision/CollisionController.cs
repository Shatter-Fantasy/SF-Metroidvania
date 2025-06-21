using UnityEngine;

namespace SF.Physics
{
	/// <summary>
	/// Controls the values passed into physics.2D calls to allow for custom collision detection systems.
	/// </summary>
	[System.Serializable]
    public struct CollisionController
    {
	    public bool CollisionActivated;
	    
		[Header("Collision Correction")]
		public float SkinWidth;

		[Header("Ray distance")]
		public float HoriztonalRayDistance;
		public float VerticalRayDistance;

		[Header("Ray Amount")]
		public int HoriztonalRayAmount;
		public int VerticalRayAmount;
		

        [HideInInspector] public RaycastHit2D[] RaycastHit2Ds;

		public CollisionController(float horiztonalRayDistance = 0.01f,
							 float verticalRayDistance = 0.01f,
							 short horiztonalRayAmount = 3,
							 short verticalRayAmount = 3,
							 float horiztonalOffset = 0.01f,
							 float skinWidth = 0.02f)
		{
			HoriztonalRayDistance = horiztonalRayDistance;
			VerticalRayDistance = verticalRayDistance;
			HoriztonalRayAmount = horiztonalRayAmount;
			VerticalRayAmount = verticalRayAmount;
			SkinWidth = skinWidth;
			RaycastHit2Ds = new RaycastHit2D[4];

			CollisionActivated = true;
		}
	}
}
