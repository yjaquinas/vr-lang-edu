using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayer : MonoBehaviour {

	public int playerLevel = 0;
	public int curExp = 0;
	public int needExp = 30;

	public void AddExp(int getExp)
	{
		curExp += getExp;
		if(curExp >= needExp)
		{
			playerLevel++;
			curExp = 0;
		}
		print("플레이어 레벨: " + playerLevel + " 플레이어 경험치" + curExp);
	}

	private void OnDisable()
	{
		PlayerPrefs.SetInt("userLevel", playerLevel);
		PlayerPrefs.SetInt("userExp", curExp);
	}

	// Use this for initialization
	void Start () {
		playerLevel = PlayerPrefs.GetInt("userLevel", 0);
		curExp = PlayerPrefs.GetInt("userExp", 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			curExp += 10;
			if (curExp >= needExp)
			{
				playerLevel++;
				curExp = 0;
			}
			print(playerLevel + " " + curExp);
		}
	}
}
