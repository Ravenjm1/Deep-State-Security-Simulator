using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRight;
    [SerializeField] private GameObject _bannerFailed;
    [SerializeField] private GameObject _bannerGoldChange;
    private TMP_Text _textGoldChange;

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
        _textGoldChange = _bannerGoldChange.GetComponentInChildren<TMP_Text>();
    }

    public void ShowResult(bool result, string goldText, string badText)
    {
        timeShow = timeSet;
        showResult = true;
        if (result) 
        {
            _bannerRight.SetActive(true);
            SetAlpha(_bannerRight, 1f);
        }
        else 
        {
            _bannerFailed.gameObject.SetActive(true);
            SetAlpha(_bannerFailed, 1f);
        }

        if (goldText != "") 
        {
            _bannerGoldChange.SetActive(true);
            SetAlpha(_bannerGoldChange, 1f);
            _textGoldChange.text = goldText;
        }
    }

    void Update() 
    {
        if (showResult) 
        {
            timeShow -= Time.deltaTime;

            if (timeShow <= 0f)
            {
                showResult = false;

                StartCoroutine(FadeOutRoutine(_bannerRight, 1f));
                StartCoroutine(FadeOutRoutine(_bannerFailed, 1f));

                StartCoroutine(FadeOutRoutine(_bannerGoldChange, 6f));
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

    private IEnumerator FadeOutRoutine(GameObject _object, float fadeDuration)
    {
        // Получаем всех дочерних компонентов Image и TextMeshPro
        Image[] images = _object.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = _object.GetComponentsInChildren<TextMeshProUGUI>();

        // Начальное значение альфа (1.0 - полностью видно)
        float startAlpha = 1f;

        // Время, прошедшее с начала анимации
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration); // Плавное уменьшение альфа

            // Устанавливаем альфа для всех Image
            foreach (var image in images)
            {
                if (image != null)
                {
                    Color color = image.color;
                    color.a = alpha;
                    image.color = color;
                }
            }

            // Устанавливаем альфа для всех TextMeshPro
            foreach (var text in texts)
            {
                if (text != null)
                {
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                }
            }

            yield return null; // Ждём следующий кадр
        }
        _object.SetActive(false);
    }

    void SetAlpha(GameObject _object, float alpha)
    {
        // Получаем всех дочерних компонентов Image и TextMeshPro
        Image[] images = _object.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] texts = _object.GetComponentsInChildren<TextMeshProUGUI>();

        // Убедимся, что альфа точно 0 после завершения
        foreach (var image in images)
        {
            if (image != null)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        foreach (var text in texts)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
    }
}
