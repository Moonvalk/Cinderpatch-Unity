using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities;
using Moonvalk.Animation;

public class CameraPerspectiveHandler : MonoBehaviour
{
    public CameraController Controller;
    public CapsuleCollider Collider;
    public List<CameraPerspective> Perspectives;

    protected Dictionary<CameraTargetPoint, CameraPerspective> _perspectiveMap;
    protected TransformLite _originalPerspective;
    protected CameraTargetPoint _currentPerspective = CameraTargetPoint.Main;
    protected TransformLite _currentTransform;

    private void Start()
    {
        this._currentTransform = new TransformLite(this.Controller.transform);
        this._originalPerspective = new TransformLite(this.Controller.transform);
        this._perspectiveMap = new Dictionary<CameraTargetPoint, CameraPerspective>();
        for (int index = 0; index < this.Perspectives.Count; index++)
        {
            this._perspectiveMap.Add(this.Perspectives[index].Target, this.Perspectives[index]);
        }
    }

    private void FixedUpdate()
    {
        this.Controller.SetRigTransform(this._currentTransform);
    }

    public void AnimateForCurrentPerspective()
    {
        switch(this._currentPerspective)
        {
            case CameraTargetPoint.Main:
                this.AnimateTo(this._originalPerspective.Position, this._originalPerspective.Rotation);
                break;
            default:
                this.AnimateTo(this._perspectiveMap[this._currentPerspective].transform.position, this._perspectiveMap[this._currentPerspective].transform.rotation.eulerAngles);
                break;
        }
    }

    public void AnimateTo(Vector3 position_, Vector3 rotation_) {
        // Reset the camera.
        TweenVec3 resetTween = new TweenVec3(() => ref this._currentTransform.Position, () => ref this._currentTransform.Rotation);
        resetTween.To(position_, rotation_).Duration(1.5f).Ease(Easing.Cubic.InOut);
        resetTween.Start();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "CameraPerspective")
        {
            CameraPerspective perspective = other.GetComponentInChildren<CameraPerspective>();
            if (perspective.Enabled && this._currentPerspective != perspective.Target)
            {
                this._currentPerspective = perspective.Target;
                this.AnimateForCurrentPerspective();
            }
        }
    }
    
    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.tag == "CameraPerspective")
    //     {
    //         CameraPerspective perspective = other.GetComponentInChildren<CameraPerspective>();
    //         if (perspective.Enabled && this._currentPerspective != perspective.Target)
    //         {
    //             this._currentPerspective = perspective.Target;
    //             this.AnimateForCurrentPerspective();
    //         }
    //     }
    // }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "CameraPerspective")
        {
            CameraPerspective perspective = other.GetComponentInChildren<CameraPerspective>();
            if (perspective.Enabled && this._currentPerspective != CameraTargetPoint.Main)
            {
                this._currentPerspective = CameraTargetPoint.Main;
                this.AnimateForCurrentPerspective();
            }
        }
    }

    public void UseMainPerspective()
    {
        if (this._currentPerspective != CameraTargetPoint.Main)
        {
            this._currentPerspective = CameraTargetPoint.Main;
            this.AnimateForCurrentPerspective();
        }
    }
}
