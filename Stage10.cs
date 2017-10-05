using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage10 : Function
{
    public GameObject fieldPre ,player;
    public Sprite[] imgs = new Sprite[13],arrow=new Sprite[2];
    public Text time_text,state_text,score_text,leave_text;
    public Image[] arrow_img=new Image[4];
    Common.Move state;
    Common.Direct direct;
    Field[,] fields = new Field[12, 12];
    int[] pos = new int[2];//,front=new int[2];
    float time;
    int score = 0;

	// Use this for initialization
	void Start ()
    {
        state = Common.Move.Wait;
        #region Fieldsの設定
        for(int i = 0; i < 12; i++)
        {
            for(int j = 0; j < 12; j++)
            {
                GameObject o = Instantiate(fieldPre) as GameObject;
                fields[i, j] = new Field(o,i,j);
            }
        }
        for(int i = 0; i < 11; i++)
        {
            fields[i, 0].Around = 12;
            fields[i+1, 11].Around = 12;
            fields[0, i+1].Around = 12;
            fields[11, i].Around = 12;
            fields[i, 0].img = imgs[12];
            fields[i + 1, 11].img = imgs[12];
            fields[0, i + 1].img = imgs[12];
            fields[11, i].img = imgs[12];
        }
        for(int i = 0; i < 20; i++)
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
        for (int i = 1; i < 11; i++)
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
        int x = Random.Range(1, 11);
        int y = Random.Range(1, 11);
        while (fields[x, y].Around == 9)
        {
            x = Random.Range(1, 11);
            y = Random.Range(1, 11);
        }
        pos[0] = x; pos[1] = y;
        //front[0] = x + 1;front[1] = y;
        direct = Common.Direct.Right;
        player.transform.position = new Vector3(x, y, 0);
        SetImg(x, y, -1);
        state_text.text = "Game Start! 周りには" + fields[x, y].Around + "個埋まっています";
        fields[x, y].Into = true;
        #endregion
        time = 1501;
    }

    // Update is called once per frame
    void Update ()
    {
        #region 時間
        time -= Time.deltaTime;
        if (time < 0)
        {
            time = 0;
            state = Common.Move.End;
        }
        int m = Mathf.FloorToInt(time / 60);
        int s = Mathf.FloorToInt(time % 60);
        time_text.text = "Time " + m.ToString().PadLeft(2, '0') + ":" + s.ToString().PadLeft(2, '0');
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
        Vector3 vec = player.transform.position + new Vector3(0, -1.5f, -10);
        vec.x = Mathf.Max(3, Mathf.Min(8, vec.x));
        vec.y = Mathf.Max(1.5f, Mathf.Min(6.5f, vec.y));
        transform.position = vec;
        #endregion
        #region ボタン
        if (Input.GetMouseButton(0))
        {

        }
        #endregion
    }

    void Waiting()
    {

    }
    void Walking()
    {
        int[] front = DireToVec(direct);
        player.transform.Translate(new Vector3(front[0], front[1]) / 100f);
        if((player.transform.position-new Vector3(pos[0], pos[1])).magnitude < 0.01f)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
        }
    } 
    void Digging()
    {
        int[] front = DireToVec(direct);
        player.transform.Translate(new Vector3(front[0], front[1]) / 100f);
        if ((player.transform.position - new Vector3(pos[0], pos[1])).magnitude < 0.01f)
        {
            state = Common.Move.Wait;
            player.transform.position = new Vector3(pos[0], pos[1]);
            player.GetComponent<Animator>().SetInteger("Move_Int", 0);
            SetImg(pos[0], pos[1], -1);
            if (fields[pos[0], pos[1]].Around == 9)
            {
                SetImg(pos[0], pos[1], 10);
                score -= 500;
            }
            else score += 10;
            score_text.text = "Score:" + score.ToString().PadLeft(5, '0');
        }
    }
    void Getting()
    {

    }
    void Endding()
    {

    }

    void SetImg(int x, int y, int target)
    {
        if (target == -1) fields[x, y].img = imgs[fields[x, y].Around];
        else fields[x, y].img = imgs[target];
    }

    public void ArrowButton(int n)
    {
        Common.Direct direction = IntToDire(n);
        //img.sprite = arrow[1];
        arrow_img[n].sprite = arrow[1];
        if (state == Common.Move.Wait)
        {
            int[] front = DireToVec(direction);
            pos[0] += front[0]; pos[1] += front[1];
            direct = direction;
            player.GetComponent<Animator>().SetInteger("Direct_Int", (int)direction);
            if (fields[pos[0], pos[1]].Into)
            {
                state = Common.Move.Walk;
                player.GetComponent<Animator>().SetInteger("Move_Int", 1);
            }
        }
    }
    public void ArrowUp(Image img)
    {
        img.sprite = arrow[0];
    }

    public void Dig_Button()
    {
        int[] front = DireToVec(direct);
        pos[0] += front[0]; pos[1] += front[1];
        if (state == Common.Move.Wait&& !fields[front[0], front[1]].Into)
        {
            state = Common.Move.Dig;
            player.GetComponent<Animator>().SetInteger("Move_Int", 2);
            SetImg(pos[0], pos[1], 0);
            fields[pos[0], pos[1]].Into = true;
        }
    }
}
