using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text textRight;
    public TMP_Text textFailed;
    [SerializeField] private TMP_Text textGoldChange;

    // Инвентарь
    PlayerInventory playerInventory;
    PlayerStats playerStats;
    public List<Image> slots;
    public List<Image> items;
    public Slider hpBar;

    void OnEnable() => LocationContext.OnReady += Init;
    void OnDisable() => LocationContext.OnReady -= Init;

    bool showResult = false;
    float timeShow = 0f;
    float timeSet = 3f;

    void Init()
    {
        playerStats = LocationContext.GetDependency.Player.Stats;
        playerInventory = playerStats.GetComponentInChildren<PlayerInventory>();
    }

    public void ShowResult(bool result, string goldText, string badText)
    {
        timeShow = timeSet;
        showResult = true;
        if (result) 
        {
            textRight.gameObject.SetActive(true);
        }
        else 
        {
            textFailed.gameObject.SetActive(true);
            textFailed.text = "You Failed" + "\n" + badText;
        }
        textGoldChange.gameObject.SetActive(true);
        textGoldChange.text = goldText;
    }

    void Update() 
    {
        if (showResult) 
        {
            timeShow -= Time.deltaTime;

            if (timeShow <= 0f)
            {
                showResult = false;
                textRight.gameObject.SetActive(false);
                textFailed.gameObject.SetActive(false);
                textGoldChange.gameObject.SetActive(false);
            }
        }

        if (playerStats != null)
        {
            hpBar.value = playerStats.Hp / playerStats.MaxHp;
        }

        if (playerInventory != null)
        {
            UpdateSlots();
            UpdateItems();
        }
    }

    public void ChangeRenderUI(bool activate)
    {
        foreach (Image _slot in slots)
        {
            _slot.gameObject.SetActive(activate);
        }
        hpBar.gameObject.SetActive(activate);
    }

    void UpdateSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].color = (playerInventory.currentSlot == i) ? Color.yellow : Color.white;
        }
    }

    void UpdateItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (i < playerInventory.inventory.Length && playerInventory.inventory[i] != null)
            {
                items[i].gameObject.SetActive(true);
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }
    }
}
