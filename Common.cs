using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
    public enum State
    {
        Wait=0,
        Walk,
        Dig,
        Get,
    }
}

public class Field :MonoBehaviour
{
    GameObject obj;
    int num;

    public Field(GameObject o)
    {
        obj = o;
    }
}
