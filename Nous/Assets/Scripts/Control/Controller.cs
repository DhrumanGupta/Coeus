using System;
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
        public Transform Transform { get; set; }

        protected static Controller CurrentController;

        protected float InputX;

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

        #endregion

        private float _raycastDistance;

        protected bool IsBeingControlled { get; set; }
        public abstract float Damage { get; set; }

        #region Unity Events

        protected virtual void Awake()
        {
            this.Camera = Camera.main;
            this.Rigidbody = GetComponent<Rigidbody2D>();
            this.SpriteRenderer = GetComponent<SpriteRenderer>();
            this.Animator = GetComponent<Animator>();
            this.Transform = transform;

            this._animatorRunId = Animator.StringToHash("isRunning");
            this._animatorIdleId = Animator.StringToHash("isIdle");
            this._animatorJumpId = Animator.StringToHash("jump");

            this._raycastDistance = (this.SpriteRenderer.bounds.size.x / 2f) + 0.1f;
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
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

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
                this.InputX = 0;
                this.IsBeingControlled = false;
                return;
            }

            if (toControl == this)
            {
                return;
            }

            toControl.IsBeingControlled = true;
            CurrentController = toControl;

            this.InputX = 0;
            this.IsBeingControlled = false;
        }

        private void FlipSpriteBasedOnDirection()
        {
            this.SpriteRenderer.flipX = this.InputX < 0;
        }

        private void UpdateAnimator()
        {
            bool isPlayerMoving = InputX == 0;
            this.Animator.SetBool(_animatorIdleId, isPlayerMoving);
            if (isPlayerMoving)
            {
                return;
            }

            this.Animator.SetBool(_animatorRunId, Mathf.Abs(InputX) > 0);
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
            for (int i = -1; i < 2; i++)
            {
                var offset = new Vector2(0, i * (_raycastDistance - 0.09f));

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