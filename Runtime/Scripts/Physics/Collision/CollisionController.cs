using UnityEngine;

namespace SF.Physics
{
	/// <summary>
	/// Controls the values passed into physics.2D calls to allow for custom collision detection systems.
	/// </summary>
	[System.Serializable]
    public struct CollisionController
    {
		[Header("Collision Corection")]
		public float SkinWidth;

		[Header("Ray distance")]
		public float HoriztonalRayDistance;
		public float VerticalRayDistance;

		[Header("Ray Amount")]
		public int HoriztonalRayAmount;
		public int VerticalRayAmount;

		[field: Header("Ray Offsets")]
        [field: SerializeField] public float RayOffset { get; private set; }

        [HideInInspector] public RaycastHit2D[] RaycastHit2Ds;

		public CollisionController(float horiztonalRayDistance = 0.01f,
							 float verticalRayDistance = 0.01f,
							 short horiztonalRayAmount = 3,
							 short verticalRayAmount = 3,
							 float horiztonalOffset = 0.01f,
							 float skinWidth = 0.02f)
		{
			RayOffset = horiztonalOffset;
			HoriztonalRayDistance = horiztonalRayDistance;
			VerticalRayDistance = verticalRayDistance;
			HoriztonalRayAmount = horiztonalRayAmount;
			VerticalRayAmount = verticalRayAmount;
			SkinWidth = skinWidth;
			RaycastHit2Ds = new RaycastHit2D[4];
		}


		/// <summary>
		/// NOT IMPLEMENTED YET.
		/// Check the <see cref="AIModule.AICrushAction"/> for the bitwise operator method I would like to implement here.
		/// 
		/// Checks if anything in the collision controllers RayCastHit2D array matches the layer mask. 
		/// This allows reusing the already cached raycast hits for better performance.
		/// </summary>
		/// <param name="layerMask"></param>
		/// <returns></returns>
		public bool DidHitObjectInLayerMask(LayerMask layerMask)
		{
			Debug.Log("The DidHitObjectInLayerMask in CollisionController is not implemented yet and currently always returns true.");
			return true;
		}
	}
}
