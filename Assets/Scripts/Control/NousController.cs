using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    public class NousController : Controller
    {
        public static readonly string Name = "Nous";
        
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
            this._health -= CurrentController.Damage * Time.deltaTime;
            if (this._health <= 0)
            {
                CurrentController.ChangeControl(null);
            }
        }
    }
}