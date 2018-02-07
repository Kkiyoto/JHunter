using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public RectTransform back;
    bool fade = false;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(GameObject.Find("FadeCanvas"));
    }

    // Update is called once per frame
    void Update ()
    {
        if (fade)
        {
            back.position -= new Vector3(2, 0, 0);
            if (back.position.x < 50) back.position += new Vector3(100, 0, 0);
        }
	}

    public void SceneManage()//string scene)
    {
        fade = true;
        GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
    }
}
