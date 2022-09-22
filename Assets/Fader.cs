using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float FadeDuration;
    public float FadeDelay;
    public Easing.Types FadeEasing;
    public Image ScreenColor;

    protected float _currentOpacity = 1f;
    protected Color _originalColor;

    // Start is called before the first frame update
    private void Start()
    {
        this._originalColor = this.ScreenColor.color;

        Tween fadeOut = new Tween(() => ref this._currentOpacity);
        fadeOut.Duration(this.FadeDuration).Delay(this.FadeDelay).Ease(Easing.Functions[this.FadeEasing]).OnUpdate(() => {
            this.ScreenColor.color = new Color(this._originalColor.r, this._originalColor.g, this._originalColor.b, this._currentOpacity);
        }).OnComplete(() => {
            Destroy(this.gameObject);
        });
        fadeOut.To(0f).Start();
    }
}
