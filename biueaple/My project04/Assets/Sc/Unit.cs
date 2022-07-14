using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public ChamState state;
    public GameObject HP;
    public Image HPBar;
    public Camera cam;

    [SerializeField]
    protected Transform m_Target = null;

    void Start()
    {
        state.HP = 100;
        state.Attack = 10;
        state.Speed = 10.0f;
    }

    
    void Update()
    {
        HP.transform.position = cam.WorldToScreenPoint(m_Target.position);
        HPBar.rectTransform.sizeDelta = new Vector2(state.HP, 20);
    }
}
