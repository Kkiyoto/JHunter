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
            rule = false;
        }
        else
        {
            foreach (Image img in Vanishs) img.color = Color.white;
            foreach (Image img in Rules) img.color = Color.clear;
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
        if (rule) SceneManager.LoadScene("Select");
        else RuleOpen(false);
    }
}
