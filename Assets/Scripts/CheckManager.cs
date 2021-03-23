using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{

    string guideText;
    string playerText;
    bool isRight;

    public bool CheckLine(string guideLine, string playerLine)
    {
        guideText = guideLine.ToString().ToLower();
        playerText = playerLine.ToString().ToLower();

        //if(guideText == playerText)
        if(guideText.Contains(playerText))
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }

        // 스페셜케이스 (발음이 같은 단어들)
        switch(playerText)
        {
            case "one":
                if(guideText.Contains("won") || guideText.Contains("1")) isRight = true;
                break;
            case "two":
                if(guideText.Contains("to") || guideText.Contains("2")) isRight = true;
                break;
            case "three":
                if(guideText.Contains("3")) isRight = true;
                break;
            case "four":
                if(guideText.Contains("for") || guideText.Contains("4")) isRight = true;
                break;
            case "five":
                if(guideText.Contains("5")) isRight = true;
                break;
            case "that is on the shelf number one":
                if(guideText.Contains("that's on the shelf number one")) isRight = true;
                if(guideText.Contains("that's on the shelf number 1")) isRight = true;
                break;
        }

        Debug.Log("guideText: " + guideText);
        Debug.Log("playerText: " + playerText);

        return isRight;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
