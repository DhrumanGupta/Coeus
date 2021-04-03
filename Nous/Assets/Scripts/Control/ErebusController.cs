using System.Collections;
using UnityEngine;

namespace Game.Control
{
    public class ErebusController : Controller
    {
        public static readonly string Name = "Erebus";
        [field: SerializeField] public override float Damage { get; set; }
        
        [Header("Ability")]
        [SerializeField] private GameObject _abilityParticleSystem = null;
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
            if (Physics2D.OverlapCircle(teleportPos, 1f))
            {
                // TODO:
                // Play Error sound
                yield break;
            }

            this.Rigidbody.bodyType = RigidbodyType2D.Static;
            _collider.isTrigger = true;
            StartCoroutine(Shake(0.3f));
            yield return new WaitForSeconds(0.3f);

            this.SpriteRenderer.enabled = false;
            Destroy(Instantiate(_abilityParticleSystem, this.Transform.position, Quaternion.identity), 2f);

            yield return new WaitForSeconds(0.2f);

            this.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _collider.isTrigger = false;
            this.SpriteRenderer.enabled = true;
            
            _nextTimeToUseAbility = Time.time + _abilityCooldown;
            this.Transform.position = teleportPos;
        }

        private IEnumerator Shake(float time)
        {
            float elapsed = 0f;
            while (elapsed < time)
            {
                float randomX = Random.Range(-0.15f, 0.15f);
                float randomY = Random.Range(-0.08f, 0.08f);
                this.Transform.position += new Vector3(randomX, randomY, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}