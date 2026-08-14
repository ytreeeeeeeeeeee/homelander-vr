using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.UI
{
    public class HomelanderEditVideoController : MonoBehaviour
    {
        [SerializeField] private EyeCanvas editCanvasPrefab;
        [SerializeField] private VideoPlayer editVideoPlayer;
        [SerializeField] private float fadeInTime = 1f;
        [SerializeField] private AudioSource videoAudioSource;
        
        public static HomelanderEditVideoController Instance;
        
        public event Action OnOneSecondToEndEdit;

        private readonly List<EyeCanvas> _eyeCanvases = new();
        private bool _isEditEndEventGenerated = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SkyworthVrRig.EyeCameraCreated += OnCameraCreated;
            GameController.Instance.OnGameWin +=  PlayEdit;
            editVideoPlayer.loopPointReached += OnVideoFinished;

            videoAudioSource.ignoreListenerPause =  true;
        }

        private void Update()
        {
            ControlEditFading();
        }

        private void OnDisable()
        {
            SkyworthVrRig.EyeCameraCreated -= OnCameraCreated;
            GameController.Instance.OnGameWin -= PlayEdit;
            editVideoPlayer.loopPointReached -= OnVideoFinished;
        }

        private void PlayEdit()
        {
            AudioListener.pause = true;
            
            _eyeCanvases.ForEach(editCanvas => editCanvas.gameObject.SetActive(true));
            editVideoPlayer.Play();
        }

        private void OnCameraCreated(SkyworthEye eye, Camera cam)
        {
            EyeCanvas eyeEditCanvas = Instantiate(editCanvasPrefab, cam.transform);
            eyeEditCanvas.targetCamera = cam;
            eyeEditCanvas.gameObject.SetActive(false);
            
            _eyeCanvases.Add(eyeEditCanvas);
        }
        
        private void ControlEditFading()
        {
            if (!editVideoPlayer.isPlaying)
            {
                return;
            }

            double timeLeft = editVideoPlayer.length - editVideoPlayer.time;

            if (1.0 >= timeLeft)
            {
                if (!_isEditEndEventGenerated)
                {
                    OnOneSecondToEndEdit?.Invoke();
                    _isEditEndEventGenerated = true;
                    StartCoroutine(FadeOutVideoVolume(fadeInTime));
                }
            } 
            else if (0.0 >= timeLeft)
            {
                _isEditEndEventGenerated = false;
            }
        }
        
        private IEnumerator FadeOutVideoVolume(float duration)
        {
            float startVolume = editVideoPlayer.GetDirectAudioVolume(0);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                float volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                
                editVideoPlayer.SetDirectAudioVolume(0, volume);
                
                yield return null;
            }
            
            editVideoPlayer.SetDirectAudioVolume(0, 0f);
        }

        private void OnVideoFinished(VideoPlayer _)
        {
            AudioListener.pause = false;
        }
    }
}
