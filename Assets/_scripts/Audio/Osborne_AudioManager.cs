using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class Osborne_AudioManager : Singleton<Osborne_AudioManager>
{
    public AudioSource Layer1;
    public AudioSource Layer2;
    public AudioSource Layer3;

    public AudioMixerSnapshot MainSnapshot;
    public AudioMixerSnapshot FailSnapshot;
    public AudioMixerSnapshot SuccessSnapshot;

    public float FadeInTime;
    public float FadeOutTime;
    public float TransitionTime;

    private Dictionary<int, bool> _layers = new Dictionary<int, bool>();
    private Sequence _sequence;

    // Use this for initialization
    private void Start()
    {
        SetLayers(true, false, false, true);

        Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingCompleteHandler);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, LevelFailHandler);
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, LevelSuccessHandler);
    }

    private void LevelSuccessHandler(GameEvent arg0)
    {
        SetLayers(true, false, false);
        SuccessSnapshot.TransitionTo(TransitionTime);
    }

    private void LevelFailHandler(GameEvent arg0)
    {
        SetLayers(false, true, true);
        FailSnapshot.TransitionTo(TransitionTime);
    }

    //This could be much more dynamic based on the state of the game, but at the moment it's a very simple progression
    //where a completed building starts the next layer. 
    private void BuildingCompleteHandler(GameEvent arg0)
    {
        if (_layers[0] && !_layers[1])
        {
            SetLayers(false, true, false);
        }

        if (_layers[0] && _layers[1] && ! _layers[2])
        {
            SetLayers(true, true, true);
        }
    }

    public void SetLayers(bool layer1, bool layer2, bool layer3, bool immediate = false)
    {
        _layers[0] = layer1;
        _layers[1] = layer2;
        _layers[2] = layer3;

        UpdateAudio(immediate);
    }

    public void UpdateAudio(bool immediate = false)
    {
        _sequence?.Kill();
        if (!immediate)
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(Layer1.DOFade(_layers[0] ? 1 : 0, _layers[0] ? FadeInTime : FadeOutTime));
            _sequence.Append(Layer2.DOFade(_layers[1] ? 1 : 0, _layers[1] ? FadeInTime : FadeOutTime));
            _sequence.Append(Layer3.DOFade(_layers[2] ? 1 : 0, _layers[2] ? FadeInTime : FadeOutTime));
            _sequence.Play();
        }
        else
        {
            Layer1.volume = _layers[0] ? 1 : 0;
            Layer2.volume = _layers[1] ? 1 : 0;
            Layer3.volume = _layers[2] ? 1 : 0;
        }
    }

    public void SwitchLevels(AudioClip new1, AudioClip new2, AudioClip new3)
    {
        Layer1.volume = 0;
        Layer2.volume = 0;
        Layer3.volume = 0;

        Layer1.clip = new1;
        Layer2.clip = new2;
        Layer3.clip = new3;

        Layer1.Play();
        Layer2.Play();
        Layer3.Play();

        SetLayers(true, false, false, true);
        MainSnapshot.TransitionTo(1f);
    }
}
