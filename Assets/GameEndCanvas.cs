using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndCanvas : MonoBehaviour
{
    public UnityEngine.UI.Image BlurImage;

    // Start is called before the first frame update
    void Start()
    {
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, OnSuccess);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, OnFailure);
        var tempColor = BlurImage.color;
        tempColor.a = 0f;
        BlurImage.color = tempColor;
    }

    private void OnFailure(GameEvent arg0)
    {
        FadeInBlurImage();
    }


    private void OnSuccess(GameEvent arg0)
    {
        FadeInBlurImage();
    }

    private void FadeInBlurImage()
    {
        BlurImage.gameObject.SetActive(true);
        double av = 0;
        for(int i = 0; i < 4000; i++)
        {
            var tempColor = BlurImage.color;
            tempColor.a = 0; //1f makes it fully visible, 0f makes it fully transparent.
            BlurImage.color = tempColor;
            av += .0000025;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
