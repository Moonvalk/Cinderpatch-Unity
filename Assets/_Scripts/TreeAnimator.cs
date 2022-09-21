using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Accessory;

public class TreeAnimator : MonoBehaviour
{
    public List<Transform> Trees;
    protected List<Transform> _leaves;

    public float TreeTiltSpeed = 5f;
    public float TreeTiltAmount = 20f;
    public float LeavesTiltSpeed = 5f;
    public float LeavesTiltAmount = 20f;

    protected float _rotationTrees = 0f;
    protected float _rotationLeaves = 0f;
    protected List<float> _originalTreeRotation;
    protected List<float> _originalLeavesRotation;

    private void Start()
    {
        this._leaves = new List<Transform>();
        this._originalTreeRotation = new List<float>();
        this._originalLeavesRotation = new List<float>();
        foreach (Transform tree in Trees)
        {
            this._originalTreeRotation.Add(tree.rotation.eulerAngles.z);
            for (int index = 0; index < tree.childCount; index++)
            {
                Transform child = tree.GetChild(index);
                this._leaves.Add(child);
                this._originalLeavesRotation.Add(child.rotation.eulerAngles.z);
            }
        }

        this.playRotationAnimation(() => ref this._rotationTrees, this.TreeTiltAmount, this.TreeTiltSpeed);
        this.playRotationAnimation(() => ref this._rotationLeaves, this.LeavesTiltAmount, this.LeavesTiltSpeed);
    }

    protected void playRotationAnimation(Ref<float> referenceValue_, float rotationAmount_, float rotationSpeed_)
    {
        Tween tweenIn = new Tween(referenceValue_);
        tweenIn.To(rotationAmount_).Duration(rotationSpeed_).Ease(Easing.Quadratic.InOut)
            .OnComplete(() => {
                Tween tweenOut = new Tween(referenceValue_);
                tweenOut.To(-rotationAmount_).Duration(rotationSpeed_).Ease(Easing.Quadratic.InOut)
                    .OnComplete(() => {
                        tweenIn.Start();
                    });
                tweenOut.Start();
            });
        tweenIn.Start();
    }

    private void FixedUpdate()
    {
        this.updateRotation();
    }

    protected void updateRotation()
    {
        updateListRotation(this.Trees, this._originalTreeRotation, this._rotationTrees);
        updateListRotation(this._leaves, this._originalLeavesRotation, this._rotationLeaves);
    }

    protected void updateListRotation(List<Transform> objects_, List<float> originalRotations_, float rotation_)
    {
        for (int index = 0; index < objects_.Count; index++)
        {
            objects_[index].rotation = Quaternion.Euler(objects_[index].rotation.eulerAngles.x, objects_[index].rotation.eulerAngles.y, originalRotations_[index] + rotation_);
        }
    }
}
