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
            // 0 ~ 6 ��ȭ 
            new Item(0, "��ȭ", "Boots.",
            new ItemData(){ Price = 300, Speed = 1.2f } ),

            new Item(1, "�ż��� ��ȭ", "Boots of Swiftness.",
            new ItemData(){ Price = 900, Speed = 1.6f } ),

            new Item (2,"������ ���̿��Ͼ� ��ȭ","Ionian Boots of Lucidity.",
            new ItemData(){ Price = 950, Speed = 1.4f } ),

            new Item(3, "�⵿���� ��ȭ", "Boots of Mobility.",
            new ItemData(){ Price = 1000, Speed = 1.2f } ),

            new Item(4,"�������� ��ȭ", "Berserker's Greaves",
            new ItemData(){ Price = 1100, Speed = 1.4f } ),

            new Item(5,"�������� �Ź�","Sorcerer's Shoes",
            new ItemData(){ Price =1100 , Speed =1.4f } ),

            new Item (6,"�Ǳ� ��ȭ", "Plated Steelcaps",
            new ItemData() { Price = 1100, Speed = 1.4f } )
        };
    }



    



}
