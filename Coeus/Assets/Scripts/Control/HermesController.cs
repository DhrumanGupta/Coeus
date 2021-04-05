using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Control
{
    public class HermesPlayerController : PlayerController
    {
        public static readonly string Name = "Hermes";
        [field: SerializeField] public override float Damage { get; set; }

        private new void Awake()
        {
            base.Awake();
        }

        private new void Update()
        {
            if (!this.IsBeingControlled)
            {
                return;
            }
            
            base.Update();
        }

        private new void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
