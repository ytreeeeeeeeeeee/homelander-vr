using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game.UI
{
    public class HomelanderEditVideoController : MonoBehaviour
    {
        [SerializeField] private HomelanderEditCanvas editCanvasPrefab;
        [SerializeField] private VideoPlayer editVideoPlayer;
        [SerializeField] private float fadeInTime = 1f;
        
        public static HomelanderEditVideoController Instance;
        
        public event Action OnOneSecondToEndEdit;

        private readonly List<HomelanderEditCanvas> _eyeEditCanvases = new();
        private bool _isEditEndEventGenerated = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SkyworthVrRig.EyeCameraCreated += OnCameraCreated;
            GameController.Instance.OnGameWin +=  PlayEdit;
        }

        private void Update()
        {
            ControlEditFading();
        }

        private void OnDisable()
        {
            SkyworthVrRig.EyeCameraCreated -= OnCameraCreated;
            GameController.Instance.OnGameWin -= PlayEdit;
        }

        private void PlayEdit()
        {
            _eyeEditCanvases.ForEach(editCanvas => editCanvas.gameObject.SetActive(true));
            editVideoPlayer.Play();
        }

        private void OnCameraCreated(SkyworthEye eye, Camera cam)
        {
            HomelanderEditCanvas eyeEditCanvas = Instantiate(editCanvasPrefab, cam.transform);
            eyeEditCanvas.targetCamera = cam;
            eyeEditCanvas.gameObject.SetActive(false);
            
            _eyeEditCanvases.Add(eyeEditCanvas);
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
    }
}
