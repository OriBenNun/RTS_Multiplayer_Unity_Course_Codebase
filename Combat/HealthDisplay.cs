using System.Collections;
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

        private float _lastDisplayOnDamageTime;
        private bool _isMouseOver;
        private bool _isGameOver;
        
        private void Awake()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;
            
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
            
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }
        
        private void OnMouseEnter()
        {
            if (!hasAuthority) { return; }

            _isMouseOver = true;
            DisplayHealthBar();
        }
        
        private void OnMouseExit()
        {
            if (!hasAuthority) { return; }
            
            _isMouseOver = false;
            
            if (Time.time < _lastDisplayOnDamageTime + healthBarDisplayTimeAfterDamage) { return; }
            HideHealthBar();
        }
        
        private void DisplayHealthBar()
        {
            if (_isGameOver) { return; }
            
            healthBarParent.SetActive(true);
        }

        private void HideHealthBar() => healthBarParent.SetActive(false);

        private IEnumerator HideHealthBarAfterWaitTime()
        {
            while (Time.time < _lastDisplayOnDamageTime + healthBarDisplayTimeAfterDamage)
            {
                yield return new WaitForSeconds(healthBarDisplayTimeAfterDamage / 2);
            }

            if (_isMouseOver) { yield break; }
            
            HideHealthBar();
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;

            if (currentHealth == maxHealth) { return; }
            
            DisplayHealthBar();

            _lastDisplayOnDamageTime = Time.time;

            StartCoroutine(nameof(HideHealthBarAfterWaitTime));
        }

        [Client]
        private void ClientHandleGameOver(string winnerId)
        {
            HideHealthBar();
            _isGameOver = true;
        }
        
    }
}
