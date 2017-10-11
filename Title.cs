using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public Image[] Vanishs, Rules;
    public Sprite[] imgs=new Sprite[4];
    public AudioClip SE;
    public AudioSource BGM;
    public Text music;
    bool rule;
    int large;

	// Use this for initialization
	void Start ()
    {
        rule = true;
        large = PlayerPrefs.GetInt("SoundLarge", 1);
        Vanishs[0].sprite = imgs[large];
        if (large == 0) { BGM.volume = 0f; GetComponent<AudioSource>().volume = 0; }
        else if (large == 1) { BGM.volume = 1f; GetComponent<AudioSource>().volume = 1; }
        else if (large == 2) BGM.volume = 0.5f;
        else if (large == 3) BGM.volume = 0.2f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RuleOpen(bool b)
    {
        if (b&&rule)
        {
            foreach (Image img in Vanishs) img.color = Color.clear;
            foreach (Image img in Rules) img.color = Color.white;
            music.color = Color.clear;
            rule = false;
        }
        else
        {
            foreach (Image img in Vanishs) img.color = Color.white;
            foreach (Image img in Rules) img.color = Color.clear;
            music.color = new Color(0.6f, 0.8f, 0.95f);
            rule = true;
        }
        GetComponent<AudioSource>().PlayOneShot(SE);
    }
    public void Sound()
    {
        if (rule)
        {
            GetComponent<AudioSource>().PlayOneShot(SE);
            large = (large + 1) % 4;
            Vanishs[0].sprite = imgs[large];
            if (large == 0) { BGM.volume = 0f; GetComponent<AudioSource>().volume = 0; }
            else if (large == 1) { BGM.volume = 1f; GetComponent<AudioSource>().volume = 1; }
            else if (large == 2) BGM.volume = 0.5f;
            else if (large == 3) BGM.volume = 0.2f;
        }
        else RuleOpen(false);
    }
    public void ToSelect()
    {
        if (rule)
        {
            SceneManager.LoadScene("Select");
            PlayerPrefs.SetInt("SoundLarge", large);
            if (large == 1) { PlayerPrefs.SetFloat("SoundLargeC", 0.8f); PlayerPrefs.SetFloat("SoundLargeP", 1f); }
            else if (large == 2) { PlayerPrefs.SetFloat("SoundLargeC", 0.3f); PlayerPrefs.SetFloat("SoundLargeP", 0.4f); }
            else if (large == 3) { PlayerPrefs.SetFloat("SoundLargeC", 0.1f); PlayerPrefs.SetFloat("SoundLargeP", 0.12f); }
            else { PlayerPrefs.SetFloat("SoundLargeC", 0f); PlayerPrefs.SetFloat("SoundLargeP", 0); }
        }
        else RuleOpen(false);
    }
}
