using System.Collections.Generic;

using UnityEngine;

namespace SF
{
    public class AnimationActivatable : ActivatableWrapper, IActivatable
    {
        public Animator _animator;
        public int _parameterHash;
        private List<int> _parameterHashes = new List<int>();
        public bool _hasAnimParameter = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if(_animator == null) 
                return;

            _parameterHash = Animator.StringToHash("IsActivated");

            for(int i = 0; i < _animator.parameters.Length; i++)
            {
                _parameterHashes.Add(_animator.parameters[i].nameHash);
            }

            if(_parameterHashes.Contains(_parameterHash))
                _hasAnimParameter = true;
        }

        protected override void OnActivation()
        {

            if(_animator == null && _hasAnimParameter)
                return;

            _animator.SetBool("IsActivated", true);
        }

        protected override void OnDeactivate()
        {
            if(_animator == null && _hasAnimParameter)
                return;

            _animator.SetBool("IsActivated", false);
        }
    }
}
