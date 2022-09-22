using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Utilities;
using Moonvalk.Accessory;

public class TextureAnimator : MonoBehaviour
{
    [SerializeField]
    protected MeshRenderer _renderer;

    public TextureAnimationSet[] AnimationSets;
    public string DefaultSet = "Default";
    public string DefaultAnimation = "Default";
    public bool RandomizeFirstFrame = false;

    protected Dictionary<string, Dictionary<string, TextureAnimation>> _animations;
    protected string _currentAnimation = "";
    protected string _currentSet = "";
    protected InitValue<MicroTimer> _animationTimer;
    protected int _currentFrame = 0;

    public Dictionary<string, TextureAnimation> Animations
    {
        get
        {
            return this._animations[this._currentSet];
        }
    }

    private void Awake()
    {
        if (this.DefaultSet != "")
        {
            this._currentSet = this.DefaultSet;
        }

        // Establish a clone of the material selected for instances.
        this._renderer = GetComponent<MeshRenderer>();
        this._renderer.material = new Material(this._renderer.material);

        // Build a map for finding animation sets and their corresponding animations.
        this._animations = new Dictionary<string, Dictionary<string, TextureAnimation>>();
        foreach (TextureAnimationSet set in this.AnimationSets)
        {
            if (this._currentSet == "")
            {
                this._currentSet = set.Name;
            }
            Dictionary<string, TextureAnimation> newSet = new Dictionary<string, TextureAnimation>();
            foreach (TextureAnimation animation in set.Animations)
            {
                newSet.Add(animation.Name, animation);
            }
            this._animations.Add(set.Name, newSet);
        }

        this._animationTimer = new InitValue<MicroTimer>(this.initAnimationTimer);
    }

    private void Start()
    {
        if (DefaultAnimation != "") {
            this.Play(DefaultAnimation);
            if (RandomizeFirstFrame) {
                this._currentFrame = Random.Range(0, this.Animations[_currentAnimation].Frames.Count - 1);
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
            this._currentFrame = this.Animations[this._currentAnimation].Frames.Count - 1;
        }
        this.playNextFrame();
    }

    protected void playNextFrame()
    {
        TextureAnimation current = this.Animations[this._currentAnimation];
        TextureFrame frame = this.NextFrame();
        float frameDuration = this.CurrentFrameDuration();
        this._renderer.material.mainTexture = frame.Image;
        this._animationTimer.Value.Start(frameDuration);
    }

    public TextureFrame NextFrame()
    {
        this._currentFrame++;
        if (this._currentFrame >= this.Animations[this._currentAnimation].Frames.Count)
        {
            this._currentFrame = 0;
        }
        return this.Animations[this._currentAnimation].Frames[this._currentFrame];
    }

    public float CurrentFrameDuration()
    {
        float frameDuration = this.Animations[this._currentAnimation].Frames[this._currentFrame].Duration;
        if (frameDuration == 0f)
        {
            frameDuration = this.Animations[this._currentAnimation].DefaultDuration;
        }
        return (frameDuration / this.Animations[this._currentAnimation].Speed);
    }

    public void SetTimeScale(float newTimeScale_)
    {
        this._animationTimer.Value.SetTimeScale(newTimeScale_);
    }

    public void ChangeSet(string newSetName_)
    {
        if (this._currentSet == newSetName_)
        {
            return;
        }
        if (this._animations.ContainsKey(newSetName_))
        {
            this._currentSet = newSetName_;
        }   
    }

    public float GetAnimationDuration(string animation_)
    {
        if (!this.Animations.ContainsKey(animation_))
        {
            return 0f;
        }
        float duration = 0f;
        TextureAnimation request = this.Animations[animation_];
        foreach (TextureFrame frame in request.Frames)
        {
            float frameDuration = frame.Duration;
            if (frameDuration == 0f)
            {
                frameDuration = request.DefaultDuration;
            }
            duration += (frameDuration / request.Speed);
        }

        return duration;
    }
}
