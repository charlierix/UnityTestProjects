using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio
{
    #region class: Audio

    private class Audio
    {
        public AudioClip[] Clips { get; set; }

        public float Duration = -1f;        // nullable wasn't working

        public void PlayRandom(AudioSource source)
        {
            if (source == null || Clips == null || Clips.Length == 0)
            {
                return;
            }

            int index = UnityEngine.Random.Range(0, Clips.Length);

            source.PlayOneShot(Clips[index]);
        }
    }

    #endregion

    #region Declaration Section

    private readonly Dictionary<PlayerAudio_Action, Audio> _audio;
    private readonly AudioSource _source;

    private PlayerAudio_Action _action = PlayerAudio_Action.Nothing;
    private double _actionPlayed = 0d;

    private double _currentTime = 0d;

    #endregion

    #region Constructor

    public PlayerAudio(SoundDefinitions definitions, GameObject gameObject)
    {
        //TODO: Make a few with slightly randomized pitch
        _source = gameObject.AddComponent<AudioSource>();

        _audio = new Dictionary<PlayerAudio_Action, Audio>();

        _audio.Add(PlayerAudio_Action.Nothing, new Audio());      // this one doesn't have any meaning, but have something in case sloppy code requests an entry for this enum

        _audio.Add(PlayerAudio_Action.Walking, new Audio()
        {
            Clips = definitions.FootStep,
            Duration = .45f,
        });

        _audio.Add(PlayerAudio_Action.WingFlap, new Audio()
        {
            Clips = definitions.WingFlap,
            Duration = -1f,     // only play it when they actually flap the wings
        });
    }

    #endregion

    public void SetState(PlayerAudio_Action action)
    {
        if (_action != action)
        {
            if (_audio.TryGetValue(action, out Audio audio))
            {
                audio.PlayRandom(_source);
                _actionPlayed = _currentTime;
            }

            _action = action;
        }
    }

    public void Update(float elapsed)
    {
        _currentTime += elapsed;

        if (_audio.TryGetValue(_action, out Audio audio))
        {
            if (audio.Duration > 0f && _currentTime - _actionPlayed > audio.Duration)
            {
                audio.PlayRandom(_source);
                _actionPlayed = _currentTime;
            }
        }
    }
}

#region enum: PlayerAudio_Action

public enum PlayerAudio_Action
{
    Nothing,
    Walking,
    WingFlap,
}

#endregion
