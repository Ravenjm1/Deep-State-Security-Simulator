using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class RadiationController : NetworkBehaviour
{
    public TMP_Text visualText;
    public RectTransform sheetCanvas;
    [NonSerialized] public List<RadItem> radItems;
    Vector2 startPos = new Vector2(0, 35f);
    Vector2 offset_y = new Vector2(0, -18f);
    void OnEnable() => LocationContext.OnReady += Init;
    void OnDisable() => LocationContext.OnReady -= Init;
    

    [Server]
    void Start()
    {
        radItems = new List<RadItem>
        {
            new RadItem(),
            new RadItem(),
            new RadItem(),
            new RadItem(),
            new RadItem(),
            new RadItem(),
        };
        radItems[0].SetItem(
            "Headphones Beats",
            UnityEngine.Random.Range(1f, 3f),
            UnityEngine.Random.Range(1f, 3f)
        );
        radItems[1].SetItem(
            "Battery",
            UnityEngine.Random.Range(2f, 4f),
            UnityEngine.Random.Range(1f, 3f)
        );
        radItems[2].SetItem(
            "Teflon pans",
            UnityEngine.Random.Range(3f, 5f),
            UnityEngine.Random.Range(1f, 3f)
        );
        radItems[3].SetItem(
            "Barrels",
            UnityEngine.Random.Range(4f, 6f),
            UnityEngine.Random.Range(1f, 3f)
        );
        radItems[4].SetItem(
            "Rockets",
            UnityEngine.Random.Range(6f, 9f),
            UnityEngine.Random.Range(1f, 3f)
        );
        radItems[5].SetItem(
            "Nuclear",
            UnityEngine.Random.Range(8f, 12f),
            UnityEngine.Random.Range(1f, 3f)
        );
    }
    [Server]
    void Init()
    {
        RpcInfoToDoc(radItems);
    }
    [ClientRpc]
    void RpcInfoToDoc(List<RadItem> _rad_items)
    {
        radItems = _rad_items;
        for (int i = 0; i < radItems.Count; i ++)
        {
            var item = radItems[i];
            GameObject newItemInfo = Instantiate(visualText.gameObject, sheetCanvas);
            newItemInfo.SetActive(true);
            Vector2 position = startPos + offset_y * i;
            newItemInfo.GetComponent<RectTransform>().anchoredPosition = position;
            var txt = item.name + "\nMax rad: " + Math.Round(item.rangeMin, 1) + " - " + Math.Round(item.rangeMax, 1);
            newItemInfo.GetComponent<TMP_Text>().text = txt;
        }
    }

    public RadItem GetRadItem() 
    {
        var _item = radItems[UnityEngine.Random.Range(0, radItems.Count)];
        return _item;
    }

}


public class RadItem
{
    public string name;
    public float rangeMin;
    public float rangeMax;

    public void SetItem(string name, float rangeMin, float range)
    {
        this.name = name;
        this.rangeMin = rangeMin;
        this.rangeMax = rangeMin + range;
    }
}