using UnityEngine;

namespace SF.PhysicsLowLevel
{
	[System.Serializable]
	public struct BoundsData
	{
		[field: Header("Bounds")]
		public Bounds Bounds;

		public Vector2 TopRight => new (Bounds.max.x, Bounds.max.y);
		public Vector2 TopCenter => new (Bounds.center.x, Bounds.max.y);
		public Vector2 TopLeft => new (Bounds.min.x, Bounds.max.y);

		public Vector2 BottomRight => new (Bounds.max.x, Bounds.min.y);
		public Vector2 BottomCenter => new (Bounds.center.x, Bounds.min.y);
		public Vector2 BottomLeft => new (Bounds.min.x, Bounds.min.y);

		public Vector2 MiddleRight => new (Bounds.max.x, Bounds.center.y);
		public Vector2 MiddleCenter => new (Bounds.center.x, Bounds.center.y);
		public Vector2 MiddleLeft => new (Bounds.min.x, Bounds.center.y);

		public BoundsData(Collider2D collider2D)
		{
			Bounds = collider2D.bounds;
		}
		public void UpdateBounds(Collider2D collider2D)
		{
			Bounds = collider2D.bounds;
		}
	}
}