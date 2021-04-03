using System;
using System.Collections;
using UnityEngine;
using UnityEngineInternal;

namespace Game.Control
{
    public class ErebusController : Controller
    {
        public static readonly string Name = "Erebus";
        [field: SerializeField] public override float Damage { get; set; }
        
        [SerializeField] private float _abilityCooldown = 2f;
        private float _nextTimeToUseAbility = 0f;

        private Collider2D _collider;

        private new void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider2D>();
        }

        private new void Update()
        {
            if (!this.IsBeingControlled)
            {
                return;
            }
            
            base.Update();
            GetInput();
        }

        private new void FixedUpdate()
        {
            base.FixedUpdate();
        }
        
        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.E) && _nextTimeToUseAbility <= Time.time)
            {
                StartCoroutine(UseAbility());
            }
        }
        
        private IEnumerator UseAbility()
        {
            var teleportPos = this.Camera.ScreenToWorldPoint(Input.mousePosition);
            teleportPos.z = 0;
            
            // Check if there is something around the area
            if (Physics2D.OverlapCircle(teleportPos, 1.2f))
            {
                // TODO:
                // Play Error sound
                yield return null;
            }
            
            ChangeCharacterVisibility(false);

            yield return new WaitForSeconds(0.2f);

            ChangeCharacterVisibility(true);

            _nextTimeToUseAbility = Time.time + _abilityCooldown;
            this.Transform.position = teleportPos;
        }

        private void ChangeCharacterVisibility(bool isVisible)
        {
            this.Rigidbody.isKinematic = !isVisible;
            _collider.isTrigger = !isVisible;
            this.SpriteRenderer.enabled = isVisible;
        }
    }
}