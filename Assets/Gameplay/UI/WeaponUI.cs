#region

using System.Collections.Generic;
using ModestTree;
using Systems.WeaponSystem.Scripts;
using UnityEngine;

#endregion

namespace Gameplay.GameplayObjects.UI
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField] public WeaponUIWrapper weaponPrefab;

        public List<SerializableDictionaryEntry<WeaponType, WeaponUIWrapper>> weaponUIWrappers;

        private WeaponUIWrapper m_CurrentWeaponUIWrapper;

        public void SetWeaponSprite(WeaponType weaponType)
        {
            if (weaponUIWrappers.IsEmpty()) return;
            if (m_CurrentWeaponUIWrapper != null)
            {
                m_CurrentWeaponUIWrapper.weaponSprite.color = Color.black;
                m_CurrentWeaponUIWrapper.index.color = Color.black;
            }

            m_CurrentWeaponUIWrapper = weaponUIWrappers.Find(x => x.Key == weaponType)?.Value;
            if (m_CurrentWeaponUIWrapper != null)
            {
                m_CurrentWeaponUIWrapper.weaponSprite.color = Color.red;
                m_CurrentWeaponUIWrapper.index.color = Color.red;
            }
        }

        public void AddWeapon(Weapon weapon, int index)
        {
            WeaponUIWrapper weaponUIWrapper = Instantiate(weaponPrefab, transform);
            weaponUIWrapper.weaponSprite.sprite = weapon.WeaponSprite;
            weaponUIWrapper.index.text = (weaponUIWrappers.Count + 1).ToString();
            weaponUIWrapper.weaponIndex = index;
            weaponUIWrappers.Add(
                new SerializableDictionaryEntry<WeaponType, WeaponUIWrapper>(weapon.WeaponType, weaponUIWrapper));
        }

        public void RemoveWeapon(WeaponType weaponType)
        {
            WeaponUIWrapper weaponUIWrapper = weaponUIWrappers.Find(x => x.Key == weaponType).Value;
            if (weaponUIWrapper != null)
            {
                Destroy(weaponUIWrapper.gameObject);
                weaponUIWrappers.Remove(
                    new SerializableDictionaryEntry<WeaponType, WeaponUIWrapper>(weaponType, weaponUIWrapper));
            }
        }
    }
}