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
        [SerializeField] private TextMeshProUGUI _healthText = null;

        private Dictionary<float, string> _characterLinesByHealth;
        private string _lastLineWritten;
        private bool _isWriting;
        private List<float> _completedValues;

        private void Awake()
        {
            CoeusPlayerController.OnHealthChanged += OnPlayerHealthChanged;
            _completedValues = new List<float>();
            _characterLinesByHealth = new Dictionary<float, string>()
            {
                { 90, "What are these voices?"},
                { 70, "They are talking to me"},
                { 50, "What is this pain I feel?" },
                { 30, "Moment by moment, I am changing" },
                { 10, "Farewell, cruel world!" }
                
            };

            OnPlayerHealthChanged(100f);
        }

        private void OnDestroy()
        {
            CoeusPlayerController.OnHealthChanged -= OnPlayerHealthChanged;
        }

        private void OnPlayerHealthChanged(float health)
        {
            _healthBar.value = health;
            var color = _healthColorGradient.Evaluate(health / 100f);
            _healthBarFill.color = color;

            foreach (var characterLineByHealth in _characterLinesByHealth)
            {
                if (characterLineByHealth.Key < health || _completedValues.Contains(characterLineByHealth.Key))
                {
                    continue;
                }
                
                _completedValues.Add(characterLineByHealth.Key);
                StartCoroutine(ShowLine(characterLineByHealth.Value));
                break;
            }
        }
        
        private IEnumerator ShowLine(string line)
        {
            if (_isWriting)
                yield break;
            
            if (_lastLineWritten == line)
                yield break;
            
            _lastLineWritten = line;
            
            _isWriting = true;
            _healthText.text = "";
            
            var wait = new WaitForSeconds(0.01f);
            foreach (var c in line)
            {
                _healthText.text += c;
                yield return wait;
            }

            yield return new WaitForSeconds(0.4f);

            _isWriting = false;
        }
    }
}
