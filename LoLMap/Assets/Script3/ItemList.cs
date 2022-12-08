using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemList : MonoBehaviour
{
    public List<Item> items = new List<Item>();


    private void Awake()
    {
        ItemDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }


    void ItemDatabase()
    {
        items = new List<Item>()
        {
            // 0 ~ 6 장화 
            new Item(0, "장화", "Boots.",
            new ItemData(){ Price = 300, Speed = 1.2f } ),

            new Item(1, "신속의 장화", "Boots of Swiftness.",
            new ItemData(){ Price = 900, Speed = 1.6f } ),

            new Item (2,"명석함의 아이오니아 장화","Ionian Boots of Lucidity.",
            new ItemData(){ Price = 950, Speed = 1.4f } ),

            new Item(3, "기동력의 장화", "Boots of Mobility.",
            new ItemData(){ Price = 1000, Speed = 1.2f } ),

            new Item(4,"광전사의 군화", "Berserker's Greaves",
            new ItemData(){ Price = 1100, Speed = 1.4f } ),

            new Item(5,"마법사의 신발","Sorcerer's Shoes",
            new ItemData(){ Price =1100 , Speed =1.4f } ),

            new Item (6,"판금 장화", "Plated Steelcaps",
            new ItemData() { Price = 1100, Speed = 1.4f } )
        };
    }



    



}
