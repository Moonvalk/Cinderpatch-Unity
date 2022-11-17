using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonvalk.Animation;
using UnityEngine.Audio;

public class AudioFader : MonoBehaviour
{
    public AudioClip[] Playlist;
    public AudioSource Sound;
    public float StartVolume = 0f;
    public float TargetVolume = 1f;
    public float FadeDuration = 4f;

    protected Tween _soundTween;
    protected float _soundVolume;
    protected static int _currentPlaylistIndex = -1;
    protected bool _isPlaying = false;

    private void Start()
    {
        this.Sound.volume = this.StartVolume;
        this._soundVolume = this.StartVolume;
        this._soundTween = new Tween(() => ref this._soundVolume);
        this._soundTween.Duration(this.FadeDuration).OnUpdate(() => {
            this.Sound.volume = this._soundVolume;
        });
    }

    private void FixedUpdate()
    {
        if (!this.Sound.isPlaying && this._isPlaying)
        {
            AudioFader._currentPlaylistIndex++;
            if (AudioFader._currentPlaylistIndex >= this.Playlist.Length)
            {
                AudioFader._currentPlaylistIndex = 0;
            }
            this.Sound.clip = this.Playlist[AudioFader._currentPlaylistIndex];
            this.Sound.Play();
        }
    }

    public void Play()
    {
        if (!this._isPlaying)
        {
            this._isPlaying = true;
            AudioFader._currentPlaylistIndex++;
            if (AudioFader._currentPlaylistIndex >= this.Playlist.Length)
            {
                AudioFader._currentPlaylistIndex = 0;
            }
            this.Sound.clip = this.Playlist[AudioFader._currentPlaylistIndex];
            this.Sound.Play();
            this.FadeIn();
        }
    }

    public void FadeIn()
    {
        this._soundTween.To(this.TargetVolume).Start();
    }

    public void FadeOut()
    {
        this._soundTween.To(this.StartVolume).Start();
    }
}
