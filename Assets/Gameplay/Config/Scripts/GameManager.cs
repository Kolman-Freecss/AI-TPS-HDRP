#region

using System;
using System.Collections;
using Gameplay.GameplayObjects.Interactables._derivatives;
using Systems.NarrationSystem.Dialogue.Components;
using Systems.NarrationSystem.Dialogue.Data;
using Systems.NarrationSystem.Flow;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

#endregion

public class GameManager : MonoBehaviour
{
    [SerializeField] public bool debug = false;

    public enum RoundTypes
    {
        None,
        InGame_Init,
        InGame_Second
    }

    public float timeToFinishGame = 5f;

    [SerializeField] private Dialogue m_GameFinalDialogue;

    #region Member properties

    public static GameManager Instance { get; private set; }

    public event Action OnGameStarted;
    public bool IsGameStarted { get; private set; }

    [HideInInspector] public PlayerController m_player;

    [HideInInspector] public bool m_GameWon;

    public bool gamePaused;

    private Action m_OnLastDialogueFinish;
    private bool lastFinalDialogueInit = false;

    private delegate void FinishGameDelegate();

    private FinishGameDelegate m_OnFinishGame;

    [HideInInspector] public int m_CurrentScore = 0;

    #endregion

    #region InitData

    [Inject]
    public void Construct()
    {
        Debug.Log("GameManager Construct");
    }

    private void Awake()
    {
        ManageSingleton();
    }

    private void ManageSingleton()
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        if (debug) SetPlayerDebug();
    }

    public void SetPlayerDebug()
    {
        m_player = FindObjectOfType<PlayerController>();
    }

    #endregion

    #region Logic

    public void InitGame(SceneTransitionHandler.SceneStates sceneState)
    {
        SceneTransitionHandler.Instance.LoadScene(sceneState, true);
        m_CurrentScore = 0;
        StartGame(sceneState);
    }

    public void StartGame(SceneTransitionHandler.SceneStates sceneState)
    {
        SoundManager.BackgroundMusic backgroundMusic = SoundManager.BackgroundMusic.InGameInit;
        switch (sceneState)
        {
            case SceneTransitionHandler.SceneStates.InGameInit:
                backgroundMusic = SoundManager.BackgroundMusic.InGameInit;
                break;
            case SceneTransitionHandler.SceneStates.InGameSecond:
                backgroundMusic = SoundManager.BackgroundMusic.InGameSecond;
                break;
        }

        SoundManager.Instance.StartBackgroundMusic(backgroundMusic);
        IsGameStarted = true;
        OnGameStarted?.Invoke();
    }

    public void RestartGame()
    {
        InitGame(SceneTransitionHandler.SceneStates.InGameInit);
    }

    public void PauseGameEvent(bool mIsPaused)
    {
        gamePaused = mIsPaused;
        DisableOrEnablePlayer(!mIsPaused);
    }

    /// <summary>
    /// Disable or enable player movement and interaction
    /// </summary>
    /// <param name="disable">
    /// false = disable
    /// true = enable
    /// </param>
    public void DisableOrEnablePlayer(bool disable)
    {
        m_player.enabled = disable;
    }

    public void OnPlayerDeath(RoundTypes roundType)
    {
        EndGame(false);
    }

    public void OnPlayerEndRound(RoundTypes roundType, PortalInteractable portalInteractable)
    {
        switch (roundType)
        {
            case RoundTypes.InGame_Init:
                if (portalInteractable.NextRoundType == RoundTypes.InGame_Second)
                {
                    SceneTransitionHandler.Instance.LoadScene(SceneTransitionHandler.SceneStates.InGameSecond, true);
                    SoundManager.Instance.StartBackgroundMusic(SoundManager.BackgroundMusic.InGameSecond);
                }
                else
                {
                    EndGame(true);
                }

                break;
            case RoundTypes.InGame_Second:
                EndGame(true);
                break;
        }
    }

    public void EndGame(bool isWin)
    {
        if (m_GameFinalDialogue != null && isWin)
            try
            {
                m_OnFinishGame = new FinishGameDelegate(FinishGame);
                m_OnLastDialogueFinish += m_OnFinishGame.Invoke;
                InitFinalNarration();
            }
            catch (Exception e)
            {
                Debug.LogError("GameManager: Error while initializing narration round: " + e);
            }
        else
            FinishGame();

        void FinishGame()
        {
            try
            {
                if (m_GameFinalDialogue != null && isWin) m_OnLastDialogueFinish -= m_OnFinishGame.Invoke;

                m_GameWon = isWin;
                SoundManager.Instance.StartBackgroundMusic(isWin
                    ? SoundManager.BackgroundMusic.WinGame
                    : SoundManager.BackgroundMusic.LostGame);
                StartCoroutine(OnEndGame());
            }
            catch (Exception e)
            {
                Debug.LogError("GameManager: Error while finishing game: " + e);
            }
        }

        IEnumerator OnEndGame()
        {
            yield return new WaitForSeconds(timeToFinishGame);
            SceneTransitionHandler.Instance.LoadScene(SceneTransitionHandler.SceneStates.EndGame, true);
            IsGameStarted = false;
        }
    }

    public void FinishGameGoMenu()
    {
        SoundManager.Instance.StartBackgroundMusic(SoundManager.BackgroundMusic.Intro);
        SceneTransitionHandler.Instance.LoadScene(SceneTransitionHandler.SceneStates.Home, true);
    }

    private void InitFinalNarration()
    {
        try
        {
            FlowState fs0Aux = FlowListener.Instance.Entries[0].m_State;
            FlowState fs1Aux = FlowListener.Instance.Entries[1].m_State;
            FlowListener.Instance.Entries = new[]
            {
                new FlowListenerEntry { m_State = fs0Aux, m_Event = new UnityEvent() },
                new FlowListenerEntry { m_State = fs1Aux, m_Event = new UnityEvent() }
            };
            FlowListener.Instance.Entries[0].m_Event.AddListener(DialogueStarted);
            FlowListener.Instance.Entries[1].m_Event.AddListener(DialogueEnded);
            DialogueInstigator.Instance.FlowChannel.OnFlowStateChanged += OnFlowStateChanged;
            DialogueInstigator.Instance.DialogueChannel.RaiseRequestDialogue(m_GameFinalDialogue);
        }
        catch (Exception e)
        {
            Debug.LogError("GameManager: Error while initializing narration round: " + e);
        }
    }

    private void OnFlowStateChanged(FlowState state)
    {
    }

    public void DialogueStarted()
    {
        if (Instance.m_player == null)
        {
            Instance.m_player = FindObjectOfType<PlayerController>();
            if (Instance.m_player == null)
            {
                Debug.LogError("RoundManager: No player found");
                return;
            }
        }

        lastFinalDialogueInit = true;

        PauseGameEvent(true);
    }

    public void DialogueEnded()
    {
        if (Instance.m_player == null)
        {
            Instance.m_player = FindObjectOfType<PlayerController>();
            if (Instance.m_player == null)
            {
                Debug.LogError("RoundManager: No player found");
                return;
            }
        }

        if (lastFinalDialogueInit)
        {
            m_OnLastDialogueFinish?.Invoke();
            DialogueInstigator.Instance.FlowChannel.OnFlowStateChanged -= OnFlowStateChanged;
        }

        PauseGameEvent(false);
    }

    #endregion
}