using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIElements.Utilities
{
    public enum BorderSide
    {
        AllSides,
        LeftSide,
        RightSide,
        TopSide,
        BottomSide,
    }

    public static class VisualElementsExtensions
    {
        public static VisualElement AddVisualElement(string name = "")
        {
            // TODO: Do some validation on the passed in name.
            VisualElement newElement = new VisualElement()
            {
                name = name
            };
            return newElement;
        }
        
        
        
        public static T AddRow<T>(this T parent) where T : VisualElement
        {
            SFRow row = new();
            return parent.AddChild(row);
        }
        
        public static T AddRow<T>(this T parent, VisualElement child) where T : VisualElement
        {
            SFRow row = new();
            if(child != null)
                row.Add(child);
            
            return parent.AddChild(row);
        }

        /// <summary>
        /// Adds the sf-row--item uss class name to the first depth children.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T SetChildrenAsRowItems<T>(this T parent) where T : VisualElement
        {
            foreach(var child in parent.Children())
            {
                child.AddToClassList("sf-row--item");
            }
            return parent;
        }

        public static T AddChild<T>(this T parent, VisualElement child, string className = "") where T : VisualElement
        {
            if(!string.IsNullOrEmpty(className))
                child.AddToClassList(className);

            parent.Add(child);
            return parent;
        }
        public static T AddChild<T>(this T parent, VisualElement child, string[] classNames) where T : VisualElement
        {
            parent.CheckNullElements(child);

            // Make sure we are not passing in an empty array
            if(classNames.Length > 0)
            {
                for(int i = 0; i < classNames.Length; i++)
                {
                    if(!string.IsNullOrEmpty(classNames[i]))
                        child.AddToClassList(classNames[i]);
                }
            }

            parent.Add(child);
            return parent;
        }

        /// <summary>
        /// Will log an error if either the child or parent UI Element is null.
        /// Doesn't stop the sequence of method chaining in progress. You have to do that in the calling method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="bindingPath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static T AddBindableChild<T,U>(this T parent, U child, string bindingPath, string className = "") where T : VisualElement where U : BindableElement
        {
            parent.CheckNullElements(child);

            if(!string.IsNullOrEmpty(bindingPath))
                child.bindingPath = bindingPath;

            return parent.AddChild(child,className);
        }

        private static T CheckNullElements<T,U>(this T parent, U child) where T : VisualElement where U : VisualElement
        {
            if(parent == null)
            {
                Debug.LogError("The parent visual element that is having a child element added to is null.");
                return null;
            }

            if(child == null)
            {
                Debug.LogError($"The passed in child trying to be added to {parent} is null.");
                return parent;
            }

            return parent;
        }

        public static T AddClass<T>(this T target, string className) where T : VisualElement
        {
            if(!string.IsNullOrEmpty(className))
                target.AddToClassList(className);
            return target;
        }
        /// <summary>
        /// Adds an every class name in the passed array to the class name list for the targetted visual element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public static T AddClass<T>(this T target, string[] classNames) where T : VisualElement
        {
            if(classNames.Length > 0)
            {
                for(int i = 0; i < classNames.Length; i++)
                {
                    if(!string.IsNullOrEmpty(classNames[i]))
                        target.AddToClassList(classNames[i]);
                }
            }
            return target;
        }

        public static T Rename<T>(this T target, string newName) where T : VisualElement
        {
            if(target != null)
                target.name = newName;
            return target;
        }

        public static T SetFontSize<T>(this T target, int fontSize) where T : VisualElement
        {
            target.style.fontSize = fontSize;
            return target;
        }

        public static T SetWidth<T>(this T target, int width, LengthUnit lengthUnit) where T : VisualElement
        {
            target.style.width = new Length(width, lengthUnit);
            return target;
        }

        public static T SetAllMargin<T>(this T target, int margin, LengthUnit lengthUnit = LengthUnit.Pixel) where T : VisualElement
        {
            target.style.marginTop = new Length(margin, lengthUnit);
            target.style.marginBottom = new Length(margin, lengthUnit);
            target.style.marginLeft = new Length(margin, lengthUnit);
            target.style.marginRight = new Length(margin, lengthUnit);
            return target;
        }

        public static T SetAllPadding<T>(this T target, int padding, LengthUnit lengthUnit = LengthUnit.Pixel) where T : VisualElement
        {
            target.style.paddingTop = new Length(padding, lengthUnit);
            target.style.paddingBottom = new Length(padding, lengthUnit);
            target.style.paddingLeft = new Length(padding, lengthUnit);
            target.style.paddingRight = new Length(padding, lengthUnit);
            return target;
        }

        public static T SetBorder<T>(this T target, 
            int borderWidth, Color borderColor, 
            BorderSide borderSide = BorderSide.TopSide) where T : VisualElement
        {
            switch(borderSide)
            {
                case BorderSide.AllSides:
                    {
                        target.SetAllBorders(borderWidth, borderColor);
                        break;
                    }
                case BorderSide.LeftSide:
                    {
                        target.style.borderLeftColor = borderColor;
                        target.style.borderLeftWidth = borderWidth;
                        break;
                    }
                case BorderSide.RightSide:
                    {
                        target.style.borderRightColor = borderColor;
                        target.style.borderRightWidth = borderWidth;
                        break;
                    }
                case BorderSide.TopSide:
                    {
                        target.style.borderTopColor = borderColor;
                        target.style.borderTopWidth = borderWidth;
                        break;
                    }
                case BorderSide.BottomSide:
                    {
                        target.style.borderBottomColor = borderColor;
                        target.style.borderBottomWidth = borderWidth;
                        break;
                    }
            }
            return target;
        }


        public static T SetAllBorders<T>(this T target, int borderWidth, Color borderColor) where T : VisualElement
        {
            target.style.borderTopColor = borderColor;
            target.style.borderRightColor = borderColor;
            target.style.borderBottomColor = borderColor;
            target.style.borderLeftColor = borderColor;

            target.style.borderTopWidth = borderWidth;
            target.style.borderRightWidth = borderWidth;
            target.style.borderLeftWidth = borderWidth;
            target.style.borderBottomWidth = borderWidth;

            return target;
        }

        public static T SetAllBordersRadius<T>(this T target, int borderRadius) where T : VisualElement
        {
            target.style.borderTopRightRadius = borderRadius;
            target.style.borderTopLeftRadius = borderRadius;
            target.style.borderBottomRightRadius = borderRadius;
            target.style.borderBottomLeftRadius = borderRadius;

            return target;
        }

        /// <summary>
        /// Sets the display style of a Visual Element and returns the element to allow method chaining.
        /// </summary>
        /// <remarks>
        /// Advised to use a null propagation (target?.) check during the method chaining to always make sure
        /// you don't try chaining on a null value.
        /// </remarks>
        /// <param name="target"></param>
        /// <param name="displayStyle"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T SetDisplayStyle<T>(this T target, DisplayStyle displayStyle) where T : VisualElement
        {
            if (target != null)
                target.style.display = displayStyle;
            
            return target;
        }
        
        
        /* Start of the event registering extensions. */
        public static T AddCallback<T,TEventType>(this T target, EventCallback<TEventType> callback)
            where T : VisualElement
            where TEventType : EventBase<TEventType>, new()
        {
            if (target == null)
            {
               Debug.LogWarning("When trying to register a UI Event the target UI was null.");
            }
            
            target?.RegisterCallback<TEventType>(callback);
            return target;
        }
        
        public static T Register<T,TEventType>(this T target, EventCallback<TEventType> callback)
            where T : VisualElement
            where TEventType : EventBase<TEventType>, new()
        {
            if (target == null)
            {
                Debug.LogWarning("When trying to register a UI Event the target UI was null.");
            }
            
            target?.RegisterCallback<TEventType>(callback);
            return target;
        }
    }
}
