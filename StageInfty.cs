using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageInfty : Function
{
    public GameObject fieldPre, player;
    public RectTransform Pause;
    public Sprite[] imgs = new Sprite[14], arrow = new Sprite[2];
    public Text time_text, state_text, score_text, leave_text,pause_text;
    public Image[] arrow_img = new Image[4];
    public AudioClip[] SEs = new AudioClip[4], sound = new AudioClip[6];
    public AudioSource BGM;
    Common.Move state;
    Common.Direct direct;
    int[,] nums = new int[53, 53];
    Common.State[,] field = new Common.State[53, 53];
    GameObject[,] objs = new GameObject[9,9];
    int[] pos = { 26, 26 };//,front=new int[2];
    float time, width, height, real = 0;
    int score = 0, count = 0, leave = 0;
    bool pause = true;

    // Use this for initialization
    void Start()
    {
        state = Common.Move.Wait;
        time = 301;
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
        for (int i = 0; i < 53; i++)
        {
            for (int j = 0; j < 53; j++)
            {
                if (i == 0 || j == 0 || i == 52 || j == 52) { nums[i, j] = 12; field[i, j] = Common.State.Rock; }//石の部分
                else
                {
                    int ran = Random.Range(0, 5);
                    if (ran == 0) nums[i, j] = 9;
                    else nums[i, j] = 0;
                    field[i, j] = Common.State.Pre;
                }
            }
        }
        nums[26, 26] = 0;
        for (int i = 1; i < 52; i++)//周りの数設定
        {
            for (int j = 1; j < 52; j++)
            {
                if (nums[i, j] != 9)
                {
                    int count = 0;
                    for (int k = -1; k < 2; k++) for (int l = -1; l < 2; l++) if (nums[i + k, j + l] == 9) count++;
                    nums[i,j] = count;
                }
            }
        }
        for (int i = 22; i < 31; i++)//見えてるところ
        {
            for (int j = 22; j < 31; j++)
            {
                objs[i%9, j%9] = Instantiate(fieldPre) as GameObject;
                objs[i%9, j%9].transform.position = new Vector3(i,j);
                objs[i % 9, j % 9].name = "obj_" + (i % 9) + "_" + (j % 9);
            }
        }
        #endregion
        #region Playerの設定
        direct = Common.Direct.Right;
        //player.transform.position = new Vector3(26,26, 0);
        state_text.text = "Game Start! 周りには" + nums[26,26] + "個埋まっています";
        field[26, 26] = Common.State.Dug;
        SetImg(26,26, -1);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (pause)
        {
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
                vec.x = Mathf.Max(3.2f, Mathf.Min(48.8f, vec.x));
                vec.y = Mathf.Max(1.5f, Mathf.Min(47.1f, vec.y));
                transform.position = vec;
            }
            #endregion
            #region ボタン
            if (Input.GetMouseButton(0))
            {
                Vector3 tap = Input.mousePosition;
                tap.x = tap.x * 3 / width; tap.y = tap.y * 6.4f / height;
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
        if (count > 25)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            if (nums[pos[0], pos[1]] == 9) state_text.text = "足元に宝石があります";
            else state_text.text = "周りには " + nums[pos[0], pos[1]] + " 個埋まっています";
            count = 0;
        }
    }
    void Digging()
    {
        int[] front = DireToVec(direct);
        player.transform.Translate(new Vector3(front[0], front[1]) / 30f);
        count++;
        if (count > 30)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            SetImg(pos[0], pos[1], -1);
            time -= 10;
            if (nums[pos[0], pos[1]] == 9)
            {
                SetImg(pos[0], pos[1], 10);
                score -= 500;
                GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Out]);
                time -= 50;
                field[pos[0], pos[1]] = Common.State.Out;
            }
            else score += 10;
            score_text.text = "Score:" + score;
            if (nums[pos[0], pos[1]] == 9) state_text.text = "足元に宝石があります";
            else state_text.text = "周りには " + nums[pos[0], pos[1]] + " 個埋まっています";
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
            if (nums[front[0], front[1]] == 9)
            {
                score += 500;
                field[front[0], front[1]] = Common.State.Treasure;
                leave++;
                GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Treasure]);
                leave_text.text = "入手: " + leave + " 個";
                time += 120;
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
            vec = (vec * 19f + new Vector3(width, height, -10f)) / 20f;
            transform.position = vec;
            float scale = GetComponent<Camera>().orthographicSize;
            GetComponent<Camera>().orthographicSize = (19f * scale + nums[0, 0]) / 20f;
            count++;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    objs[i, j].transform.localScale = new Vector3(1f / (float)count, 1f / (float)count);
                }
            }
            player.transform.localScale = new Vector3(1f / (float)count, 1f / (float)count);
        }
        else if (count < 80)
        {
            transform.position = new Vector3(width, height, -10);
            GetComponent<Camera>().orthographicSize = nums[0, 0];
            Color[] col = { new Color(0.9f, 0.65f, 0), new Color(0, 0.5f, 1), Color.red, new Color(0.6f, 0.25f, 0), new Color(0.7f, 0.7f, 0.7f) };
            foreach (GameObject o in objs) Destroy(o);
            Debug.Log(nums[0, 1] + "   " + nums[0, 2]+"   ,   " + nums[0, 3] + "   " + nums[0, 4]);
            for (int i = nums[0,1]; i < nums[0,2]; i++)
            {
                for (int j = nums[0,3]; j < nums[0,4]; j++)
                {
                    if (field[i, j] != Common.State.Pre)
                    {
                        GameObject o = Instantiate(fieldPre) as GameObject;
                        o.transform.position = new Vector3(i, j);
                        o.GetComponent<SpriteRenderer>().sprite = imgs[13];
                        o.GetComponent<SpriteRenderer>().color = col[(int)field[i, j]];
                    }
                }
            }
            player.transform.localScale = new Vector3(1,1,1);
            player.GetComponent<Animator>().SetTrigger("End_Trigger");
            count = 100;
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                int no1, no2, no3;
                no1 = PlayerPrefs.GetInt("theta_tre3_0", 0);
                no2 = PlayerPrefs.GetInt("theta_tre3_1", 0);
                no3 = PlayerPrefs.GetInt("theta_tre3_2", 0);
                if (score > no1)
                {
                    PlayerPrefs.SetInt("theta_tre3_0", score);
                    PlayerPrefs.SetInt("theta_tre3_1", no1);
                    PlayerPrefs.SetInt("theta_tre3_2", no2);
                }
                else if (score > no2)
                {
                    PlayerPrefs.SetInt("theta_tre3_1", score);
                    PlayerPrefs.SetInt("theta_tre3_2", no2);
                }
                else if(score>no3) PlayerPrefs.SetInt("theta_tre3_2", score);
                SceneManager.LoadScene("Select");
            }
        }
    }

    void SetImg(int x, int y, int target)
    {
        if (target == -1)
        {
            if (field[x, y] == Common.State.Pre) objs[x % 9, y % 9].GetComponent<SpriteRenderer>().sprite = imgs[11];
            else objs[x % 9, y % 9].GetComponent<SpriteRenderer>().sprite = imgs[nums[x, y]];
        }
        else objs[x%9, y%9].GetComponent<SpriteRenderer>().sprite = imgs[target];
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
            if (field[pos[0] + front[0], pos[1] + front[1]]!=Common.State.Pre&& field[pos[0] + front[0], pos[1] + front[1]] != Common.State.Rock)
            {
                pos[0] += front[0]; pos[1] += front[1];
                state = Common.Move.Walk;
                if (direct == Common.Direct.Up) for (int i = -4; i < 5; i++) { objs[(pos[0] + i) % 9, (pos[1] + 4) % 9].transform.position = new Vector3(pos[0] + i, pos[1] + 4); SetImg(pos[0] + i, pos[1] + 4, -1); }
                else if (direct == Common.Direct.Right) for (int i = -4; i < 5; i++) { objs[(pos[0] + 4) % 9, (pos[1] + i) % 9].transform.position = new Vector3(pos[0] + 4, pos[1] + i, 0); SetImg(pos[0] + 4, pos[1] + i, -1); }
                else if (direct == Common.Direct.Down) for (int i = -4; i < 5; i++) { objs[(pos[0] + i) % 9, (pos[1] - 4) % 9].transform.position = new Vector3(pos[0] + i, pos[1] - 4); SetImg(pos[0] + i, pos[1] - 4, -1); }
                else for (int i = -4; i < 5; i++) { objs[(pos[0] - 4) % 9, (pos[1] + i) % 9].transform.position = new Vector3(pos[0] - 4, pos[1] + i); SetImg(pos[0] - 4, pos[1] + i, -1); }
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
        if (state == Common.Move.Wait && field[pos[0] + front[0], pos[1] + front[1]] == Common.State.Pre)
        {
            pos[0] += front[0]; pos[1] += front[1];
            state = Common.Move.Dig;
            player.GetComponent<Animator>().SetInteger("Move_Int", 2);
            SetImg(pos[0], pos[1], 0);
            if (direct == Common.Direct.Up) for (int i = -4; i < 5; i++) { try { objs[(pos[0] + i) % 9, (pos[1] + 4) % 9].transform.position = new Vector3(pos[0] + i, pos[1] + 4); } finally { SetImg(pos[0] + i, pos[1] + 4, -1); } }
            else if (direct == Common.Direct.Right) for (int i = -4; i < 5; i++) { try { objs[(pos[0] + 4) % 9, (pos[1] + i) % 9].transform.position = new Vector3(pos[0] + 4, pos[1] + i, 0); } finally { SetImg(pos[0] + 4, pos[1] + i, -1); } }
            else if (direct == Common.Direct.Down) for (int i = -4; i < 5; i++) { try { objs[(pos[0] + i) % 9, (pos[1] - 4) % 9].transform.position = new Vector3(pos[0] + i, pos[1] - 4); } finally { SetImg(pos[0] + i, pos[1] - 4, -1); } }
            else for (int i = -4; i < 5; i++) { try { objs[(pos[0] - 4) % 9, (pos[1] + i) % 9].transform.position = new Vector3(pos[0] - 4, pos[1] + i); } finally{ SetImg(pos[0] - 4, pos[1] + i, -1); } }
            GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Dug]);
            field[pos[0], pos[1]] = Common.State.Dug;
        }
    }
    public void Get_Button()
    {
        int[] front = DireToVec(direct);
        front[0] += pos[0]; front[1] += pos[1];
        if (state == Common.Move.Wait && field[front[0], front[1]] == Common.State.Pre)
        {
            state = Common.Move.Get;
            player.GetComponent<Animator>().SetInteger("Move_Int", 2);
            SetImg(front[0], front[1], 0);
            GetComponent<AudioSource>().PlayOneShot(SEs[(int)Common.State.Dug]);
            field[front[0], front[1]] = Common.State.Dug;
        }
    }

    void ToEndding(string s)
    {
        state_text.text = s;
        state = Common.Move.End;
        GameObject o = GameObject.Find("Score");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, -50);
        score_text.fontSize = 75;
        score_text.text = "Score:" + score;
        o = GameObject.Find("Time");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, -145);
        time_text.fontSize = 35;
        int minute = Mathf.FloorToInt(real / 60), second = Mathf.FloorToInt(real % 60);
        time_text.text = "探索時間 " + minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');// + "     残り " + leave + " 個";
        o = GameObject.Find("Leave");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 70);
        leave_text.fontSize = 48;
        int maru = 0, batu = 0, length = 0;
        nums[0, 1] = 26;nums[0, 2] = 26;nums[0, 3] = 26;nums[0, 4] = 26;
        for (int i = 1; i < 52; i++)
        {
            for (int j = 1; j < 52; j++)
            {
                if (field[i, j]!=Common.State.Pre)
                {
                    length++;
                    if (field[i, j] == Common.State.Treasure) maru++;
                    else if (field[i, j] == Common.State.Out) batu++;
                    if (i < nums[0, 1]) nums[0, 1] = i; else if (i > nums[0, 2]) nums[0, 2] = i;
                    if (j < nums[0, 3]) nums[0, 3] = j; else if (j > nums[0, 4]) nums[0, 4] = j;
                }
            }
        }
        width = (float)(nums[0, 1] + nums[0, 2]) / 2f;
        height = (float)(nums[0, 3] + nums[0, 4]) / 2f;
        nums[0, 2]++;nums[0, 4]++;
        nums[0, 5] = Mathf.Max(nums[0, 2] - nums[0, 1], nums[0, 4] - nums[0, 3]);
        nums[0, 0] = Mathf.CeilToInt((float)nums[0, 5] * 8f / 9f);
        height -= 0.3125f * nums[0, 0];
        leave_text.text = "入手数 : " + maru + "  破壊数 : " + batu + "\n道は " + length + " マス掘りました";
        GameObject.Find("Treasure").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        GameObject.Find("Dig").GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        for (int i = 0; i < 4; i++) arrow_img[i].color = Color.clear;
    }

    public void pauseButton(bool b)
    {
        pause = b;
        if (b) Pause.position = new Vector2(-width,0);
        else
        {
            Pause.position = new Vector2(width / 2f, height / 2f);
            int m = Mathf.FloorToInt(time / 60), s = Mathf.FloorToInt(time % 60);
            pause_text.text = "Pause\nTime " + m.ToString().PadLeft(2, '0') + ":" + s.ToString().PadLeft(2, '0') + "\nScore" + score + "\n残り: " + leave;
        }
    }
    public void giveup()
    {
        pause = true;
        Pause.position = new Vector2(-width, 0);
        ToEndding("Give Up　青：入手　赤：破壊");
    }
}

