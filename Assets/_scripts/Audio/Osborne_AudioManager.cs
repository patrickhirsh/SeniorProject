using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Osborne_AudioManager : MonoBehaviour {

    public AudioMixerSnapshot layer1On;
    public AudioMixerSnapshot layer1Off;
    public AudioMixerSnapshot layer2On;
    public AudioMixerSnapshot layer2Off;
    public AudioMixerSnapshot layer3On;
    public AudioMixerSnapshot layer3Off;

    public float fadeInTime;
    public float fadeOutTime;
    public float finalStopFadeTime;

    // Use this for initialization
    void Start ()
    {
        StartFirstLayer();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //Turn Layer 1 ONLY on
    public void Layer1 (){
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeOutTime);
        layer3Off.TransitionTo(fadeOutTime);
    }
    //Turn Layer 2 ONLY on
    public void Layer2()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3Off.TransitionTo(fadeInTime);
    }
    //Turn Layer 3 ONLY on
    public void Layer3()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
    }
    //Turn Layers 1 & 2 on
    public void Layer12()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3Off.TransitionTo(fadeOutTime);
    }
    //Turn Layers 1 & 3 On
    public void Layer13()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
    }
    //Turn Layers 2 & 3 On
    public void Layer23()
    {
        layer1Off.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
    }
    //Turn Layers 1, 2, and 3 on
    public void Layer123()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
    }
    //Turn all layers off
    public void StopTheMusic()
    {
        layer1Off.TransitionTo(finalStopFadeTime);
        layer2Off.TransitionTo(finalStopFadeTime);
        layer3Off.TransitionTo(finalStopFadeTime);
    }
    //Initial function, turning layer 1 on and turning layers 2 and 3 to off with 0 second delay
    void StartFirstLayer()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(0);
        layer3Off.TransitionTo(0);
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
        }
        Layer1();
    }


}
