using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities;
using Moonvalk.Accessory;

public class TextureAnimator : MonoBehaviour
{
    [SerializeField]
    protected MeshRenderer _renderer;

    public TextureAnimation[] Animations;
    public string DefaultAnimation = "";
    public bool RandomizeFirstFrame = false;
    protected Dictionary<string, TextureAnimation> _animationMap;
    protected string _currentAnimation = "";
    protected InitValue<MicroTimer> _animationTimer;
    protected int _currentFrame = 0;

    private void Awake()
    {
        // Establish a clone of the material selected for instance.s
        this._renderer = GetComponent<MeshRenderer>();
        this._renderer.material = new Material(this._renderer.material);

        this._animationMap = new Dictionary<string, TextureAnimation>();
        foreach(TextureAnimation animation in Animations)
        {
            this._animationMap.Add(animation.Name, animation);
        }

        this._animationTimer = new InitValue<MicroTimer>(this.initAnimationTimer);
    }

    private void Start()
    {
        if (DefaultAnimation != "") {
            this.Play(DefaultAnimation);
            if (RandomizeFirstFrame) {
                this._currentFrame = Random.Range(0, this._animationMap[_currentAnimation].Frames.Count - 1);
            }
        }
    }

    protected MicroTimer initAnimationTimer()
    {
        MicroTimer newTimer = new MicroTimer(() => {
            this.playNextFrame();
        });
        return newTimer;
    }

    public void Play(string animationName_)
    {
        this.Play(animationName_, true, 1f);
    }

    public void Play(string animationName_, float timeScale_)
    {
        this.Play(animationName_, true, timeScale_);
    }

    public void Play(string animationName_, bool restart_, float timeScale_)
    {
        this._animationTimer.Value.SetTimeScale(timeScale_);
        if (this._currentAnimation == animationName_)
        {
            return;
        }
        this._currentAnimation = animationName_;
        if (restart_)
        {
            this._currentFrame = this._animationMap[this._currentAnimation].Frames.Count - 1;
        }
        this.playNextFrame();
    }

    protected void playNextFrame()
    {
        TextureAnimation current = this._animationMap[this._currentAnimation];
        TextureFrame frame = this.NextFrame();
        float frameDuration = this.CurrentFrameDuration();
        this._renderer.material.mainTexture = frame.Image;
        this._animationTimer.Value.Start(frameDuration);
    }

    public TextureFrame NextFrame()
    {
        this._currentFrame++;
        if (this._currentFrame >= this._animationMap[this._currentAnimation].Frames.Count)
        {
            this._currentFrame = 0;
        }
        return this._animationMap[this._currentAnimation].Frames[this._currentFrame];
    }

    public float CurrentFrameDuration()
    {
        float frameDuration = this._animationMap[this._currentAnimation].Frames[this._currentFrame].Duration;
        if (frameDuration == 0f)
        {
            frameDuration = this._animationMap[this._currentAnimation].DefaultDuration;
        }
        return (frameDuration / this._animationMap[this._currentAnimation].Speed);
    }

    public void SetTimeScale(float newTimeScale_)
    {

    }
}
