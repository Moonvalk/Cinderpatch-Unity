using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class CoffinAnimator : MonoBehaviour
{
    public float TargetHeight = 3f;
    public float OutHeight = 10f;
    public float Dampening = 5f;
    public float Tension = 25f;

    public GameObject Door;
    public float OpenDoorDegrees = 136f;
    public float OpenDoorDuration = 0.5f;
    public PlayerController Player;
    public GameObject SmokeEffect;

    protected float _height;
    protected float _rotation;
    protected float _doorRotation;

    private void Start()
    {
        this._height = transform.position.y;
        this._rotation = 122f;
        this.Player.EnableControl(false);
        this.Player.EnablePhysics(false);
        this.Player.Hide(true);
        this.Player.transform.position = transform.position;

        this.animateIn();
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, this._height, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, this._rotation, 0f);
        this.Door.transform.rotation = Quaternion.Euler(0f, this._doorRotation + this._rotation, 0f);
    }

    private void animateIn()
    {
        MicroTimer dropTimer = new MicroTimer(() => {
            Spring newSpring = new Spring(() => ref this._height);
            newSpring.To(this.TargetHeight).Tension(this.Tension).Dampening(this.Dampening);
        });
        MicroTimer spinTimer = new MicroTimer(() => {
            Spring spinSpring = new Spring(() => ref this._rotation);
            spinSpring.To(0f).Tension(this.Tension * 2).Dampening(this.Dampening);
        });
        MicroTimer doorTimer = new MicroTimer(this.openDoor);
        doorTimer.OnUpdate(() => {
            this.Player.transform.position = transform.position;
        });
        dropTimer.Start(3f);
        spinTimer.Start(2f);
        doorTimer.Start(9f);
    }

    private void openDoor()
    {
        this.Player.Hide(false);
        this.Player.Animator.Play("Idle");
        Tween open = new Tween(() => ref this._doorRotation);
        open.Ease(Easing.Back.Out).To(OpenDoorDegrees).Duration(OpenDoorDuration).OnComplete(() => {
            MicroTimer jumpTimer = new MicroTimer(() => {
                this.Player.EnablePhysics(true);
                this.Player.Animator.Play("Jump");
                this.Player.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.forward * -500f) + (Vector3.up * 250f));
                this.animateOut();
            });
            jumpTimer.Start(0.5f);
        });
        open.Start();
        Instantiate(this.SmokeEffect, transform.position - (Vector3.up * 0.5f), Quaternion.Euler(0f, 90f, 0f));
        Instantiate(this.SmokeEffect, transform.position, Quaternion.Euler(0f, 90f, 0f));
        Instantiate(this.SmokeEffect, transform.position + (Vector3.up * 0.5f), Quaternion.Euler(0f, 90f, 0f));
    }

    private void animateOut()
    {
        MicroTimer timer = new MicroTimer(() => {
            this.Player.EnableControl(true);
            Tween pull = new Tween(() => ref this._height);
            pull.To(this.OutHeight).Duration(1.5f).Ease(Easing.Cubic.In).Start().OnComplete(() => {
                Destroy(this.gameObject);
            });
        });
        timer.Start(2f);
    }
}
