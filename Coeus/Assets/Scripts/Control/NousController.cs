using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    public class NousController : Controller
    {
        public static readonly string Name = "Nous";
        public static event Action<float> OnHealthChanged;
        
        [field: SerializeField] public override float Damage { get; set; }

        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;

        private float _health;

        private new void Awake()
        {
            base.Awake();
            this._health = _maxHealth;
            this.IsBeingControlled = true;
            CurrentController = this;
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
            if (CurrentController == null)
            {
                return;
            }
            
            this._health -= CurrentController.Damage * Time.deltaTime;
            OnHealthChanged?.Invoke(this._health);
            if (this._health <= 0)
            {
                CurrentController.ChangeControl(null);
            }
        }
    }
}