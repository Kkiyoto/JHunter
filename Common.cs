using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
    public enum Move
    {
        Wait=0,
        Walk,
        Dig,
        Get,
        End,
        Begin,
    }

    public enum State
    {
        Dug=0,
        Treasure,
        Out,
        Pre,
        Rock
    }

    public enum Direct
    {
        Up=0,
        Right,
        Down,
        Left
    }
}

public class Function : MonoBehaviour
{
    public int[] DireToVec(Common.Direct direct)
    {
        int[] ans = { 0, 0 };
        if (direct == Common.Direct.Up) ans[1] = 1;
        else if (direct == Common.Direct.Down) ans[1] = -1;
        else if (direct == Common.Direct.Right) ans[0] = 1;
        else if (direct == Common.Direct.Left) ans[0] = -1;
        return ans;
    }
    public Common.Direct IntToDire(int n)
    {
        if (n == 0) return Common.Direct.Up;
        else if (n == 1) return Common.Direct.Right;
        else if (n == 2) return Common.Direct.Down;
        else return Common.Direct.Left;
    }
}


public class Field :MonoBehaviour
{
    GameObject obj;
    int num;
    public Common.State state;

    public Field(GameObject o,int x,int y)
    {
        obj = o;
        obj.transform.position = new Vector3(x, y, 0);
        num = 0;
    }

    public int Around
    {
        set
        {
            num = value;
            if (num == 12)
            {
                state = Common.State.Rock;
            }
            else
            {
                state = Common.State.Pre;
            }
        }
        get { return num; }
    }

    public Sprite img
    {
        set { obj.GetComponent<SpriteRenderer>().sprite = value;}
    }

    public void End(Vector3 scale,Color col)
    {
        obj.transform.localScale = scale;
        obj.GetComponent<SpriteRenderer>().color = col;
    }

    public bool Into
    {
        set { if (value) state = Common.State.Dug; }
        get { return (state == Common.State.Dug || state == Common.State.Treasure || state == Common.State.Out); }
    }
}
