using UnityEngine;
using UnityEngine.Events;
using Moonvalk.Utilities.Algorithms;
using Moonvalk.Utilities;
using Moonvalk.Animation;
using Moonvalk.Accessory;
using System.Collections.Generic;
using Moonvalk;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Player1;

    protected Rigidbody _rigidbody;
    protected Collider _collider;

    public float RideHeight = 1f;
    public float GroundRayHeight = 1.2f;
    public float Tension = 2f;
    public float Dampening = 0.05f;
    public float JumpInitialForce = 20f;
    public float JumpHoldForce = 5f;
    public float JumpHoldDuration = 0.5f;

    protected bool _jumping = false;
    protected bool _grounded = false;
    protected bool _canJump = true;

    protected int _moveHorizontal = 0;
    protected int _moveVertical = 0;

    public float Friction = 5f;
    public float Acceleration = 2f;
    public float Deceleration = 1f;
    public float MoveSpeed = 2f;
    public float AimingMoveSpeed = 2f;
    protected Vector3 _currentMovementSpeed;
    protected bool _enabled = false;
    protected bool _isAiming = false;

    public Transform PlayerSprite;
    public GameObject SmokeEffect;
    public TextureAnimator Animator;

    protected Vector3 _frontFaceDirection;
    protected MicroTimer _frontFaceDirectionTimer;

    protected MicroTimer _animationTimer;

    public UnityEvent OnEnableEvent;
    public UnityEvent OnDisableEvent;

    public float AttackRange = 1f;
    public float AttackCooldown = 1f;
    protected MicroTimer _attackTimer;
    protected bool _isAttackAvailable = true;

    public GameObject Target;
    protected bool _isTargetActive = false;
    protected Targetable _currentTarget;
    protected SpriteRenderer _targetSprite;
    protected Spring _targetSpring;
    protected Tween _targetTween;
    protected float _targetScale = 0f;
    protected float _targetOpacity = 0f;

    public float CurrentMoveSpeed
    {
        get
        {
            return (this._isAiming ? this.AimingMoveSpeed : this.MoveSpeed);
        }
    }

    public bool IsAiming
    {
        get
        {
            return this._isAiming;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._collider = GetComponent<Collider>();
        this._currentMovementSpeed = new Vector3();
        this._frontFaceDirection = new Vector3();
        
        // Global.GetGameManager().AssignPlayer(this);
        PlayerController.Player1 = this;
    }

    private void Start()
    {
        this._frontFaceDirectionTimer = new MicroTimer(() => {
            this.updateFrontFaceDirection();
            this._frontFaceDirectionTimer.Start(0.2f);
        });
        this._frontFaceDirectionTimer.Start(0.2f);
        this._animationTimer = new MicroTimer();

        this.PlayerSprite.localScale = new Vector3(-1f, this.PlayerSprite.localScale.y, this.PlayerSprite.localScale.z);

        // Targeting animations.
        this._targetSprite = this.Target.GetComponent<SpriteRenderer>();
        this._targetTween = new Tween(() => ref this._targetOpacity);
        this._targetTween.Duration(0.3f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this._targetSprite.color = new Color(1f, 1f, 1f, this._targetOpacity);
        });
        this._targetSpring = new Spring(() => ref this._targetScale);
        this._targetSpring.Tension(50f).Dampening(4.5f).OnUpdate(() => {
            this.Target.transform.localScale = new Vector3(this._targetScale, this._targetScale, this._targetScale);
        });
        this._targetSprite.color = new Color(1f, 1f, 1f, 0f);

        // Attacking
        this._attackTimer = new MicroTimer(() => {
            this._isAttackAvailable = true;
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (!this._enabled)
        {
            return;
        }
        this.handleJumpInput();
        this.handleMoveInput();

        if (Input.GetMouseButton(1))
        {
            this._isAiming = true;
            this.Animator.ChangeSet("Melee");
            if (Input.GetMouseButtonDown(0) && this._isAttackAvailable)
            {
                this.Animator.Play("Attack");
                this._animationTimer.Start(this.Animator.GetAnimationDuration("Attack"));
                
                MicroTimer hitTimer = new MicroTimer(() => {
                    if (this._currentTarget != null)
                    {
                        this._currentTarget.Hit(1f);
                        Instantiate(this.SmokeEffect, this._currentTarget.transform.position, Quaternion.Euler(0f, 0f, 0f));
                    }
                });
                hitTimer.Start(0.5f);
                this._attackTimer.Start(this.AttackCooldown);
                this._isAttackAvailable = false;
            }
        }
        else
        {
            this.Animator.ChangeSet("Default");
            this._isAiming = false;
        }
    }

    private void FixedUpdate()
    {
        this.handleMovement();
        this.handleGroundedCheck();
        if (this._jumping)
        {
            this._rigidbody.AddForce(Vector3.up * this.JumpHoldForce);
        }
        this.handleSpring();

        if (this._enabled)
        {
            this.adjustPlayerOrientation();
            this.handleTargeting();
            this.animate();
        }
    }

    protected void handleTargeting()
    {
        this._currentTarget = null;
        if (this._isAiming)
        {
            float nearestDistance = float.MaxValue;
            int nearestIndex = 0;
            List<Targetable> allTargets = Targetable.GetAllTargets();
            for (int index = 0; index < allTargets.Count; index++)
            {
                if (!allTargets[index].IsActive)
                {
                    continue;
                }
                float distance = Vector3.Distance(this._frontFaceDirection, allTargets[index].AimPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = index;
                }
            }
            if (nearestDistance <= this.AttackRange)
            {
                this._currentTarget = allTargets[nearestIndex];
                this.Target.transform.position = allTargets[nearestIndex].AimPosition;
                if (!this._isTargetActive)
                {
                    this._isTargetActive = true;
                    this._targetTween.To(1f).Start();
                    this._targetSpring.To(0.6f);
                }
            }
            else
            {
                if (this._isTargetActive)
                {
                    this._isTargetActive = false;
                    this._targetTween.To(0f).Start();
                    this._targetSpring.To(0f);
                }
            }
        }
        else if (this._isTargetActive)
        {
            this._isTargetActive = false;
            this._targetTween.To(0f).Start();
            this._targetSpring.To(0f);
        }
    }

    protected void animate()
    {
        if (this._animationTimer.IsRunning)
        {
            return;
        }
        if (!this._grounded)
        {
            this.Animator.Play("Jump");
        }
        else
        {
            float collectiveVelocity = Mathf.Abs(this._rigidbody.velocity.x) + Mathf.Abs(this._rigidbody.velocity.z);
            if (collectiveVelocity > 0.2f)
            {
                float timeScale = Mathf.Clamp(collectiveVelocity * 0.4f, 0.2f, 0.8f);
                this.Animator.Play("Run", timeScale);
            }
            else
            {
                this.Animator.Play("Idle");
            }
        }
    }

    protected void adjustPlayerOrientation()
    {
        this.PlayerSprite.rotation = Quaternion.Euler(this.PlayerSprite.rotation.x, this.PlayerSprite.rotation.y, -this._rigidbody.velocity.x * 3f);
        this.PlayerSprite.localScale = new Vector3(-Mathf.Sign(this._rigidbody.velocity.x), this.PlayerSprite.localScale.y, this.PlayerSprite.localScale.z);
    }

    protected void handleGroundedCheck()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, this.GroundRayHeight);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.isTrigger)
                {
                    continue;
                }
                if (!this._grounded) {
                    Instantiate(this.SmokeEffect, hit.point, Quaternion.Euler(0f, 0f, 0f));
                }
                this._grounded = true;
                return;
            }
        }
        this._grounded = false;
    }

    private void handleSpring()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, this.RideHeight);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.isTrigger)
                {
                    continue;
                }
                Vector3 velocity = this._rigidbody.velocity;
                Vector3 rayDirection = transform.TransformDirection(Vector3.down);

                Vector3 otherVelocity = Vector3.zero;
                Rigidbody hitBody = hit.rigidbody;
                if (hitBody != null) {
                    otherVelocity = hitBody.velocity;
                }

                float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
                float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);

                float relativeVelocity = (rayDirectionVelocity - otherDirectionVelocity);
                float rideForce = MotionAlgorithms.SimpleHarmonicMotion(this.Tension, (hit.distance - this.RideHeight), this.Dampening, relativeVelocity);
                this.updatePlayerSprite(hit.distance - this.RideHeight, hit.point);

                if (!this._jumping)
                {
                    this._rigidbody.AddForce(rayDirection * rideForce);
                }

                if (hitBody != null) {
                    hitBody.AddForceAtPosition(rayDirection * -rideForce, hit.point);
                }
            }
        }
    }

    private void updatePlayerSprite(float distance_, Vector3 hitPoint_)
    {
        this.PlayerSprite.localScale = new Vector3(1f, 1f + Mathf.Clamp(distance_, -0.5f, 0.5f), 1f);
        float updatedYPosition = (this.PlayerSprite.position.y + ((hitPoint_.y - this.PlayerSprite.position.y) * 0.5f));
        this.PlayerSprite.position = new Vector3(this.PlayerSprite.position.x, updatedYPosition, this.PlayerSprite.position.z);
    }

    private void handleJumpInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            this._jumping = false;
        }
        else if (Input.GetKey(KeyCode.Space) && this._grounded && !this._jumping && this._canJump)
        {
            this._jumping = true;
            this._canJump = false;
            this._rigidbody.velocity.Set(this._rigidbody.velocity.x, 0f, this._rigidbody.velocity.z);
            this._rigidbody.AddForce(Vector3.up * this.JumpInitialForce);
            MicroTimer jumpTimer = new MicroTimer(() => {
                this._jumping = false;
                this._canJump = true;
            });
            jumpTimer.Start(this.JumpHoldDuration);
            Instantiate(this.SmokeEffect, this.PlayerSprite.transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
    }

    private void handleMoveInput()
    {
        this._moveHorizontal = Convert.ToInt(Input.GetKey(KeyCode.D)) - Convert.ToInt(Input.GetKey(KeyCode.A));
        this._moveVertical = Convert.ToInt(Input.GetKey(KeyCode.W)) - Convert.ToInt(Input.GetKey(KeyCode.S));
    }
    
    private void updateFrontFaceDirection()
    {
        if (this._moveHorizontal != 0)
        {
            this._frontFaceDirection.x = transform.position.x + (this._moveHorizontal * 0.5f);
        }
        this._frontFaceDirection.z = transform.position.z;
        this._frontFaceDirection.y = transform.position.y;
    }

    private void handleMovement()
    {
        Vector3 friction = new Vector3(-this._rigidbody.velocity.x * this.Friction, 0f, -this._rigidbody.velocity.z * this.Friction);
        this._rigidbody.AddForce(friction);

        this.handleAxisMovement(this._moveHorizontal, ref this._currentMovementSpeed.x);
        this.handleAxisMovement(this._moveVertical, ref this._currentMovementSpeed.z);

        this._rigidbody.AddForce(_currentMovementSpeed);
    }

    private void handleAxisMovement(int input_, ref float movementAxis_)
    {
        if (input_ != 0)
        {
            movementAxis_ += input_ * this.Acceleration;
        }
        else if (Mathf.Abs(movementAxis_) >= this.Deceleration)
        {
            movementAxis_ -= Mathf.Sign(movementAxis_) * this.Deceleration;
        }
        else
        {
            movementAxis_ = 0f;
        }
        movementAxis_ = Mathf.Clamp(movementAxis_, -this.CurrentMoveSpeed, this.CurrentMoveSpeed);
    }

    public void EnablePhysics(bool isEnabled_)
    {
        this._rigidbody.isKinematic = !isEnabled_;
        this._rigidbody.detectCollisions = isEnabled_;
        this._moveHorizontal = 0;
        this._moveVertical = 0;
    }

    public void EnableControl(bool isEnabled_)
    {
        if (this._enabled == isEnabled_)
        {
            return;
        }
        this._enabled = isEnabled_;
        
        if (this._enabled)
        {
            this.OnEnableEvent.Invoke();
        }
        else
        {
            this.OnDisableEvent.Invoke();
        }
    }

    public void Hide(bool isVisible_)
    {
        this.PlayerSprite.gameObject.SetActive(!isVisible_);
    }
}
