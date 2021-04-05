using System;
using System.Collections.Generic;
using Game.Control;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Obstacles
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TimedPressurePlate : TimedEvent
    {
        [SerializeField] private UnityEvent _onPressurePlateDown = null;
        [SerializeField] private UnityEvent _onPressurePlateUp = null;

        private Vector3 _startPos;
        private Vector3 _endPos;

        [SerializeField] private float _pressDistance = 1f;
        [SerializeField] private float _timeBeforeUp = 5f;

        [SerializeField] private string _playerTag = null;
        private Dictionary<string, Color> _colorByTag;

        private bool _isPressed;

        private void Awake()
        {
            _startPos = transform.position;
            _endPos = _startPos - new Vector3(0, _pressDistance, 0);

            _colorByTag = new Dictionary<string, Color>()
            {
                {"Coeus", new Color32(255, 226, 184, 255)},
                {"Hermes", new Color32(238, 184, 255, 255)},
                {"Erebus", new Color32(106, 24, 108, 255)}
            };

            if (_playerTag == null || !_colorByTag.ContainsKey(_playerTag)) return;
            this.GetComponent<SpriteRenderer>().color = _colorByTag[_playerTag];
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!string.IsNullOrWhiteSpace(_playerTag) && !collision.collider.CompareTag(_playerTag)) return;

            if (collision.contacts.Length <= 0) return;

            var contact = collision.contacts[0];
            if (Vector3.Dot(contact.normal, Vector3.down) <= 0.5) return;

            _isPressed = collision.collider.TryGetComponent(out PlayerController _);

            if (!_isPressed) return;

            CancelTimer();
            transform.position = _endPos;
            _onPressurePlateDown?.Invoke();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!_isPressed) return;
            base.StartTimer(_timeBeforeUp, HandleTimerFinished);
            _isPressed = false;
        }

        private void HandleTimerFinished()
        {
            transform.position = _startPos;
            _onPressurePlateUp?.Invoke();
        }
    }
}