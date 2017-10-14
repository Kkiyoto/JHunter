using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage10 : Function
{
    public GameObject fieldPre ,player;
    public Sprite[] imgs = new Sprite[14],arrow=new Sprite[2];
    public Text time_text,state_text,score_text,leave_text;
    public Image[] arrow_img=new Image[4];
    public AudioClip[] SEs = new AudioClip[3],sound=new AudioClip[6];
    public AudioSource BGM;
    Common.Move state;
    Common.Direct direct;
    Field[,] fields = new Field[12, 12];
    int[] pos = new int[2];//,front=new int[2];
    float time,width,height,real=0;
    int score = 0,count=0,leave=20;

	// Use this for initialization
	void Start ()
    {
        state = Common.Move.Wait;
        time = 1501;
        width = Screen.width;
        height = Screen.height;
        #region 音楽
        float large = PlayerPrefs.GetFloat("SoundLargeP", 0.8f);
        if (large > 0)
        {
            int ran = Random.Range(0, 6);
            BGM.clip = sound[ran];
            BGM.volume = large;
            BGM.Play();
        }
        large = PlayerPrefs.GetFloat("SoundLargeP", 1f);
        GetComponent<AudioSource>().volume = large;
        #endregion
        #region Fieldsの設定
        for (int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                GameObject o = Instantiate(fieldPre) as GameObject;
                fields[i, j] = new Field(o,i,j);
                if (Mathf.Max(Mathf.Abs(5.5f - i), Mathf.Abs(5.5f - j)) > 5)//石の部分
                {
                    fields[i, j].Around = 12;
                    fields[i, j].img = imgs[12];
                }
            }
        }
        for(int i = 0; i < 20; i++)//宝埋め
        {
            int k = Random.Range(1, 11);
            int l = Random.Range(1, 11);
            while (fields[k, l].Around == 9)
            {
                k = Random.Range(1, 11);
                l = Random.Range(1, 11);
            }
            fields[k, l].Around = 9;
        }
        for (int i = 1; i < 11; i++)//周りの数設定
        {
            for (int j = 1; j < 11; j++)
            {
                if (fields[i, j].Around != 9)
                {
                    int count = 0;
                    for (int k = -1; k < 2; k++) for (int l = -1; l < 2; l++) if (fields[i + k, j + l].Around == 9) count++;
                    fields[i, j].Around = count;
                }
            }
        }
        #endregion
        #region Playerの設定
        /*
        int x = Random.Range(1, 11);
        int y = Random.Range(1, 11);
        while (fields[x, y].Around == 9)
        {
            x = Random.Range(1, 11);
            y = Random.Range(1, 11);
        }*/
        int x = Random.Range(1, 10);
        int y = Random.Range(1, 10);
        while (fields[x, y].Around == 9|| fields[x+1, y].Around == 9|| fields[x, y+1].Around == 9|| fields[x+1, y+1].Around == 9)
        {
            x = Random.Range(1, 10);
            y = Random.Range(1, 10);
        }
        pos[0] = x; pos[1] = y;
        //front[0] = x + 1;front[1] = y;
        direct = Common.Direct.Right;
        player.transform.position = new Vector3(x, y, 0);
        SetImg(x, y, -1);
        SetImg(x+1, y, -1);
        SetImg(x, y+1, -1);
        SetImg(x+1, y+1, -1);
        state_text.text = "Game Start! 周りには" + fields[x, y].Around + "個埋まっています";
        fields[x, y].Into = true;
        fields[x+1, y].Into = true;
        fields[x, y+1].Into = true;
        fields[x+1, y+1].Into = true;
        #endregion
    }

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log("pos[0], pos[1]= " + pos[0] + "  " + pos[1]);
        #region 時間
        if (state != Common.Move.End)
        {
            time -= Time.deltaTime;
            if (time < 0) time = 0;
            int m = Mathf.FloorToInt(time / 60), s = Mathf.FloorToInt(time % 60);
            time_text.text = "Time " + m.ToString().PadLeft(2, '0') + ":" + s.ToString().PadLeft(2, '0');
            real += Time.deltaTime;
        }
        #endregion
        #region swich (state)
        switch (state)
        {
            case Common.Move.Wait:
                Waiting();
                break;
            case Common.Move.Walk:
                Walking();
                break;
            case Common.Move.Dig:
                Digging();
                break;
            case Common.Move.Get:
                Getting();
                break;
            case Common.Move.End:
                Endding();
                break;
        }
        #endregion
        #region カメラ
        if (state != Common.Move.End)
        {
            Vector3 vec = player.transform.position + new Vector3(0, -1.5f, -10);
            vec.x = Mathf.Max(3, Mathf.Min(8, vec.x));
            vec.y = Mathf.Max(1.3f, Mathf.Min(6.3f, vec.y));
            transform.position = vec;
        }
        #endregion
        #region ボタン
        if (Input.GetMouseButton(0))
        {
            Vector3 tap = Input.mousePosition;
            tap.x = tap.x * 3 / width;tap.y = tap.y * 6.4f / height;
            if (tap.y < 1)
            {
                if (tap.x < 1) ArrowButton(Common.Direct.Left);
                else if (tap.x < 2) ArrowButton(Common.Direct.Down);
                else ArrowButton(Common.Direct.Right);
            }
            else if (tap.y < 2 && tap.x > 1 && tap.x < 2) ArrowButton(Common.Direct.Up);
            else ArrowUp();
        }
        if (Input.GetMouseButtonUp(0)) ArrowUp();
        #endregion
    }

    void Waiting()
    {
        if (time <= 0) ToEndding("Time Over...　青：入手　赤：破壊");
    }
    void Walking()
    {
        int[] front = DireToVec(direct);
        player.transform.Translate(new Vector3(front[0], front[1]) / 25f);
        count++;
        if(count>25)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            if(fields[pos[0], pos[1]].Around==9) state_text.text = "足元に宝石があります";
            else state_text.text = "周りには " + fields[pos[0], pos[1]].Around + " 個埋まっています";
            count = 0;
        }
    } 
    void Digging()
    {
        int[] front = DireToVec(direct);
        player.transform.Translate(new Vector3(front[0], front[1]) / 30f);
        count++;
        if (count>30)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            SetImg(pos[0], pos[1], -1);
            time -= 10;
            if (fields[pos[0], pos[1]].Around == 9)
            {
                SetImg(pos[0], pos[1], 10);
                score -= 500;
                GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Out]);
                leave--;
                if (leave == 0) ToEndding("探索終了！　青：入手　赤：破壊");
                else leave_text.text = "残り: " + leave + " 個";
                fields[pos[0], pos[1]].state = Common.State.Out;
            }
            else score += 10;
            score_text.text = "Score:" + score;
            if (fields[pos[0], pos[1]].Around == 9) state_text.text = "足元に宝石があります";
            else state_text.text = "周りには " + fields[pos[0], pos[1]].Around + " 個埋まっています";
            count = 0;
        }
    }
    void Getting()
    {
        count++;
        if (count > 45)
        {
            state = Common.Move.Wait;
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            int[] front = DireToVec(direct);
            front[0] += pos[0]; front[1] += pos[1];
            SetImg(front[0], front[1], -1);
            time -= 60;
            if (fields[front[0], front[1]].Around == 9)
            {
                score += 500;
                fields[front[0], front[1]].state = Common.State.Treasure;
                leave--;
                GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Treasure]);
                if (leave == 0) ToEndding("探索終了！　青：入手　赤：破壊");
                else leave_text.text = "残り: " + leave+" 個";
            }
            else score -= 50;
            score_text.text = "Score:" + score;
            count = 0;
        }
    }
    void Endding()
    {
        if (count < 40)
        {
            Vector3 vec = transform.position;
            vec= (vec * 19f + new Vector3(5.5f, 2f, -10f)) / 20f;
            transform.position = vec;
            float scale = GetComponent<Camera>().orthographicSize;
            GetComponent<Camera>().orthographicSize = (19f * scale + 10f) / 20f;
            if (Mathf.Abs(10 - scale) < 0.1f && (vec - new Vector3(5.5f, 2f, -10)).magnitude < 0.1f)
            {
                transform.position = new Vector3(5.5f, 2f, -10);
                GetComponent<Camera>().orthographicSize = 10;
                count++;
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        fields[i, j].End( new Vector3(1f / (float)count, 1f / (float)count),Color.white);
                        }
                }
                player.transform.localScale = new Vector3(1f / (float)count, 1f / (float)count);
            }
        }
        else if (count < 80)
        {
            Color[] col = {new Color(0.9f, 0.65f, 0),new Color(0,0.5f,1),  Color.red, new Color(0.6f, 0.25f, 0), new Color(0.7f,0.7f,0.7f) };
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    fields[i, j].img = imgs[13];
                    fields[i, j].End( new Vector3(1f / (80f-(float)count), 1f / (80f-(float)count)),col[(int)fields[i,j].state]);
                }
            }
            player.transform.localScale = new Vector3(1f / (80f - (float)count), 1f / (80f - (float)count));
            count++;
            player.GetComponent<Animator>().SetTrigger("End_Trigger");
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                int no1, no2, no3;
                no1 = PlayerPrefs.GetInt("theta_tre0_0", 0);
                no2 = PlayerPrefs.GetInt("theta_tre0_1", 0);
                no3 = PlayerPrefs.GetInt("theta_tre0_2", 0);
                if (score > no1)
                {
                    PlayerPrefs.SetInt("theta_tre0_0", score);
                    PlayerPrefs.SetInt("theta_tre0_1", no1);
                    PlayerPrefs.SetInt("theta_tre0_2", no2);
                }
                else if (score > no2)
                {
                    PlayerPrefs.SetInt("theta_tre0_1", score);
                    PlayerPrefs.SetInt("theta_tre0_2", no2);
                }
                else if (score > no3) PlayerPrefs.SetInt("theta_tre0_2", score);
                SceneManager.LoadScene("Select");
            }
        }
    }

    void SetImg(int x, int y, int target)
    {
        if (target == -1) fields[x, y].img = imgs[fields[x, y].Around];
        else fields[x, y].img = imgs[target];
    }

    public void ArrowButton(Common.Direct direction)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == (int)direction) arrow_img[i].sprite = arrow[1];
            else arrow_img[i].sprite = arrow[0];
        }
        if (state == Common.Move.Wait)
        {
            int[] front = DireToVec(direction);
            direct = direction;
            player.GetComponent<Animator>().SetInteger("Direct_Int", (int)direction);
            if (fields[pos[0]+front[0], pos[1]+front[1]].Into)
            {
                pos[0] += front[0]; pos[1] += front[1];
                state = Common.Move.Walk;
                player.GetComponent<Animator>().SetInteger("Move_Int", 1);
            }
        }
    }
    public void ArrowUp()
    {
        arrow_img[0].sprite = arrow[0];
        arrow_img[1].sprite = arrow[0];
        arrow_img[2].sprite = arrow[0];
        arrow_img[3].sprite = arrow[0];
    }

    public void Dig_Button()
    {
        int[] front = DireToVec(direct);
        if (state == Common.Move.Wait&& fields[pos[0]+front[0], pos[1]+front[1]].state==Common.State.Pre)
        {
            pos[0] += front[0]; pos[1] += front[1];
            state = Common.Move.Dig;
            player.GetComponent<Animator>().SetInteger("Move_Int", 2);
            SetImg(pos[0], pos[1], 0);
            fields[pos[0], pos[1]].Into = true;
            GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Dug]);
        }
    }
    public void Get_Button()
    {
        int[] front = DireToVec(direct);
        front[0] += pos[0]; front[1] += pos[1];
        if (state == Common.Move.Wait && fields[front[0], front[1]].state == Common.State.Pre)
        {
            state = Common.Move.Get;
            player.GetComponent<Animator>().SetInteger("Move_Int", 2);
            SetImg(front[0], front[1], 0);
            GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Dug]);
            fields[front[0], front[1]].Into = true;
        }
    }

    void ToEndding(string s)
    {
        state_text.text = s;
        state = Common.Move.End;
        GameObject o = GameObject.Find("Score");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, -50);
        score_text.fontSize = 75;
        score += Mathf.RoundToInt(time * 5);
        score_text.text = "Score:" + score;
        o = GameObject.Find("Time");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, -145);
        time_text.fontSize = 35;
        int minute = Mathf.FloorToInt(real / 60), second = Mathf.FloorToInt(real % 60);
        time_text.text = "探索時間 " + minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0') + "     残り " + leave + " 個";
        o = GameObject.Find("Leave");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 70);
        leave_text.fontSize = 48;
        int maru = 0,batu = 0,length=0;
        for(int i = 1; i < 11; i++)
        {
            for(int j = 1; j < 11; j++)
            {
                if (fields[i, j].Into)
                {
                    length++;
                    if (fields[i, j].state == Common.State.Treasure) maru++;
                    else if (fields[i, j].state == Common.State.Out) batu++;
                }
            }
        }
        leave_text.text = "入手数 : " + maru + "  破壊数 : " + batu+"\n道は " +length+" マス掘りました";
        GameObject.Find("Treasure").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        GameObject.Find("Dig").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        for (int i = 0; i < 4; i++) arrow_img[i].color = Color.clear;
    }
}
