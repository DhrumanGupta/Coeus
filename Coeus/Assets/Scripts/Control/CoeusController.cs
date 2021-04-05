using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    public class CoeusController : PlayerController
    {
        public static readonly string Name = "Coeus";
        public static event Action<float> OnHealthChanged;
        
        [field: SerializeField] public override float Damage { get; set; }

        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;

        private float _health;

        private new void Awake()
        {
            base.Awake();
            this._health = _maxHealth;
            ChangeControl(this);
        }

        private new void Update()
        {
            if (!this.IsBeingControlled)
            {
                ReduceHealth();
                return;
            }
            
            base.Update();
        }

        private new void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void ReduceHealth()
        {
            if (CurrentPlayerController == null)
            {
                return;
            }
            
            this._health -= CurrentPlayerController.Damage * Time.deltaTime;
            OnHealthChanged?.Invoke(this._health);
            if (this._health <= 0)
            {
                CurrentPlayerController.ChangeControl(null);
            }
        }
    }
}