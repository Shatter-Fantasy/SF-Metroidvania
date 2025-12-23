using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace SF.UIModule
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
    
    public partial class FillBarUGUI : VisualElement
    {
        [SerializeField] protected Image _fillImage;
        [SerializeField] protected Label _valueLabel;
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

            if (_fillImage != null)
            {
                //_fillImage.fillAmount = _value;
            }
        }
    }
}
