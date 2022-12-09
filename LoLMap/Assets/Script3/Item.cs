using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemData
{
    public int Price = 0;
    public float Speed = 0.0f;

}


[System.Serializable]
public class Item
{
    public int id;
    public string title;
    public string description;
    public Sprite icon;
    public Dictionary<string, int> stats = new Dictionary<string, int>();
    public ItemData ItemStates = new ItemData();

    public Item(int id, string title, string description, ItemData p_data )
    {
        ItemStates = p_data;
    }

    public Item (int id, string title, string description, Dictionary<string,int> stats)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.icon = Resources.Load<Sprite>("Assets/Image/아이템/" + title);
        this.stats = stats;
    }


    public Item(Item item)
    {
        this.id = item.id;
        this.title = item.title;
        this.description = item.description;
        this.icon = Resources.Load<Sprite>("Assets/Image/아이템/" + title);
        this.stats = item.stats;
    }

   
}
