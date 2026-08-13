using System;
using System.Collections;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public const string MenuSceneName = "Menu";
    public const string MainSceneName = "Main";

    public static SceneLoader Instance;
        
    [HideInInspector] public bool isLoading = false;

    private void Awake()
    {
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            return;
        }
            
        string currentSceneName = SceneManager.GetActiveScene().name;
            
        if (currentSceneName == sceneName)
        {
            throw new Exception("Trying to load current scene");
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
            
        bool waitFading = true;
        Fader.Instance.FadeIn(() => waitFading = false);

        while (waitFading)
        {
            yield return null;
        }
            
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (0.9f > op.progress)
        {
            yield return null;
        }
            
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }
            
        waitFading = true;
        Fader.Instance.FadeOut(() => waitFading = false);

        while (waitFading)
        {
            yield return null;
        }
            
        isLoading = false;
    }
}