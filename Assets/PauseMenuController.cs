using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Moonvalk.Animation;
using Moonvalk.Utilities;
using Moonvalk;

[System.Serializable]
public enum PauseMenuButton
{
    Resume,
    Settings,
    Main,
    Exit,
}

public class PauseMenuController : MonoBehaviour
{
    public Image MenuBackground;
    public float MenuBackgroundOpacity = 0.75f;
    public float MenuDelay = 8f;

    protected MicroTimer _menuTimer;
    protected bool _isMenuDisplaying = false;
    protected float _backgroundOpacity = 0f;

    public RectTransform MenuPanel;
    public float MenuPanelStartOffset = -800f;
    private float _menuPanelPositionX;
    protected Vector3 _originalMenuPosition;

    public Color TextColorHover;

    public Image[] FadeInList;
    protected Tween _fadeInTween;
    protected float _fadeInOpacity;

    public Image[] Buttons;
    public Image[] ButtonText;
    public Image Hover;
    public Vector2 HoverOffset;

    protected int _activeButtonHovered = -1;
    protected Spring _hoverSpring;
    protected float _hoverScale;

    public UnityEvent ResumeEvent;
    public UnityEvent SettingsEvent;
    public UnityEvent MainMenuEvent;

    private void Start()
    {
        this.updateBackgroundOpacity();
        this._originalMenuPosition = this.MenuPanel.position;
        this._menuPanelPositionX = this.MenuPanelStartOffset;
        this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);

        this._fadeInOpacity = 0f;
        this._fadeInTween = new Tween(() => ref this._fadeInOpacity);
        this._fadeInTween.Duration(0.5f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.updateFadeInList();
        });
        this.updateFadeInList();

        // Hover button.
        this._hoverScale = 1f;
        this._hoverSpring = new Spring(() => ref this._hoverScale);
        this._hoverSpring.Tension(50f).Dampening(5f).OnUpdate(() => {
            Vector3 newScale = new Vector3(this._hoverScale, this._hoverScale, this._hoverScale);
            if (this._activeButtonHovered != -1)
            {
                this.ButtonText[this._activeButtonHovered].transform.localScale = newScale;
            }
            this.Hover.transform.localScale = newScale;
        });
    }

    private void updateBackgroundOpacity()
    {
        Color original = this.MenuBackground.color;
        this.MenuBackground.color = new Color(original.r, original.g, original.b, this._backgroundOpacity);
    }

    private void updateFadeInList()
    {
        foreach (Image img in FadeInList)
        {
            img.color = new Color(1f, 1f, 1f, this._fadeInOpacity);
        }
    }

    public void DisplayMenu(bool flag_)
    {
        if (this._isMenuDisplaying == flag_)
        {
            return;
        }
        this._isMenuDisplaying = flag_;
        if (this._isMenuDisplaying)
        {
            this.fadeBackgroundIn();
        }
    }

    protected void fadeBackgroundIn()
    {
        Tween fadeBackground = new Tween(() => ref this._backgroundOpacity);
        fadeBackground.Duration(1f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.updateBackgroundOpacity();
        })
        .OnComplete(() => {
            this.pushMenu();
        });
        fadeBackground.To(this.MenuBackgroundOpacity).Start();
    }

    protected void pushMenu()
    {
        Tween menuPanelTween = new Tween(() => ref this._menuPanelPositionX);
        menuPanelTween.Duration(1f).Ease(Easing.Cubic.Out).OnUpdate(() => {
            this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);
        });
        menuPanelTween.To(0f).Start();
        this._fadeInTween.To(1f).Start();
    }

    public void ButtonEnterHover(PauseMenuComponent component_)
    {
        int buttonIndex = (int)component_.Button;
        this._activeButtonHovered = buttonIndex;
        foreach (Image text in this.ButtonText)
        {
            text.color = Color.white;
        }

        for (int index = 0; index < this.Buttons.Length; index++) {
            this.ButtonText[index].color = Color.white;
            this.ButtonText[index].transform.localScale = Vector3.one;
            this.Buttons[index].color = Color.white;
        }
        
        this.ButtonText[buttonIndex].color = this.TextColorHover;
        Image active = this.Buttons[buttonIndex];
        active.color = new Color(1f, 1f, 1f, 0f);
        this.Hover.transform.position = new Vector3(active.transform.position.x + this.HoverOffset.x, active.transform.position.y + this.HoverOffset.y, this.Hover.transform.position.z);
        this._hoverSpring.Snap(0.9f);
        this._hoverSpring.To(1f);
    }

    public void ButtonExitHover(PauseMenuComponent component_)
    {
        this._activeButtonHovered = -1;
    }

    public void ButtonClick(PauseMenuComponent component_)
    {
        switch(component_.Button)
        {
            case PauseMenuButton.Resume:
                this.enterResume();
                break;
            case PauseMenuButton.Settings:
                this.enterSettings();
                break;
            case PauseMenuButton.Main:
                this.enterMainMenu();
                break;
            case PauseMenuButton.Exit:
                Application.Quit();
                break;
        }
    }

    protected void enterResume()
    {
        this.hideMenu();
        this.hideBackground();
        this.ResumeEvent.Invoke();
        MicroTimer timer = new MicroTimer(() => {
            this.gameObject.SetActive(false);
        });
        timer.Start(1f);
    }

    protected void enterMainMenu()
    {
        this.hideMenu();
        this.hideBackground();
        this.MainMenuEvent.Invoke();
        MicroTimer timer = new MicroTimer(() => {
            Global.Systems.ClearAllSystems();
            SceneManager.LoadScene("Menu");
        });
        timer.Start(6f);
    }

    protected void enterSettings()
    {
        this.hideMenu();
        this.SettingsEvent.Invoke();
    }

    protected void hideMenu()
    {
        Tween menuPanelTween = new Tween(() => ref this._menuPanelPositionX);
        menuPanelTween.Duration(1f).Ease(Easing.Cubic.Out).OnUpdate(() => {
            this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);
        });
        menuPanelTween.To(this.MenuPanelStartOffset).Start();
        this._isMenuDisplaying = false;
    }

    public void ShowMenu()
    {
        Tween menuPanelTween = new Tween(() => ref this._menuPanelPositionX);
        menuPanelTween.Duration(1f).Ease(Easing.Cubic.Out).OnUpdate(() => {
            this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);
        });
        menuPanelTween.To(0f).Start();
    }

    protected void hideBackground()
    {
        // Fade background.
        Tween fadeBackground = new Tween(() => ref this._backgroundOpacity);
        fadeBackground.Duration(1f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.updateBackgroundOpacity();
        });
        fadeBackground.To(0f).Start();
        this._fadeInTween.To(0f).Start();
    }
}
