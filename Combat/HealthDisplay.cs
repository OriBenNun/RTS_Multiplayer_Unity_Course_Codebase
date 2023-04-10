using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class HealthDisplay : NetworkBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private float healthBarDisplayTimeAfterDamage = 3.5f;

        private void Awake()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void OnMouseEnter()
        {
            if (!hasAuthority) { return; }
            
            DisplayHealthBar();
        }
        
        private void OnMouseExit()
        {
            if (!hasAuthority) { return; }
            
            HideHealthBar();
        }
        
        private void DisplayHealthBar() => healthBarParent.SetActive(true);
        private void HideHealthBar() => healthBarParent.SetActive(false);

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;

            if (currentHealth == maxHealth) { return; }
            
            DisplayHealthBar();

            Invoke(nameof(HideHealthBar), healthBarDisplayTimeAfterDamage);
        }
    }
}
