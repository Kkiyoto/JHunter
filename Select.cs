using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Select : MonoBehaviour
{
    public Text[] ranks = new Text[5];
    public AudioClip SE;
    public AudioSource BGM;
    int[,] scores = new int[4, 3];
    string[] stage = { "Stage 10×10", "Stage Round", "Stage 15×15", "Stage Big" },state= { "小さめのマップ。目安4分", "円形のマップ。目安8分" , "中くらいのマップ。目安10分" , "宝ゲットで1分追加、壊すと1分減少" };
    int to_game;

	// Use this for initialization
	void Start ()
    {
        for(int i = 0; i < 4; i++)
        {
            for (int j=0; j < 3; j++)
            {
                scores[i, j] = PlayerPrefs.GetInt("theta_tre" + i + "_" + j,0);
            }
        }
        ChangeStage(0);
        #region 音楽
        float large = PlayerPrefs.GetFloat("SoundLargeP", 0.8f);
        if (large > 0)
        {
            BGM.volume = large;
            BGM.Play();
        }
        large = PlayerPrefs.GetFloat("SoundLargeP", 1f);
        GetComponent<AudioSource>().volume = large;
        #endregion
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void ChangeStage(int s)
    {
        to_game = s;
        for(int i = 0; i < 3; i++)
        {
            ranks[i].text = "No." + (i + 1) + "   " + scores[s, i].ToString().PadLeft(5, '0')+" Points";
        }
        ranks[3].text = stage[s];
        ranks[4].text = state[s];
        GetComponent<AudioSource>().PlayOneShot(SE);
    }
    public void GameStart(bool b)
    {
        if (b) SceneManager.LoadScene("Stage" + to_game);
        else SceneManager.LoadScene("Title");
    }
}
