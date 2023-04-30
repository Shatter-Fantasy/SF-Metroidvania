using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF.Physics.Helpers
{
	[System.Serializable]
	public struct BoundsData
	{
		[field: Header("Bounds")]
		public Bounds Bounds;

		public Vector2 TopRight => new Vector2(Bounds.max.x, Bounds.max.y);
		public Vector2 TopCenter => new Vector2(Bounds.center.x, Bounds.max.y);
		public Vector2 TopLeft => new Vector2(Bounds.min.x, Bounds.max.y);

		public Vector2 BottomRight => new Vector2(Bounds.max.x, Bounds.min.y);
		public Vector2 BottomCenter => new Vector2(Bounds.center.x, Bounds.min.y);
		public Vector2 BottomLeft => new Vector2(Bounds.min.x, Bounds.min.y);

		public Vector2 MiddleRight => new Vector2(Bounds.max.x, Bounds.center.y);
		public Vector2 MiddleCenter => new Vector2(Bounds.center.x, Bounds.center.y);
		public Vector2 MiddleLeft => new Vector2(Bounds.min.x, Bounds.center.y);

		public BoundsData(Collider2D collider2D)
		{
			Bounds = collider2D.bounds;
		}
	}
}