using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara : MonoBehaviour
{
    Unit myChara;
    
    void Start()
    {
        myChara = GetComponent<Unit>();
        myChara.state.HP = 100;
        myChara.state.Attack = 10;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
