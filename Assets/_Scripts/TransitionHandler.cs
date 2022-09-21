using System;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Accessory;
using Moonvalk.Utilities;

public class TransitionHandler : MonoBehaviour
{
    public Material VinesSet1;
    public Material VinesSet2;

    public float Set1Duration;
    public float Set2Duration;
    public float Set1Delay;
    public float Set2Delay;

    public bool IsOpen = false;

    protected float _vineGrowthSet1;
    protected float _vineGrowthSet2;

    protected MicroTimer timer;

    private void Start()
    {
        this._vineGrowthSet1 = this.IsOpen ? 0f : 1f;
        this._vineGrowthSet2 = this.IsOpen ? 0f : 1f;
        this.updateVineMaterials();
        this.Transition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.Transition();
        }
    }

    public void Transition()
    {
        if (this.IsOpen)
        {
            this.IsOpen = false;
            this.Close();
        }
        else
        {
            this.IsOpen = true;
            this.Open();
        }
    }

    private void FixedUpdate()
    {
        this.updateVineMaterials();
        
    }

    protected void updateVineMaterials()
    {
        this.VinesSet1.SetFloat("_Grow", this._vineGrowthSet1);
        this.VinesSet2.SetFloat("_Grow", this._vineGrowthSet2);
    }

    public void Open()
    {
        this.animateGrowth(() => ref this._vineGrowthSet1, 0f, this.Set1Duration, this.Set1Delay, () => { setTransitionMeshActive(true); });
        this.animateGrowth(() => ref this._vineGrowthSet2, 0f, this.Set2Duration, this.Set2Delay);
    }

    public void Close()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        this.animateGrowth(() => ref this._vineGrowthSet1, 1f, this.Set2Duration, this.Set2Delay);
        this.animateGrowth(() => ref this._vineGrowthSet2, 1f, this.Set1Duration, this.Set1Delay);
    }

    protected void animateGrowth(Ref<float> referenceValue_, float target_, float duration_, float delay_)
    {
        this.animateGrowth(referenceValue_, target_, duration_, delay_, () => {});
    }

    protected void animateGrowth(Ref<float> referenceValue_, float target_, float duration_, float delay_, Action onComplete_)
    {
        Tween newTween = new Tween(referenceValue_);
        newTween.Duration(duration_).To(target_).Ease(Easing.Cubic.InOut).Delay(delay_).OnComplete(() => {
            onComplete_();
        });
        newTween.Start();
    }

    protected void setTransitionMeshActive(bool flag_)
    {
        transform.GetChild(0).gameObject.SetActive(flag_);
    }
}
