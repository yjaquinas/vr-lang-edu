using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CPlayResultManager : MonoBehaviour {

    [SerializeField] GameObject _leftSentence;
    [SerializeField] GameObject _rightSentence;
    [SerializeField] Text _sentenceText;
    // Use this for initialization
    void Start() {
        //string[] myArray = new string[] { "aaaa", "bbbb", "cccc" };
        //setSentence(myArray, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    // 문장 
    public void setSentence(string[] stringArray, bool isLeft, bool isClear, int playerSentence)
    {
        GameObject tempGameObject;

        if (isLeft)
            tempGameObject = _leftSentence;
        else
            tempGameObject = _rightSentence;

        for (int i = 0; i < stringArray.Length; i++)
        {
            _sentenceText.text = (stringArray[i]);
            Text textObject = Instantiate(_sentenceText, Vector3.zero, Quaternion.identity);
            textObject.transform.SetParent(tempGameObject.transform);
            textObject.transform.localPosition = new Vector3(0f, i * -35f, 0f);
            textObject.transform.localRotation = Quaternion.identity;
            textObject.rectTransform.anchoredPosition = new Vector3(0f, i * -35f, 0f);
            textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            if(playerSentence == i)
            {
                if (isClear)
                    textObject.color = Color.blue;
                else
                    textObject.color = Color.red;
            }
        }
    }
    
}
