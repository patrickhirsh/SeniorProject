using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndCanvas : MonoBehaviour
{
    public UnityEngine.UI.Image BlurImage;
    public UnityEngine.UI.Text VOFText;

    // Start is called before the first frame update
    void Start()
    {
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, OnSuccess);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, OnFailure);
        var tempColor = BlurImage.color;
        tempColor.a = 0f;
        BlurImage.color = tempColor;
    }

    private void SetText(string v)
    {
        VOFText.text = v;
    }

    private void OnFailure(GameEvent arg0)
    {
        GameManager.SetGameState(GameState.GameEndManu);
        SetText("Failure");
        FadeInBlurImage();
    }


    private void OnSuccess(GameEvent arg0)
    {
        GameManager.SetGameState(GameState.GameEndManu);
        SetText("Success!");
        FadeInBlurImage();
    }

    public void TurnOff()
    {
        BlurImage.enabled = false;
        VOFText.enabled = false;
        GameManager.SetGameState(GameState.LevelSimulating);
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
        VOFText.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
