#region

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image lifeBarImage;
    [SerializeField] private TextMeshPro lifeBarText;
    [SerializeField] private float lifeAnimationDuration = 0.25f;

    public void SetNormalizedValue(float newValue)
    {
        lifeBarImage.DOFillAmount(newValue, lifeAnimationDuration);
    }

    public void SetText(float newText)
    {
        lifeBarText.text = "+" + newText.ToString();
    }
}