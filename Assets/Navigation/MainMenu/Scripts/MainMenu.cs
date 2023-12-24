#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button exitButton;

    [SerializeField] private Button otherSceneButton;

    [SerializeField] private int otherSceneIndex;


    void OnEnable()
    {
        playButton.onClick.AddListener(OnPlay);
        optionsButton.onClick.AddListener(OnOptions);
        exitButton.onClick.AddListener(OnExit);
        otherSceneButton.onClick.AddListener(OnLoadOtherScene);
    }

    void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlay);
        optionsButton.onClick.RemoveListener(OnOptions);
        exitButton.onClick.RemoveListener(OnExit);
    }

    void OnPlay()
    {
        Debug.Log("Play");
    }

    void OnOptions()
    {
        Debug.Log("Options");
    }

    void OnExit()
    {
        Debug.Log("Exit");
    }

    void OnLoadOtherScene()
    {
        LoadingScreen.Instance.LoadScene(otherSceneIndex);
    }
}