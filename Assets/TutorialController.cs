using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class TutorialController : MonoBehaviour
{
    public Image[] Prompts;
    protected int _currentPrompt = 0;

    protected Tween _promptTween;
    protected Spring _promptSpring;
    protected float _promptScale = 0f;
    protected float _promptOpacity = 0f;

    protected bool _animatingIn = true;
    protected MicroTimer _promptTimer;

    private void Start()
    {
        foreach (Image img in this.Prompts)
        {
            img.color = new Color(1f, 1f, 1f, 0f);
            img.gameObject.SetActive(false);
        }

        this._promptTween = new Tween(() => ref this._promptOpacity);
        this._promptTween.Duration(0.5f).Ease(Easing.Cubic.InOut).OnComplete(() => {
            this.checkForNextPrompt();
        });
        this._promptSpring = new Spring(() => ref this._promptScale);
        this._promptSpring.Tension(50f).Dampening(6.5f);


        this._promptTimer = new MicroTimer(() => {
            this.NextStep();
            this._promptTimer.Start(15f);
        });
    }

    private void FixedUpdate()
    {
        this.updateCurrentPrompt();
    }

    protected void updateCurrentPrompt()
    {
        this.Prompts[this._currentPrompt].color = new Color(1f, 1f, 1f, this._promptOpacity);
        this.Prompts[this._currentPrompt].transform.localScale = new Vector3(this._promptScale, this._promptScale, this._promptScale);
    }

    public void DisplayPrompt()
    {
        // Call for first tutorial step here.
        this._animatingIn = true;
        this.Prompts[this._currentPrompt].gameObject.SetActive(true);
        this._promptTween.To(1f).Start();
        this._promptSpring.To(1f);
        this._promptTimer.Start(15f);
    }
    
    public void NextStep()
    {
        this._animatingIn = false;
        this._promptTween.To(0f).Start();
        this._promptSpring.To(0f);
    }

    public void ShowNextStep()
    {
        this._currentPrompt++;
        if (this._currentPrompt < this.Prompts.Length)
        {
            this._animatingIn = true;
            this.Prompts[this._currentPrompt].gameObject.SetActive(true);
            this._promptTween.To(1f).Start();
            this._promptSpring.To(1f);
        }
        else
        {
            this._promptTween.Stop();
            this._promptSpring.Stop();
            this._promptTimer.Stop();
            Destroy(this.gameObject);
        }
    }

    private void checkForNextPrompt()
    {
        if (!this._animatingIn)
        {
            this.Prompts[this._currentPrompt].gameObject.SetActive(false);
            this.ShowNextStep();
        }
    }

}
