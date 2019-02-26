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

    public void Layer1 (){
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(fadeOutTime);
        layer3Off.TransitionTo(fadeOutTime);
    }
    public void Layer2()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3Off.TransitionTo(fadeOutTime);
    }
    public void Layer3()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2On.TransitionTo(fadeInTime);
        layer3On.TransitionTo(fadeInTime);
    }

    public void StopTheMusic()
    {
        layer1Off.TransitionTo(finalStopFadeTime);
        layer2Off.TransitionTo(finalStopFadeTime);
        layer3Off.TransitionTo(finalStopFadeTime);
    }

    void StartFirstLayer()
    {
        layer1On.TransitionTo(fadeInTime);
        layer2Off.TransitionTo(0);
        layer3Off.TransitionTo(0);

    }


}
