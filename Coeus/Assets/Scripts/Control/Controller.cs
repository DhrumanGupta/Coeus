using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Game.Control
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Controller : MonoBehaviour
    {
        #region Protected Variables

        protected Rigidbody2D Rigidbody;
        protected SpriteRenderer SpriteRenderer;
        protected Animator Animator;
        protected Camera Camera;

        protected float InputX;

        protected static Controller CurrentController;
        protected bool IsBeingControlled { get; set; }

        [Header("Movement")] [SerializeField] protected float MoveSpeed;
        [SerializeField] protected float JumpForce;
        [SerializeField] protected float FrictionForce;
        [SerializeField] protected LayerMask GroundLayerMask;
        [SerializeField] protected Transform GroundCheck;

        protected bool IsJumping { get; set; }
        protected bool IsGrounded { get; set; }

        #endregion

        #region Private Variables

        private int _animatorRunId;
        private int _animatorIdleId;
        private int _animatorJumpId;
        private int _animatorControlId;

        private float _raycastDistance;

        #endregion

        public abstract float Damage { get; set; }
        public static List<Controller> Controllers { get; private set; }
        public Transform Transform { get; private set; }

        #region Unity Events

        protected virtual void Awake()
        {
            if (Controllers == null)
            {
                Controllers = new List<Controller>();
            }

            if (!Controllers.Contains(this))
            {
                Controllers.Add(this);
            }

            this.Camera = Camera.main;
            this.Rigidbody = GetComponent<Rigidbody2D>();
            this.SpriteRenderer = GetComponent<SpriteRenderer>();
            this.Animator = GetComponent<Animator>();
            this.Transform = transform;

            _animatorRunId = Animator.StringToHash("isRunning");
            _animatorIdleId = Animator.StringToHash("isIdle");
            _animatorJumpId = Animator.StringToHash("jump");
            _animatorControlId = Animator.StringToHash("isBeingControlled");

            _raycastDistance = (this.SpriteRenderer.bounds.size.x / 2f) + 0.1f;
        }

        protected virtual void Update()
        {
            GetInput();
            CheckControl();
            FlipSpriteBasedOnDirection();
            UpdateAnimator();
        }

        protected virtual void FixedUpdate()
        {
            CheckIfGrounded();
            Move();
            RaycastForPlayers();
        }

        #endregion

        private void GetInput()
        {
            this.InputX = Input.GetAxis("Horizontal");
            this.IsJumping = Input.GetButtonDown("Jump") && this.IsGrounded;
        }

        private void CheckControl()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var pos = this.Camera.ScreenToWorldPoint(Input.mousePosition);
            var colliders = Physics2D.OverlapCircleAll(pos, 0.1f);

            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent(out Controller controller))
                {
                    continue;
                }

                ChangeControl(controller);
            }
        }

        public void ChangeControl(Controller toControl)
        {
            if (toControl == null)
            {
                this.RemoveControl();
                return;
            }

            this.RemoveControl();

            toControl.Animator.SetBool(_animatorControlId, true);
            toControl.IsBeingControlled = true;
            CurrentController = toControl;
        }

        private void RemoveControl()
        {
            this.InputX = 0;
            this.Animator.SetBool(_animatorControlId, false);
            this.IsBeingControlled = false;
        }

        private void FlipSpriteBasedOnDirection()
        {
            if (InputX == 0) return;
            this.SpriteRenderer.flipX = this.InputX > 0;
        }

        private void UpdateAnimator()
        {
            bool isPlayerMoving = InputX == 0;
            this.Animator.SetBool(_animatorIdleId, isPlayerMoving);
            if (isPlayerMoving)
            {
                return;
            }

            this.Animator.SetBool(_animatorRunId, Mathf.Abs(InputX) > 0.1f);
        }

        private void CheckIfGrounded()
        {
            this.IsGrounded = Physics2D.OverlapCircle(this.GroundCheck.position, 0.2f, this.GroundLayerMask);
        }

        private void Move()
        {
            var currentVelocity = this.Rigidbody.velocity;

            currentVelocity = new Vector2(this.InputX * MoveSpeed, currentVelocity.y);
            this.Rigidbody.velocity = currentVelocity;

            // Apply some friction
            Vector2 friction = currentVelocity.normalized * FrictionForce;
            friction.x *= -1;
            friction.y = 0;
            this.Rigidbody.AddForce(friction, ForceMode2D.Force);

            if (!this.IsJumping)
            {
                return;
            }

            this.Rigidbody.AddForce(new Vector2(0, this.JumpForce), ForceMode2D.Impulse);
            this.IsJumping = false;
            this.Animator.SetTrigger(_animatorJumpId);
        }

        private void RaycastForPlayers()
        {
            if (this.Rigidbody.velocity.x == 0)
            {
                return;
            }
            
            var dir = this.Rigidbody.velocity.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);

            var raycastYOffset = this.SpriteRenderer.bounds.size.y / 2f - 0.01f;
            
            for (int i = -1; i < 2; i++)
            {
                
                var offset = new Vector2(0, i * raycastYOffset);

                var raycastHits = new RaycastHit2D[2];
                var size = Physics2D.RaycastNonAlloc((Vector2) this.Transform.position + offset, dir, raycastHits,
                    _raycastDistance);

                if (size <= 0)
                {
                    continue;
                }

                foreach (var hit in raycastHits)
                {
                    if (!hit || !hit.transform.TryGetComponent(out Controller controller) || controller == this)
                    {
                        continue;
                    }

                    if (controller == this)
                    {
                        continue;
                    }

                    this.Rigidbody.velocity = new Vector2(0, this.Rigidbody.velocity.y);
                }
            }
        }
    }
}