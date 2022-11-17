using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Moonvalk.Animation;
using Moonvalk.Utilities;
using TMPro;

[System.Serializable]
public struct DialogueQueueItem
{
    public List<string> Text;
    public int Threshold;
    public bool SkipWhenDone;
    public UnityEvent OnThresholdMetEvent;
}

public class DialogueController : MonoBehaviour
{
    public SpriteRenderer TextboxSprite;
    public TextMeshPro TextBox;
    public SpriteRenderer PromptKey;
    public TextMeshPro SeedRequirementText;

    public List<DialogueQueueItem> DialogueQueue;

    protected int _currentQueueItem = 0;
    protected int _currentQueueText = 0;

    protected bool _displayingTextbox = false;
    protected Tween _textboxTween;
    protected SpringVec3 _textboxSpring;
    protected float _textboxOpacity = 0f;
    protected Vector3 _textboxScale;

    protected MicroTimer _delayTimer;
    protected bool _isDelayingPress = false;

    protected Tween _promptKeyTween;
    protected float _promptKeyOpacity = 0f;

    public bool EndOfCurrentDialogue
    {
        get
        {
            return (this._currentQueueText >= this.DialogueQueue[this._currentQueueItem].Text.Count);
        }
    }

    private void Start()
    {
        this._textboxScale = new Vector3(0f, 0f, 0f);

        this._textboxTween = new Tween(() => ref this._textboxOpacity);
        this._textboxTween.Duration(0.3f).Ease(Easing.Cubic.InOut).OnComplete(() => {
            this._isDelayingPress = false;
        });
        this._textboxSpring = new SpringVec3(() => ref this._textboxScale);
        this._textboxSpring.Dampening(3.5f).Tension(25f);

        this.updateTextboxDisplay();

        this._delayTimer = new MicroTimer(() => {
            this._isDelayingPress = false;
        });

        this._promptKeyTween = new Tween(() => ref this._promptKeyOpacity);
        this._promptKeyTween.Duration(0.5f).Ease(Easing.Cubic.InOut);
    }

    private void FixedUpdate()
    {
        this.PromptKey.color = new Color(1f, 1f, 1f, this._promptKeyOpacity);
        this.TextBox.color = new Color(1f, 1f, 1f, this._textboxOpacity);
        updateTextboxDisplay();
    }

    private void Update()
    {
        if (this._displayingTextbox)
        {
            if (Input.GetKeyDown(KeyCode.E) && !this._isDelayingPress)
            {
                this.NextDialogue();
                this._delayTimer.Start(1f);
            }
        }
    }

    public void StartDialogue()
    {
        this._currentQueueText = 0;
        this.showTextbox(true);
        this.showPrompt();
        this._isDelayingPress = true;
        this._delayTimer.Start(1f);
    }

    public void EndDialogue()
    {
        this.showTextbox(false);
        this.hidePrompt();
        this._isDelayingPress = true;
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
        this.updateTextBoxContents();
    }

    private void updateTextboxDisplay()
    {
        this.TextboxSprite.transform.localScale = this._textboxScale;
        this.TextboxSprite.color = new Color(1f, 1f, 1f, this._textboxOpacity);
    }

    public void NextDialogue()
    {
        this._currentQueueText++;
        if (this.EndOfCurrentDialogue)
        {
            this._currentQueueText = this.DialogueQueue[this._currentQueueItem].Text.Count - 1;
            if (this.isSeedThresholdMet() || this.DialogueQueue[this._currentQueueItem].SkipWhenDone)
            {
                this.DialogueQueue[this._currentQueueItem].OnThresholdMetEvent.Invoke();
                if (this._currentQueueItem < this.DialogueQueue.Count - 1)
                {
                    this._currentQueueText = 0;
                    this._currentQueueItem++;
                }
                else
                {
                    this.hidePrompt();
                }
            }
            else
            {
                this.hidePrompt();
            }
        }

        this._textboxSpring.Snap(new Vector3(0.2f, 0.2f, 0.2f));
        this._textboxSpring.To(new Vector3(0.25f, 0.25f, 0.25f));
        this.updateTextBoxContents();
    }

    protected void updateTextBoxContents()
    {
        this.TextBox.text = this.DialogueQueue[this._currentQueueItem].Text[this._currentQueueText];
        this.SeedRequirementText.text = "x" + this.DialogueQueue[this._currentQueueItem].Threshold;
    }

    protected void showPrompt()
    {
        this._promptKeyTween.To(1f).Start();
    }

    protected void hidePrompt()
    {
        this._promptKeyTween.To(0f).Start();
    }

    public bool isSeedThresholdMet()
    {
        return (PlayerController.SeedCount >= this.DialogueQueue[this._currentQueueItem].Threshold);
    }
}
