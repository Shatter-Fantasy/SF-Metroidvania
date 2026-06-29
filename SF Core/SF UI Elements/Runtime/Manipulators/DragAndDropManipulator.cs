using UnityEngine;
using UnityEngine.UIElements;

namespace SF
{
	public class DragAndDropManipulator : PointerManipulator
	{
		private bool Enabled { get; set; }
		private VisualElement Root { get; set; }

		private Vector2 TargetStartPosition { get; set; }
		private Vector3 PointerStartPosition { get; set; }

		public DragAndDropManipulator(VisualElement target)
		{
			// VisualElement target is from the interface implemented by all manipulators. 
			this.target = target;
			Root = target.parent;
		}
		#region Pointer Handlers
		/// <summary>
		/// Stores the starting position of the target and the pointer.
		/// Also makes the target capture the pointer, and denotes the drag is now in progress via the enabled = true.
		/// </summary>
		/// <param name="evt"></param>
		private void PointerDownHandler(PointerDownEvent evt)
		{
			TargetStartPosition = target.resolvedStyle.translate;
			PointerStartPosition = evt.position;
			target.CapturePointer(evt.pointerId);
			Enabled = true;
		}

		/// <summary>
		/// This method checks if <see langword="abstract"/>drag is in progress and wether there is a captured pointer.
		/// If both are true, make the target release the pointer.
		/// </summary>
		/// <param name="evt"></param>
		private void PointerUpHandler(PointerUpEvent evt)
		{
			if(Enabled && target.HasPointerCapture(evt.pointerId))
				target.ReleasePointer(evt.pointerId);
		}

		/// <summary>
		/// This method checks wether a drag is in progress and wether the target has captured the pointer.
		/// If both are true, calculate a new position for target within the bounds of the current editor window. 
		/// </summary>
		/// <param name="evt"></param>
		private void PointerMoveHandler(PointerMoveEvent evt)
		{
			if(Enabled && target.HasPointerCapture(evt.pointerId)) 
			{ 
				Vector3 pointerDelta = evt.position - PointerStartPosition;

				// Make sure we can not leave the current panel's world bound aka element borders.
				target.style.translate = new Vector2(
					Mathf.Clamp(TargetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
					Mathf.Clamp(TargetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
			}
		}
		#endregion

		/// <summary>
		/// Checks to see if an Visual Element is overlapping another one. 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		private bool OverlapsTarget(VisualElement other)
		{
			// This needs to become an extension method to make life easier.
			return target.worldBound.Overlaps(other.worldBound);
		}

		private Vector3 RootSpaceOf(VisualElement element)
		{
			Vector2 elementWorldSpace = element.parent.LocalToWorld(element.layout.position);
			return Root.WorldToLocal(elementWorldSpace);
		}

		#region Registering Events
		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
			target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
			target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }
		#endregion
	}
}