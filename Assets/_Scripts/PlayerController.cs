using UnityEngine;
using Moonvalk.Utilities.Algorithms;
using Moonvalk.Utilities;
using Moonvalk.Accessory;

public class PlayerController : MonoBehaviour
{
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
    protected Vector3 _currentMovementSpeed;
    protected bool _enabled = false;

    public Transform PlayerSprite;
    public GameObject SmokeEffect;
    public TextureAnimator Animator;

    protected Vector3 _frontFaceDirection;
    protected MicroTimer _frontFaceDirectionTimer;

    protected MicroTimer _animationTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._collider = GetComponent<Collider>();
        this._currentMovementSpeed = new Vector3();
        this._frontFaceDirection = new Vector3();
    }

    private void Start()
    {
        this._frontFaceDirectionTimer = new MicroTimer(() => {
            this.updateFrontFaceDirection();
            this._frontFaceDirectionTimer.Start(0.2f);
        });
        this._frontFaceDirectionTimer.Start(0.2f);
        this._animationTimer = new MicroTimer();
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

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(this.SmokeEffect, this._frontFaceDirection, Quaternion.Euler(0f, 0f, 0f));
            this.Animator.Play("Attack");
            this._animationTimer.Start(1f);
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
        this.adjustPlayerOrientation();

        if (this._enabled)
        {
            this.animate();
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
            this.Animator.Play("Jump_Melee");
        }
        else
        {
            float collectiveVelocity = Mathf.Abs(this._rigidbody.velocity.x) + Mathf.Abs(this._rigidbody.velocity.z);
            if (collectiveVelocity > 0.2f)
            {
                float timeScale = Mathf.Clamp(collectiveVelocity * 0.4f, 0.2f, 0.8f);
                this.Animator.Play("Run_Melee", timeScale);
            }
            else
            {
                this.Animator.Play("Idle_Melee");
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
        if (this._moveHorizontal != 0 || this._moveVertical != 0)
        {
            this._frontFaceDirection.x = transform.position.x + this._moveHorizontal;
            this._frontFaceDirection.z = transform.position.z + this._moveVertical;
            this._frontFaceDirection.y = transform.position.y;
        }
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
        movementAxis_ = Mathf.Clamp(movementAxis_, -this.MoveSpeed, this.MoveSpeed);
    }

    public void EnablePhysics(bool isEnabled_)
    {
        this._rigidbody.isKinematic = !isEnabled_;
        this._rigidbody.detectCollisions = isEnabled_;
    }

    public void EnableControl(bool isEnabled_)
    {
        this._enabled = isEnabled_;
        this.EnablePhysics(isEnabled_);
    }

    public void Hide(bool isVisible_)
    {
        this.PlayerSprite.gameObject.SetActive(!isVisible_);
    }
}
