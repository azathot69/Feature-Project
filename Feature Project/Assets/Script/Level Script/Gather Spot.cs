using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherSpot : MonoBehaviour
{
    #region Variables
    public int canGather = 3;
    public string itemGet;
    public List<string> itemBag = new List<string>();
    #endregion

    private void Awake()
    {
        itemBag.Add("Candy");
        itemBag.Add("A Child");
        itemBag.Add("Desecrated carcass");
        itemBag.Add("Ur Mom");
        itemBag.Add("The souls of the damned");
        itemBag.Add("Gently Used Nasal Spray");
        itemBag.Add("just hair");
        itemBag.Add("my Mom");
        itemBag.Add("Teeth");
    }
    public string Gather()
    {
        if (canGather != 0) {
            //Get random Item
            itemGet = itemBag[Random.Range(0, itemBag.Count)];
            //Reduce canGather
            canGather--;

            if (canGather == 0) { this.gameObject.SetActive(false); }

            return itemGet;
        }
        else
        {
            print("You can't gather any more!");
            
            return null;
        }
    }
}
