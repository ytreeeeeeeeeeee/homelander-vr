using System;
using System.Collections;
using UnityEngine;

namespace Game.Homelander
{
    public class LaserAudio : MonoBehaviour
    {
        [Header("Audio sources")] 
        [SerializeField] private AudioSource startSource;
        [SerializeField] private AudioSource loopSource;
        [SerializeField] private AudioSource endSource;
        
        [Header("Audio clips")]
        [SerializeField] private AudioClip startClip;
        [SerializeField] private AudioClip endClip;
        
        [Header("Transition")]
        [SerializeField, Min(0f)] private float transitionDuration = 0.15f;

        [SerializeField, Min(0f)] private float audioCompletionDuration = 1f;
        
        public event Action OnCompleted;

        private Coroutine _transitionRoutine;
        private bool _isLaserActive;

        private void Awake()
        {
            startSource.volume = 1f;
            loopSource.volume = 1f;
            endSource.volume = 1f;
        }

        public void Play()
        {
            if (_isLaserActive)
            {
                return;
            }
            
            _isLaserActive = true;
            
            CancelTransition();
            
            startSource.Stop();
            loopSource.Stop();
            endSource.Stop();
            
            loopSource.volume = 0f;
            
            startSource.Play();

            _transitionRoutine = StartCoroutine(StartAttackLoop());
        }

        public void Stop()
        {
            if (!_isLaserActive)
            {
                return;
            }
            
            _isLaserActive = false;
            
            CancelTransition();
            
            _transitionRoutine = StartCoroutine(StopAttackLoop());
        }

        private IEnumerator StartAttackLoop()
        {
            float delay = Mathf.Max(0f, startClip.length - transitionDuration);
            
            yield return new WaitForSeconds(delay);

            if (!_isLaserActive)
            {
                _transitionRoutine = null;
                
                yield break;
            }
            
            loopSource.Play();
            
            float startValue = startSource.volume;
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                
                float progress = Mathf.Clamp01(elapsed / transitionDuration);
                startSource.volume = Mathf.Lerp(startValue, 0f, progress);
                
                loopSource.volume = Mathf.Lerp(0f, 1f, progress);
                
                yield return null;
            }
            
            startSource.Stop();
            startSource.volume = 1f;
            loopSource.volume = 1f;

            _transitionRoutine = null;
        }

        private IEnumerator StopAttackLoop()
        {
            float startVolume = startSource.isPlaying ? startSource.volume : 0f;
            float loopVolume = loopSource.isPlaying ? loopSource.volume : 0f;
            
            endSource.Stop();
            endSource.volume = 0f;
            endSource.Play();

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                
                float progress = Mathf.Clamp01(elapsed / transitionDuration);
                
                startSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                loopSource.volume = Mathf.Lerp(loopVolume, 0f, progress);
                endSource.volume = Mathf.Lerp(0f, 1f, progress);
                
                yield return null;
            }
            
            startSource.Stop();
            loopSource.Stop();
            
            startSource.volume = 1f;
            loopSource.volume = 1f;
            endSource.volume = 1f;
            
            yield return new WaitForSeconds(audioCompletionDuration);
            
            _transitionRoutine = null;

            if (!_isLaserActive)
            {
                OnCompleted?.Invoke();
            }
        }

        private void CancelTransition()
        {
            if (null == _transitionRoutine)
            {
                return;
            }
            
            StopCoroutine(_transitionRoutine);
            _transitionRoutine = null;
        }
    }
}