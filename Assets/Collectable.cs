using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities.Algorithms;
using Moonvalk.Utilities;
using Moonvalk.Accessory;
using Moonvalk.Animation;

public class Collectable : MonoBehaviour
{
    public CollectableType Type;
    public float RideHeight = 1f;
    protected InitValue<Vector3> _velocity;
    protected float _gravityAmount = 0.2f;

    public float Tension;
    public float Dampening;
    public float Friction = 10f;
    public float MaxGravity = 10f;

    public float MinDistanceToPlayer = 2f;
    public float PickupRange = 0.7f;

    public float MagnetStrength = 0.01f;
    public float MagnetOffDuration = 0.8f;
    public float PickupOffDuration = 0.3f;

    protected Spring _scaleSpring;
    protected float _scale = 0f;
    protected Tween _scaleTween;
    protected float _scaleMultiplier = 1f;

    protected bool _isMagnetOn = false;
    protected bool _isPickupOn = false;
    protected bool _pickedUp = false;

    public void Awake()
    {
        this._velocity = new InitValue<Vector3>(this.initVelocity);
    }

    public void Start()
    {
        MicroTimer magnetTimer = new MicroTimer(() => {
            this._isMagnetOn = true;
        });
        magnetTimer.Start(this.MagnetOffDuration);
        MicroTimer pickupTimer = new MicroTimer(() => {
            this._isPickupOn = true;
        });
        pickupTimer.Start(this.PickupOffDuration);

        this._scaleSpring = new Spring(() => ref this._scale);
        this._scaleSpring.Tension(50f).Dampening(5f);

        this._scaleTween = new Tween(() => ref this._scaleMultiplier);
        this._scaleTween.Duration(0.5f).Ease(Easing.Cubic.InOut).OnComplete(() => {
            PlayerController.Player1.AddSeed();
            Destroy(this.gameObject);
        });

        this._scaleSpring.To(1f);
    }

    protected void updateScale()
    {
        transform.localScale = new Vector3(this._scale * this._scaleMultiplier, this._scale * this._scaleMultiplier, this._scale * this._scaleMultiplier);
    }

    protected Vector3 initVelocity()
    {
        return new Vector3();
    }

    public void SetVelocity(Vector3 velocity_)
    {
        this._velocity.Value = velocity_;
    }

    public void SetCollectableType(CollectableType type_)
    {
        this.Type = type_;
    }

    public void FixedUpdate()
    {
        if (this._isPickupOn)
        {
            float distance = Vector3.Distance(PlayerController.Player1.transform.position, transform.position);
            if (distance <= this.MinDistanceToPlayer)
            {
                if (this._isMagnetOn)
                {
                    this.moveTowardsPlayer();
                }
                if (distance <= this.PickupRange && !this._pickedUp)
                {
                    this._pickedUp = true;
                    this._scaleTween.To(0f).Start();
                }
            }
        }
        
        this.updateScale();
        this.handleGravity();
        this.handleSpring();

        transform.position = transform.position + (this._velocity.Value * Time.fixedDeltaTime);
    }

    protected void moveTowardsPlayer()
    {
        Vector3 newVelocity = this._velocity.Value;
        newVelocity.x += Mathf.Sign(PlayerController.Player1.transform.position.x - transform.position.x) * this.MagnetStrength;
        newVelocity.z += Mathf.Sign(PlayerController.Player1.transform.position.z - transform.position.z) * this.MagnetStrength;

        this._velocity.Value = newVelocity;
    }

    protected void handleGravity()
    {
        this._velocity.Value += Vector3.down * this._gravityAmount;
        this._velocity.Value = new Vector3(this._velocity.Value.x - (this.Friction * this._velocity.Value.x), Mathf.Clamp(this._velocity.Value.y, -this.MaxGravity, this.MaxGravity),
            this._velocity.Value.z - (this.Friction * this._velocity.Value.z));
    }

    protected void handleSpring()
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
                Vector3 velocity = this._velocity.Value;
                Vector3 rayDirection = Vector3.down;

                Vector3 otherVelocity = Vector3.zero;
                Rigidbody hitBody = hit.rigidbody;
                if (hitBody != null) {
                    otherVelocity = hitBody.velocity;
                }

                float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
                float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);

                float relativeVelocity = (rayDirectionVelocity - otherDirectionVelocity);
                float rideForce = MotionAlgorithms.SimpleHarmonicMotion(this.Tension, (hit.distance - this.RideHeight), this.Dampening, relativeVelocity);
                this._velocity.Value += rayDirection * rideForce;

                if (hitBody != null) {
                    hitBody.AddForceAtPosition(rayDirection * -rideForce, hit.point);
                }
            }
        }
    }
}
