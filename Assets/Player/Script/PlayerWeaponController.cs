#region

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Player.Script
{
    public class PlayerWeaponController : EntityWeapons
    {
        [Header("Input")] [SerializeField] private InputActionReference reloadInput;

        [Header("Canvas")] [SerializeField] private TextMeshProUGUI currentAmmoText;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            reloadInput.action.Enable();
            reloadInput.action.performed += ReloadInputOnPerformed;
        }

        private void Update()
        {
            if (HasCurrentWeapon())
            {
                currentAmmoText.text = GetCurrentWeapon().GetAmmoInfo();
            }
        }

        private void ReloadInputOnPerformed(InputAction.CallbackContext obj)
        {
            Reload();
        }

        private void OnDisable()
        {
            reloadInput.action.performed -= ReloadInputOnPerformed;
            reloadInput.action.Disable();
        }
    }
}