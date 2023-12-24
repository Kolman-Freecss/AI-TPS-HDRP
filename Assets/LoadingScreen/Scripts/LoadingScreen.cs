#region

using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }
    private CanvasGroup fader;
    private int oldScene = -1;
    [SerializeField] private int firstSceneToLoad;
    [SerializeField] private Transform rotatingElement;

    private void Awake()
    {
        Instance = this;
        fader = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        rotatingElement.DORotate(Vector3.forward * 360f, 2f, RotateMode.FastBeyond360).SetLoops(-1);
        LoadScene(firstSceneToLoad);
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneProcessCoroutine(sceneIndex));
    }

    IEnumerator LoadSceneProcessCoroutine(int scene)
    {
        {
            Tween fadeOut = fader.DOFade(0f, 5f);

            while (fadeOut.IsPlaying())
            {
                yield return null;
            }

            if (oldScene != -1)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(oldScene);
                while (!unloadOperation.isDone)
                {
                    yield return null;
                }
            }
        }
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            Tween fadeOut = fader.DOFade(0f, 5f);
            while (fadeOut.IsPlaying())
            {
                yield return null;
            }

            oldScene = scene;
        }
    }
}