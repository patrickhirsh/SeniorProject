using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Osborne_AudioManager : Singleton<Osborne_AudioManager> {

    public AudioMixerSnapshot layer1On;
    public AudioMixerSnapshot layer1Off;
    public AudioMixerSnapshot layer2On;
    public AudioMixerSnapshot layer2Off;
    public AudioMixerSnapshot layer3On;
    public AudioMixerSnapshot layer3Off;

    public float fadeInTime;
    public float fadeOutTime;
    public float finalStopFadeTime;

    private enum ActiveLayer { l1, l2, l3, l0, l12, l13, l123, l23};

    private ActiveLayer al;

    // Use this for initialization
    void Start ()
    {
        StartFirstLayer();
        Broadcaster.AddListener(GameEvent.BuildingComplete, BuildingCompleteHandler);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, LevelFailHandler);
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, LevelSuccessHandler);
    }

    private void LevelSuccessHandler(GameEvent arg0)
    {
        Layer23();
    }

    private void LevelFailHandler(GameEvent arg0)
    {
        Layer1();
    }

    //This could be much more dynamic based on the state of the game, but at the moment it's a very simple progression
    //where a completed building starts the next layer. 
    private void BuildingCompleteHandler(GameEvent arg0)
    {
        switch (al)
        {
            case ActiveLayer.l1:
                Layer12();
                break;
            case ActiveLayer.l2:
                //Don't know why we'd use this but made it possible anyways
                break;
            case ActiveLayer.l3:
                //Don't know why we'd use this but made it possible anyways
                break;
            case ActiveLayer.l12:
                Layer123();
                break;
            case ActiveLayer.l13:
                //Don't know why we'd use this but made it possible anyways
                break;
            case ActiveLayer.l123:
                //Do nothing here, already at max vol
                break;
            case ActiveLayer.l23:
                //Don't know why we'd use this but made it possible anyways
                break;
            case ActiveLayer.l0:
                //Don't know why we'd use this but made it possible anyways
                break;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
    //Turn Layer 1 ONLY on
    public void Layer1 (){
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeOutTime);
        layer3Off.TransitionTo(fadeOutTime);
        al = ActiveLayer.l1;
    }
    //Turn Layer 2 ONLY on
    public void Layer2()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3Off.TransitionTo(fadeInTime);
        al = ActiveLayer.l2;
    }
    //Turn Layer 3 ONLY on
    public void Layer3()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
        al = ActiveLayer.l3;
    }
    //Turn Layers 1 & 2 on
    public void Layer12()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3Off.TransitionTo(fadeOutTime);
        al = ActiveLayer.l12;
    }
    //Turn Layers 1 & 3 On
    public void Layer13()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
        al = ActiveLayer.l13;
    }
    //Turn Layers 2 & 3 On
    public void Layer23()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
        al = ActiveLayer.l23;
    }
    //Turn Layers 1, 2, and 3 on
    public void Layer123()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
        al = ActiveLayer.l123;
    }
    //Turn all layers off
    public void StopTheMusic()
    {
        layer1Off.TransitionTo(finalStopFadeTime);
        layer2Off.TransitionTo(finalStopFadeTime);
        layer3Off.TransitionTo(finalStopFadeTime);
        al = ActiveLayer.l0;
    }
    //Initial function, turning layer 1 on and turning layers 2 and 3 to off with 0 second delay
    void StartFirstLayer()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(0);
        layer3Off.TransitionTo(0);
        al = ActiveLayer.l1;
    }

    public void SwitchLevels(AudioClip new1, AudioClip new2, AudioClip new3)
    {
        AudioSource[] ASList = this.gameObject.GetComponentsInChildren<AudioSource>();
        foreach(AudioSource x in ASList)
        {
            switch (x.name)
            {
                case "Layer1":
                    x.clip = new1;
                    break;
                case "Layer2":
                    x.clip = new2;
                    break;
                case "Layer3":
                    x.clip = new3;
                    break;
                
            }
            x.Play();
        }
        StartFirstLayer();
    }


}
