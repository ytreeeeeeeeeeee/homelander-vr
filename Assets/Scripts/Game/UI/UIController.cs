using System;
using System.Collections;
using System.Collections.Generic;
using Game.NPC;
using UnityEngine;
using UnityEngine.Video;

namespace Game.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private EyeCanvas eyeCanvasPrefab;
        [SerializeField] private VideoPlayer editVideoPlayer;
        [SerializeField] private float fadeInTime = 1f;
        [SerializeField] private AudioSource videoAudioSource;
        
        public static UIController Instance;
        
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
            GameController.Instance.OnGameWin += PlayEdit;
            editVideoPlayer.loopPointReached += OnVideoFinished;
            NpcManager.Instance.OnNpcKilled += OnNpcKilled;
            
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
            NpcManager.Instance.OnNpcKilled -= OnNpcKilled;
        }

        private void PlayEdit()
        {
            AudioListener.pause = true;
            
            DisableLaserOverlay();
            
            _eyeCanvases.ForEach(
                eyeCanvas => 
                {
                    eyeCanvas.editSurface.gameObject.SetActive(true);
                    eyeCanvas.npcCounter.gameObject.SetActive(false);
                }
            );
            editVideoPlayer.Play();
        }

        public void DrawLaserOverlay()
        {
            if (editVideoPlayer.isPlaying)
            {
                return;
            }
            
            _eyeCanvases.ForEach(eyeCanvas => eyeCanvas.laserOverlay.gameObject.SetActive(true));
        }

        public void DisableLaserOverlay()
        {
            _eyeCanvases.ForEach(eyeCanvas => eyeCanvas.laserOverlay.gameObject.SetActive(false));
        }

        private void UpdateNpcCounter()
        {
            _eyeCanvases.ForEach(eyeCanvas => eyeCanvas.npcCounter.text = $"{NpcManager.Instance.KilledNpcCount}/{NpcManager.Instance.NpcCount}");
        }

        private void OnCameraCreated(SkyworthEye _, Camera cam)
        {
            EyeCanvas eyeCanvas = Instantiate(eyeCanvasPrefab);
            eyeCanvas.Canvas.worldCamera = cam;
            
            _eyeCanvases.Add(eyeCanvas);
            
            UpdateNpcCounter();
        }

        private void OnNpcKilled(Npc _)
        {
            UpdateNpcCounter();
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
