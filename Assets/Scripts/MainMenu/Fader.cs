using System;
using UnityEngine;

namespace MainMenu
{
    public class Fader : MonoBehaviour
    {
        private const string FaderPath = "UI/Fader";
    
        [SerializeField] private Animator animator;
    
        private static Fader _instance;

        public static Fader Instance
        {
            get
            {
                if (null == _instance)
                {
                    Fader prefab = Resources.Load<Fader>(FaderPath);
                    _instance = Instantiate(prefab);
                    DontDestroyOnLoad(_instance.gameObject);
                }
            
                return _instance;
            }
        }

        private Action _fadedInCallback;
        private Action _fadedOutCallback;
        private bool _isFading;

        public void FadeIn(Action fadedInCallback)
        {
            if (_isFading)
            {
                return;
            }
        
            _isFading = true;
            _fadedInCallback = fadedInCallback;
        
            animator.SetBool("faded", true);
        }

        public void FadeOut(Action fadedOutCallback)
        {
            if (_isFading)
            {
                return;
            }
        
            _isFading = true;
            _fadedOutCallback = fadedOutCallback;
        
            animator.SetBool("faded", false);
        }

        private void Handle_FadeInAnimationOver()
        {
            _fadedInCallback?.Invoke();
            _fadedInCallback = null;
            _isFading = false;
        }
    
        private void Handle_FadeOutAnimationOver()
        {
            _fadedOutCallback?.Invoke();
            _fadedOutCallback = null;
            _isFading = false;
        }
    }
}
