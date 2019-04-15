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
        Broadcaster.AddListener(GameEvent.LevelCompleteSuccess, OnSuccessParent);
        Broadcaster.AddListener(GameEvent.LevelCompleteFail, OnFailureParent);
        var tempColor = BlurImage.color;
        tempColor.a = 0f;
        BlurImage.color = tempColor;
    }

    private void OnSuccessParent(GameEvent arg0)
    {
        StartCoroutine(OnSuccess());
    }

    private void OnFailureParent(GameEvent arg0)
    {
        StartCoroutine(OnFailure());
    }

    private void SetText(string v)
    {
        VOFText.text = v;
    }

    private IEnumerator OnFailure()
    {
        yield return new WaitForSeconds(5);
        GameManager.SetGameState(GameState.GameEndManu);
        SetText("Failure");
        FadeInBlurImage();
    }


    private IEnumerator OnSuccess()
    {
        yield return new WaitForSeconds(5);
        GameManager.SetGameState(GameState.GameEndManu);
        SetText("Success!");
        FadeInBlurImage();
    }

    public void TurnOff()
    {
        BlurImage.enabled = false;
        VOFText.gameObject.SetActive(false);
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
        VOFText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
