using System;
using System.Collections;
using System.Collections.Generic;
using Game.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HealthPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        
        [Header("Health Bar")]
        [SerializeField] private Slider _healthBar = null;
        [SerializeField] private Image _healthBarFill = null;
        [SerializeField] private Gradient _healthColorGradient = null;

        private void Awake()
        {
            CoeusController.OnHealthChanged += OnPlayerHealthChanged;
            OnPlayerHealthChanged(100f);
        }

        private void OnDestroy()
        {
            CoeusController.OnHealthChanged -= OnPlayerHealthChanged;
        }

        private void OnPlayerHealthChanged(float health)
        {
            _healthBar.value = health;
            var color = _healthColorGradient.Evaluate(health / 100f);
            _healthBarFill.color = color;
        }
    }
}
