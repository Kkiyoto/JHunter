using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage10 : MonoBehaviour
{
    Common.State state;

	// Use this for initialization
	void Start ()
    {
        state = Common.State.Wait;
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (state)
        {
            case Common.State.Wait:
                Waiting();
                break;
            case Common.State.Walk:
                Walking();
                break;
            case Common.State.Dig:
                Digging();
                break;
            case Common.State.Get:
                Getting();
                break;
        }

		
	}

    void Waiting()
    {

    }
    void Walking()
    {

    } 
    void Digging()
    {

    }
    void Getting()
    {

    }
}
