using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;

public class IntroCamera : MonoBehaviour
{
    public Vector3 MoveAmount;
    public float MoveDuration;
    public float MoveDelay;
    public Easing.Types MoveEasing;

    protected Vector3 _originalPosition;
    protected Vector3 _position;

    private void Start()
    {
        this._originalPosition = transform.position;
        this._position = this._originalPosition;
        TweenVec3 moveTween = new TweenVec3(() => ref this._position);
        moveTween.Duration(this.MoveDuration).Delay(this.MoveDelay).Ease(Easing.Functions[this.MoveEasing]).OnUpdate(() => {
            transform.position = this._position;
        });
        moveTween.To(this._originalPosition + this.MoveAmount).Start();
    }
}
