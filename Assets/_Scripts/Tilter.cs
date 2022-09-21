using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;

public class Tilter : MonoBehaviour
{
    public float TiltSpeed = 5f;
    public float TiltAmount = 20f;
    protected float _rotation = 0f;
    protected Vector3 _originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        this._originalRotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        Tween tween1 = new Tween(() => ref this._rotation);
        tween1.To(this.TiltAmount * 0.5f).Duration(this.TiltSpeed).Ease(Easing.Quadratic.InOut)
            .OnComplete(() => {
                Tween tween2 = new Tween(() => ref this._rotation);
                tween2.To(-this.TiltAmount * 0.5f).Duration(this.TiltSpeed).Ease(Easing.Quadratic.InOut)
                    .OnComplete(() => {
                        tween1.Start();
                    })
                    .OnUpdate(() => {
                        this.updateRotation();
                    });
                tween2.Start();
            })
            .OnUpdate(() => {
                this.updateRotation();
            });
        tween1.Start();
    }

    protected void updateRotation()
    {
        transform.rotation = Quaternion.Euler(this._originalRotation.x, this._originalRotation.y, this._originalRotation.z + this._rotation);
    }
}
