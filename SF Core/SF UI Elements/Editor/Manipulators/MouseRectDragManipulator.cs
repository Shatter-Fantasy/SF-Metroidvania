using System;
using UnityEngine;
using UnityEngine.UIElements;


namespace SFEditor.UIElements.Utilities
{
    public class MouseRectDragManipulator : MouseManipulator
    {
        public Vector2 StartingPosition;
        public Vector2 DeltaPosition;
        public Vector2 DragPosition;
        public Vector2 EndingPosition;
        public Rect DragRect;


        public Matrix4x4 HandlesMatrix;
        public Vector2 InversedStartingPosition
            => HandlesMatrix.inverse.MultiplyPoint3x4(StartingPosition);
        public Vector2 InversedDeltaPosition
            => HandlesMatrix.inverse.MultiplyPoint3x4(DeltaPosition);
        public Vector2 InversedDragPosition
            => HandlesMatrix.inverse.MultiplyPoint3x4(DragPosition);
        public Vector2 InversedEndingPosition
            => HandlesMatrix.inverse.MultiplyPoint3x4(EndingPosition);
        public Rect InversedDragRect
            => new Rect(InversedStartingPosition, (InversedDragPosition - InversedStartingPosition));

        public Action<Rect> OnDragStartHandler;
        public Action<Rect> OnDragMoveHandler;
        public Action<Rect> OnDragEndHandler;

        public bool IsDragging = false;
        public bool CanDrag = false;
        public bool StopDragOnMouseLeave = true;

        private float _mouseDownDuration = 0;
        private float _mouseDownDurationLeft = 0;

        public MouseRectDragManipulator(MouseButton mouseButton,float mouseDownDuration, EventModifiers eventModifiers = EventModifiers.None)
        {
            activators.Add(new ManipulatorActivationFilter()
            {
                button = mouseButton,
                modifiers = eventModifiers
            });

            _mouseDownDuration     = mouseDownDuration;
            _mouseDownDurationLeft = _mouseDownDuration;
        }

        public MouseRectDragManipulator(MouseButton mouseButton,float mouseDownDuration ,EventModifiers eventModifiers = EventModifiers.None, Action<Rect> onDragStarted = null, Action<Rect> onDragMoved = null, Action<Rect> onDragEnded = null)
        {
            activators.Add(new ManipulatorActivationFilter()
            {
                button = mouseButton,
                modifiers = eventModifiers
            });

            _mouseDownDuration     = mouseDownDuration;
            _mouseDownDurationLeft = _mouseDownDuration;
            if(onDragStarted != null)
                OnDragStartHandler += onDragStarted;
            if(onDragMoved != null)
                OnDragMoveHandler += onDragMoved;
            if(onDragEnded != null)
                OnDragEndHandler += onDragEnded;
        }

        protected virtual void OnMouseDown(MouseDownEvent evt)
        {
            // If we are already dragging don't let the mouse down event trickle down to other UI Elements.
            if(IsDragging == true || CanDrag == true)
            {
                evt.StopPropagation();
            }

            if(CanStartManipulation(evt) && IsDragging == false && CanDrag == false)
            {
                if(_mouseDownDurationLeft <= 0)
                {
                    CanDrag = true;
                    StartingPosition = evt.mousePosition;
                    OnDragStartHandler?.Invoke(DragRect);
                    target.CaptureMouse();
                    evt.StopPropagation();
                }
                else
                {
                    _mouseDownDurationLeft -= Time.deltaTime;
                }
            }
        }

        protected virtual void OnMouseMove(MouseMoveEvent evt)
        {
            // If we can't drag don't worry about doing any mouse move event calculations.
            if(!CanStartManipulation(evt) || CanDrag == false)
                return;

            IsDragging = true;
            DragPosition = evt.mousePosition;
            DeltaPosition = DragPosition - StartingPosition;
            DragRect = new Rect(StartingPosition, DeltaPosition);
            OnDragMoveHandler?.Invoke(DragRect);
            evt.StopPropagation();
        }

        protected virtual void OnMouseUp(MouseUpEvent evt) 
        {
            /// If we are not already dragging don't worry about doing anything.
            /// This can happen when you click down with the mouse outside of the target UI element,
            /// than move the mouse inside of the Ui element before releasing the mouse click.
            /// In this sequence of input interaction the mouse down is never called on the target element first to start dragging.
            if(!CanStartManipulation(evt) || IsDragging == false)
            {
                evt.StopPropagation();
                return;
            }

            IsDragging = false;
            CanDrag = false;

            EndingPosition         = evt.mousePosition;
            DeltaPosition          = EndingPosition - StartingPosition;
            DragRect               = new Rect(StartingPosition, DeltaPosition);
            _mouseDownDurationLeft = _mouseDownDuration;
            OnDragEndHandler?.Invoke(DragRect);
            target.ReleaseMouse();
            evt.StopPropagation();
        }

        protected virtual void OnMouseLeave(MouseLeaveEvent evt)
        {
            if(StopDragOnMouseLeave)
            {
                EndingPosition = evt.mousePosition;
                DeltaPosition = EndingPosition - StartingPosition;
                DragRect = new Rect(StartingPosition, DeltaPosition);
                OnDragEndHandler?.Invoke(DragRect);

                StopDragging();
            }
            evt.StopPropagation();
        }

        /// <summary>
        /// Makes a Rect that will have positive values for width and height.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        private Rect AbsoluteRectValues(Vector2 startPosition, Vector2 endPosition )
        {
            float xPosition = (startPosition.x < endPosition.x)
                ? startPosition.x
                : endPosition.x;

            float yPosition = (startPosition.y < endPosition.y)
                ? startPosition.y
                : endPosition.y;

            float width = (startPosition.x < endPosition.x)
                ? endPosition.x - startPosition.x
                : startPosition.x - endPosition.x;

            float height = (startPosition.y < endPosition.y)
                ? endPosition.y - startPosition.y
                : startPosition.y - endPosition.y;

            return new Rect(xPosition, yPosition, width, height);
        }

        public void StopDragging(bool doKeepRect = false)
        {
            IsDragging = false;
            CanDrag = false;
            target.ReleaseMouse();
        }
       
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }
    }
}