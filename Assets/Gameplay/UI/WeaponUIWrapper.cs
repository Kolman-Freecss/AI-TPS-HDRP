#region

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Gameplay.GameplayObjects.UI
{
    public class WeaponUIWrapper : MonoBehaviour
    {
        public Image weaponSprite;
        public TextMeshProUGUI index;

        [HideInInspector] public int weaponIndex;
    }
}