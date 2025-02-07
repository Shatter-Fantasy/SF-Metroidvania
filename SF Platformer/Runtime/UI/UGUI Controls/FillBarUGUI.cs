using System;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SF.UI
{
    [Serializable]
    public class FillBarEvent : UnityEvent<float> { }
    public enum FillBarDirection
    {
        LeftToRight,
        RightToLeft,
        UpToDown,
        DownToUp
    }

    [AddComponentMenu("UI/SF/Fill Bar UGUI")]
    [RequireComponent(typeof(RectTransform))]
    public class FillBarUGUI : UIBehaviour, ICanvasElement
    {
        [SerializeField] protected Image _fillImage;
        [SerializeField] protected TMP_Text _valueText;
        [SerializeField] protected FillBarDirection _fillDirection;

        public bool WholeNumbers = false;

        [SerializeField] protected float _value;
        public virtual float Value
        {
            get { return WholeNumbers ? Mathf.Round(_value) : _value; }
            set { Set(value);  }
        }

        [SerializeField] protected FillBarEvent _onValueChanged = new FillBarEvent();

        public void SetValueWithoutNotify(float value)
        {
            Set(value, false);
        }

        protected virtual void Set(float input, bool sendCallback = true)
        {
            if(sendCallback)
            {
                Value = input;
            }
            else
            {
                _value = input;
            }

            if(_fillImage != null)
                _fillImage.fillAmount = _value;
        }

        #region Not needed for our elements, but interface requires it.
        public virtual void GraphicUpdateComplete()
        {}

        public void LayoutComplete()
        {}
        #endregion

        public void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if(executing == CanvasUpdate.Prelayout)
                _onValueChanged?.Invoke(Value);
#endif
        }
    }

}
