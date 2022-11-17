using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using Moonvalk.Animation;
using Moonvalk;

[System.Serializable]
public enum SettingsMenuButton
{
    Back,
}

public class SettingsMenuController : MonoBehaviour
{
    public Image SettingsPanel;
    public Image BackButton;
    public Image BackText;
    public UnityEvent BackEvent;

    protected Spring _panelSpring;
    protected Tween _panelTween;
    protected float _panelScale;
    protected float _panelOpacity;

    public Slider MusicSlider;
    public Slider SoundSlider;
    public Toggle FullScreenToggle;
    public AudioMixer Mixer;

    private void Start()
    {
        this.SetMusicVolume(Global.MusicVolume);
        this.SetSoundVolume(Global.SoundVolume);
        this.SetFullScreen(Global.FullScreen);

        this.SettingsPanel.gameObject.SetActive(false);
        this.SettingsPanel.color = new Color(1f, 1f, 1f, 0f);

        this._panelSpring = new Spring(() => ref this._panelScale);
        this._panelSpring.Tension(50f).Dampening(10f).OnUpdate(() => {
            this.SettingsPanel.rectTransform.localScale = new Vector3(this._panelScale, this._panelScale, this._panelScale);
        });
        this._panelTween = new Tween(() => ref this._panelOpacity);
        this._panelTween.Duration(0.5f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.SettingsPanel.color = new Color(1f, 1f, 1f, this._panelOpacity);
        }).OnComplete(() => {
            if (this._panelOpacity == 0f)
            {
                this.SettingsPanel.gameObject.SetActive(false);
            }
        });
    }

    public void ShowMenu()
    {
        this.SettingsPanel.gameObject.SetActive(true);
        this._panelSpring.To(1f);
        this._panelTween.To(1f).Start();
    }

    public void HideMenu()
    {
        this._panelSpring.To(0f);
        this._panelTween.To(0f).Start();
    }

    public void ButtonEnterHover(SettingsMenuComponent component_)
    {
        
    }

    public void ButtonExitHover(SettingsMenuComponent component_)
    {
        
    }

    public void ButtonClick(SettingsMenuComponent component_)
    {
        switch(component_.Button)
        {
            case SettingsMenuButton.Back:
                this.HideMenu();
                this.BackEvent.Invoke();
                break;
        }
    }

    public void SetMusicVolume(float volume_)
    {
        Global.MusicVolume = volume_;
        this.Mixer.SetFloat("MusicVolume", Mathf.Log10(Global.MusicVolume) * 20f);
        this.MusicSlider.SetValueWithoutNotify(Global.MusicVolume);
    }

    public void SetSoundVolume(float volume_)
    {
        Global.SoundVolume = volume_;
        this.Mixer.SetFloat("SoundVolume", Mathf.Log10(Global.SoundVolume) * 20f);
        this.SoundSlider.SetValueWithoutNotify(Global.SoundVolume);
    }

    public void SetFullScreen(bool flag_)
    {
        Global.FullScreen = flag_;
        Screen.fullScreen = Global.FullScreen;
        this.FullScreenToggle.isOn = flag_;
    }
}
