using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Moonvalk;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class Interaction : MonoBehaviour
{
    public float MaximumInteractDistance = 2f;
    public UnityEvent InteractAction;
    public UnityEvent LeaveInteractAction;
    public SpriteRenderer PromptSprite;
    public float PromptHeight = 2f;

    protected bool _showingPrompt = false;
    protected Tween _promptTween;
    protected SpringVec3 _promptSpring;
    protected PlayerController _player;
    protected bool _interacting = false;
    protected TransformLite _promptTransform;
    protected float _promptOpacity = 0f;

    public void Start()
    {
        this._player = PlayerController.Player1;

        this._promptTransform = new TransformLite();
        this._promptTransform.Scale = new Vector3(0f, 0f, 0f);

        this._promptTween = new Tween(() => ref this._promptOpacity);
        this._promptTween.Duration(0.3f).Ease(Easing.Cubic.InOut);
        this._promptSpring = new SpringVec3(() => ref this._promptTransform.Scale);
        this._promptSpring.Dampening(5f).Tension(50f);

        this.updatePromptDisplay();
    }

    public void Update()
    {
        if (Vector3.Distance(this._player.transform.position, transform.position) <= this.MaximumInteractDistance)
        {
            if (!this._interacting)
            {
                this.showPrompt(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    this.InteractAction.Invoke();
                    
                    this._interacting = true;
                }
            }
            else
            {
                this.showPrompt(false);
            }
        }
        else 
        {
            this.showPrompt(false);
            if (this._interacting)
            {
                this.LeaveInteractAction.Invoke();
                this._interacting = false;
            }
        }
        this.updatePromptDisplay();
    }

    private void updatePromptDisplay()
    {
        this.PromptSprite.transform.localScale = this._promptTransform.Scale;
        this.PromptSprite.color = new Color(1f, 1f, 1f, this._promptOpacity);
    }

    private void showPrompt(bool flag_)
    {
        if (this._showingPrompt == flag_)
        {
            return;
        }
        this._showingPrompt = flag_;
        this._promptTween.To(flag_ ? 1f : 0f).Start();
        this._promptSpring.To(flag_ ? new Vector3(0.6f, 0.6f, 0.6f) : new Vector3(0f, 0f, 0f));
    }
}
