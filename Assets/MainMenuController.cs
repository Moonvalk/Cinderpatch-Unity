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
public enum MainMenuButton
{
    Play,
    Settings,
    News,
    Exit,
}

public class MainMenuController : MonoBehaviour
{
    public string NewsURL;

    public Image MenuBackground;
    public float MenuBackgroundOpacity = 0.75f;
    public float MenuDelay = 8f;

    protected MicroTimer _menuTimer;
    protected bool _isMenuDisplaying = false;
    protected float _backgroundOpacity = 0f;

    protected float _tiltValue = 0f;

    public Image GameLogo1;
    public Image GameLogo2;
    public float LogoSizeIdle = 0.7f;
    public float LogoSizePushed = 0.6f;
    public RectTransform LogoPushedPosition;
    public float LogoPushDelay = 3f;

    protected Spring _gameLogoSpring;
    protected Tween _gameLogoTween;
    protected TweenVec3 _gameLogoPushTween;
    protected float _gameLogoOpacity = 0f;
    protected float _gameLogoScale = 0f;
    protected Vector3 _gameLogoPush;
    protected Vector3 _originalLogo1Position;
    protected Vector3 _originalLogo2Position;

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

    public UnityEvent EnterPlayEvent;
    public UnityEvent SettingsEvent;

    protected AudioSource _menuMusic;
    protected float _musicVolume;
    protected Tween _musicTween;

    private void Awake()
    {
        this._menuMusic = GetComponent<AudioSource>();
        this._menuMusic.volume = 0f;
        this._menuMusic.loop = true;
        this._musicVolume = 0f;
    }

    private void Start()
    {
        this._menuTimer = new MicroTimer(() => {
            this.DisplayMenu(true);
        });
        this._menuTimer.Start(this.MenuDelay);
        this.updateBackgroundOpacity();

        // Logo animations.
        this._gameLogoTween = new Tween(() => ref this._gameLogoOpacity);
        this._gameLogoTween.Duration(0.5f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.updateGameLogoOpacity();
        });
        this._gameLogoSpring = new Spring(() => ref this._gameLogoScale);
        this._gameLogoSpring.Tension(50f).Dampening(4.5f);
        this.updateGameLogoOpacity();
        this.updateGameLogoScale();

        // Logo position.
        this._originalLogo1Position = this.GameLogo1.rectTransform.position;
        this._originalLogo2Position = this.GameLogo2.rectTransform.position;
        this._gameLogoPush = new Vector3(0f, 0f, 0f);
        this._gameLogoPushTween = new TweenVec3(() => ref this._gameLogoPush);
        this._gameLogoPushTween.Duration(1f).Ease(Easing.Cubic.InOut).OnUpdate(() => {
            this.updateGameLogoPosition();
        });

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

        // Music
        this._musicTween = new Tween(() => ref this._musicVolume);
        this._musicTween.Duration(4f).OnUpdate(() => {
            this._menuMusic.volume = this._musicVolume;
        });
    }

    private void updateGameLogoScale()
    {
        float offset = Mathf.Sin(this._tiltValue * 0.4f) * 0.1f;
        this.GameLogo1.rectTransform.localScale = new Vector3(this._gameLogoScale + offset, this._gameLogoScale + offset, this._gameLogoScale);
        this.GameLogo2.rectTransform.localScale = new Vector3(this._gameLogoScale + offset, this._gameLogoScale + offset, this._gameLogoScale);
    }

    private void updateGameLogoOpacity()
    {
        this.GameLogo1.color = new Color(1f, 1f, 1f, this._gameLogoOpacity);
        this.GameLogo2.color = new Color(1f, 1f, 1f, this._gameLogoOpacity);
    }

    private void updateGameLogoPosition()
    {
        this.GameLogo1.transform.position = this._originalLogo1Position + this._gameLogoPush;
        this.GameLogo2.transform.position = this._originalLogo2Position + this._gameLogoPush;
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

    private void Update()
    {
        this._tiltValue += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        this.GameLogo1.rectTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(this._tiltValue * 2f) * 15f);
        this.GameLogo2.rectTransform.rotation = Quaternion.Euler(0f, 0f, -Mathf.Sin(this._tiltValue * 0.7f) * 8f);
        this.updateGameLogoScale();
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
            this.animateLogoIn();
        });
        fadeBackground.To(this.MenuBackgroundOpacity).Start();
        this._menuMusic.loop = true;
        this._menuMusic.Play();
        this._musicTween.To(1f).Start();
    }

    protected void animateLogoIn()
    {
        this._gameLogoTween.To(1f).Start();
        this._gameLogoSpring.To(this.LogoSizeIdle);
        MicroTimer pushLogoTimer = new MicroTimer(() => {
            this.pushLogo();
        });
        pushLogoTimer.Start(this.LogoPushDelay);
    }

    protected void pushLogo()
    {
        this._gameLogoSpring.To(this.LogoSizePushed);
        this._gameLogoPushTween.To(this.LogoPushedPosition.position).Start();

        Tween menuPanelTween = new Tween(() => ref this._menuPanelPositionX);
        menuPanelTween.Duration(1f).Ease(Easing.Cubic.Out).OnUpdate(() => {
            this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);
        });
        menuPanelTween.To(0f).Start();

        this._fadeInTween.To(1f).Start();
    }

    public void ButtonEnterHover(MainMenuComponent component_)
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

    public void ButtonExitHover(MainMenuComponent component_)
    {
        this._activeButtonHovered = -1;
    }

    public void ButtonClick(MainMenuComponent component_)
    {
        switch(component_.Button)
        {
            case MainMenuButton.Play:
                this.enterPlay();
                break;
            case MainMenuButton.Settings:
                this.enterSettings();
                break;
            case MainMenuButton.News:
                Application.OpenURL(this.NewsURL);
                break;
            case MainMenuButton.Exit:
                Application.Quit();
                break;
        }
    }

    protected void enterPlay()
    {
        this._musicTween.To(0f).Start();
        this.hideMenu();
        this.hideBackground();
        this.EnterPlayEvent.Invoke();
        MicroTimer timer = new MicroTimer(() => {
            Global.Systems.ClearAllSystems();
            SceneManager.LoadScene("Level1");
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

        this._gameLogoTween.To(0f).Start();
        this._gameLogoSpring.To(0f);
    }

    public void ShowMenu()
    {
        Tween menuPanelTween = new Tween(() => ref this._menuPanelPositionX);
        menuPanelTween.Duration(1f).Ease(Easing.Cubic.Out).OnUpdate(() => {
            this.MenuPanel.position = new Vector3(this._originalMenuPosition.x - this._menuPanelPositionX, this._originalMenuPosition.y, this._originalMenuPosition.z);
        });
        menuPanelTween.To(0f).Start();

        this._gameLogoTween.To(1f).Start();
        this._gameLogoSpring.To(this.LogoSizePushed);
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
