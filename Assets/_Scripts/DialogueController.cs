using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using Moonvalk.Utilities;

public class DialogueController : MonoBehaviour
{
    public SpriteRenderer TextboxSprite;

    protected bool _displayingTextbox = false;
    protected Tween _textboxTween;
    protected SpringVec3 _textboxSpring;
    protected float _textboxOpacity = 0f;
    protected Vector3 _textboxScale;

    private void Start()
    {
        this._textboxScale = new Vector3(0f, 0f, 0f);

        this._textboxTween = new Tween(() => ref this._textboxOpacity);
        this._textboxTween.Duration(0.3f).Ease(Easing.Cubic.InOut);
        this._textboxSpring = new SpringVec3(() => ref this._textboxScale);
        this._textboxSpring.Dampening(3.5f).Tension(25f);

        this.updateTextboxDisplay();
    }

    private void Update()
    {
        
        updateTextboxDisplay();
    }

    public void StartDialogue()
    {
        this.showTextbox(true);
    }

    public void EndDialogue()
    {
        this.showTextbox(false);
    }

    private void showTextbox(bool flag_)
    {
        if (this._displayingTextbox == flag_)
        {
            return;
        }
        this._displayingTextbox = flag_;
        this._textboxTween.To(flag_ ? 1f : 0f).Start();
        this._textboxSpring.To(flag_ ? new Vector3(0.25f, 0.25f, 0.25f) : new Vector3(0f, 0f, 0f));
    }

    private void updateTextboxDisplay()
    {
        this.TextboxSprite.transform.localScale = this._textboxScale;
        this.TextboxSprite.color = new Color(1f, 1f, 1f, this._textboxOpacity);
    }
}
