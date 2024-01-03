#region

using Entity.Scripts;
using Gameplay.Config.Scripts;
using Gameplay.GameplayObjects.Interactables._derivatives;
using UnityEngine;
using UnityEngine.UI;

// using AnimationEvent = Gameplay.Events.AnimationEvent;

#endregion

namespace Gameplay.GameplayObjects.Player.Script
{
    /// <summary>
    /// Player stats class.
    /// @author: Kolman-Freecss
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private AudioClip m_OnMeatingClip;
        [SerializeField] private Image ParentActionProgressBar;

        private EntityLife m_EntityLife;
        private EntityAnimation m_EntityAnimation;

        private AudioSource m_AudioSource;

        // AnimationEvent animationEvent;
        private MeatInteractable currentMeatInteraction;
        private Image progressBar;

        private void Awake()
        {
            m_EntityLife = GetComponent<EntityLife>();
            m_EntityAnimation = GetComponent<EntityAnimation>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            ParentActionProgressBar.gameObject.SetActive(false);
            progressBar = ParentActionProgressBar.gameObject.transform.GetChild(0).GetComponent<Image>();
            m_EntityLife.onDeath.AddListener(OnDeath);
            // animationEvent = GetComponentInChildren<AnimationEvent>();
            // animationEvent.OnAnimationFinish += HandleAnimationFinished;
        }

        private void OnDeath()
        {
            RoundManager.Instance.OnPlayerDeath();
        }

        // private void HandleAnimationFinished(int handleAnimationId)
        // {
        //     if (handleAnimationId == (int)PlayerAnimationIds.Eat)
        //     {
        //         m_EntityLife.Heal(currentMeatInteraction.meatRecovery);
        //         Destroy(currentMeatInteraction.gameObject);
        //         GameManager.Instance.DisableOrEnablePlayer(true);
        //     }
        // }

        // public void OnMeatCollected(object obj)
        // {
        //     try
        //     {
        //         MeatInteractable meat = obj as MeatInteractable;
        //         currentMeatInteraction = meat;
        //         GameManager.Instance.DisableOrEnablePlayer(false);
        //         if (m_EntityAnimation != null)
        //         {
        //             StartCoroutine(PlayAnimationWithProgressBar("ParasyteEat", m_OnMeatingClip));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError(e);
        //     }
        // }

        /// <summary>
        /// Plays an animation with a progress bar.
        /// </summary>
        /// <param name="animationTrigger"></param>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        //     IEnumerator PlayAnimationWithProgressBar(string animationTrigger, AudioClip audioClip)
        //     {
        //         ParentActionProgressBar.gameObject.SetActive(true);
        //         if (m_AudioSource != null && audioClip != null)
        //         {
        //             m_AudioSource.PlayOneShot(audioClip);
        //         }
        //
        //         m_EntityAnimation.GetAnimator().SetTrigger(animationTrigger);
        //
        //         progressBar.DOFillAmount(0f, 0f);
        //
        //         float elapsedTime = 0f;
        //
        //         while (elapsedTime < 2f)
        //         {
        //             progressBar.fillAmount = Mathf.Clamp01(elapsedTime / 2f);
        //
        //             yield return null;
        //
        //             elapsedTime += Time.deltaTime;
        //         }
        //
        //         progressBar.fillAmount = 1f;
        //         ParentActionProgressBar.gameObject.SetActive(false);
        //     }
        //
        //     private void SetNormalizedActionProgress(float normalizedProgress)
        //     {
        //         ParentActionProgressBar.fillAmount = normalizedProgress;
        //     }
        //
        //     private void OnDisable()
        //     {
        //         animationEvent.OnAnimationFinish -= HandleAnimationFinished;
        //     }
        // }
    }
}