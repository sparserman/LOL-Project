using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ItemCodePassing : MonoBehaviour
{
    List<Item> item;
    public int x = 0;
    private void Start()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("������ ǥ");
    
        for(var i = 0;i<data.Count;i++)
        {
            Debug.Log(data[i]["Item"].ToString());
            //item[i] = data[i]["item"].ToString();
        }
    }
    
}
