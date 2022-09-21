using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities;
using Moonvalk.Animation;

public enum CameraTargetPoint
{
    Main,
    Low,
    Cinematic,
}

public class CameraController : MonoBehaviour
{
    public Transform FollowTarget;
    public float HorizontalSway = 0.1f;
    public float VerticalSway = 0.1f;

    [SerializeField]
    protected float _zoomAmount = -2f;
    protected TransformLite _currentTransform;
    protected TransformLite _rig;
    protected TransformLite _followTargetTransform;
    protected Vector3 _swayTarget;
    protected Tween _zoomTween;

    protected float _RIG_THRESHOLD = 0.001f;

    private void Start()
    {
        this._rig = new TransformLite(this.transform);
        this._currentTransform = new TransformLite(this.transform);
        this._followTargetTransform = new TransformLite(this.FollowTarget.transform);
        this._swayTarget = this._followTargetTransform.Position;
        this.Zoom(0f, 8f);
    }

    private void FixedUpdate()
    {
        Vector3 targetSwayPosition = new Vector3(this.FollowTarget.position.x * this.HorizontalSway, 0f, this.FollowTarget.position.z * this.VerticalSway);
        if (Vector3.Distance(this._swayTarget, targetSwayPosition) > _RIG_THRESHOLD)
        {
            this._swayTarget = targetSwayPosition;
            this.AnimateTo(this._swayTarget);
        }

        this._currentTransform.Position = new Vector3(this._rig.Position.x + this._followTargetTransform.Position.x, this._rig.Position.y,
            this._rig.Position.z + this._followTargetTransform.Position.z + this._zoomAmount);
        this._currentTransform.Rotation = this._rig.Rotation;

        transform.position = this._currentTransform.Position;
        transform.rotation = Quaternion.Euler(this._currentTransform.Rotation.x, this._currentTransform.Rotation.y, this._currentTransform.Rotation.z);
    }

    public void AnimateTo(Vector3 position_) {
        // Reset the camera.
        TweenVec3 resetTween = new TweenVec3(() => ref this._followTargetTransform.Position);
        resetTween.To(position_).Duration(0.3f).Ease(Easing.Cubic.Out);
        resetTween.Start();
    }

    public void SetRigTransform(TransformLite targetTransform_)
    {
        this._rig = targetTransform_;
    }

    public void Zoom(float zoomAmount_, float duration_)
    {
        if (this._zoomTween != null)
        {
            this._zoomTween.Stop();
            this._zoomTween = null;
        }
        this._zoomTween = new Tween(() => ref this._zoomAmount);
        this._zoomTween.To(zoomAmount_).Duration(duration_).Ease(Easing.Cubic.InOut).Start();
    }
}
