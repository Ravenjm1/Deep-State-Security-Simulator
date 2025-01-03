using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class LocationManager : NetworkBehaviour
{
    public TMP_Text timeText;
    public TMP_Text quotaText;
    [SerializeField] private PlayerUI controllerGUI;
    [SerializeField] private CheckboxListManager checkboxListManager;
    [SerializeField] private CarSpawner carSpawner;
    private float setTime = 600;
    private float timeToSpawnCar = 1f;
    private float visualTime = 0;
    [SyncVar] private float time = 0;
    [SyncVar] private int quota = 100;
    [SyncVar] private int gold = 0;
    [SyncVar] private bool inited = false;

    public static Action OnResult = delegate { };

    void OnEnable() => LocationContext.OnReady += Init;
    void OnDisable() => LocationContext.OnReady -= Init;

    void Init()
    {
        inited = true;
        if (LobbyStat.Instance != null) {
            quota = LobbyStat.Instance.Quota;
        }
        time = setTime;
        visualTime = time;
        StartCoroutine(WaitToCar());
    }

    IEnumerator WaitToCar() 
    {
        yield return null;
        SpawnCar();
    }
    void SpawnCar()
    {
        carSpawner.SpawnCar();
        checkboxListManager.InitCheckboxes();
    }

    public void AddGold(int _gold)
    {
        gold += _gold;
    }

    public void ReduceGold(int _gold)
    {
        gold = Math.Max(gold - _gold, 0);
    }

    void Update()
    {
        if (!inited)
            return;

        if (isServer)
        {
            time = Mathf.Max(time - Time.deltaTime, 0f);
        }
        visualTime = (Math.Abs(time - visualTime) >= 5 || time == 0)? time : visualTime;
        timeText.text = string.Format("{0}:{1:00}", (int)(visualTime / 60), (int)(visualTime % 60));
        quotaText.text = gold + " / " + quota;
    }

    [Server]
    void EndDay()
    {
        inited = false;
        var networkManager = (MyNetworkManager)NetworkManager.singleton;
        RpcReachQuota((gold >= quota)? true : false);
        networkManager.ServerChangeScene(networkManager.lobbyScene);
    }

    [ClientRpc]
    void RpcReachQuota(bool isReached)
    {
        LobbyStat.Instance.RichQuota = isReached;
    }
    
    [Server]
    public void Result(bool result, Person person)
    {
        ShowResult(result, person);

        if (time > 0)
        {
            SpawnCar();
        }
        else 
        {
            EndDay();
        }
    }

    void ShowResult(bool result, Person person)
    {
        var showText = "";
        var badText = "";

        var badDict = new Dictionary<Person.LieType, string>()
        {
            { Person.LieType.IdCard, "Id card is fake!" },
            { Person.LieType.LicensePlate, "license plates are stolen!" },
            { Person.LieType.Ultraviolet, "Mystiks!" },
            { Person.LieType.Contraband, "Contraband!" },
            { Person.LieType.Radiation, "Rdiation exceeded!" },
        };

        if (result)
        {
            AddGold(20);
        }
        else
        {
            ReduceGold(20);

            // Обработка нескольких типов лжи
            if (person.lier && person.lierTypes.Count > 0)
            {
                foreach (var lieType in person.lierTypes)
                {
                    if (badDict.TryGetValue(lieType, out var message))
                    {
                        badText += message + "\n"; // Добавляем каждую ошибку
                    }
                }
            }
            else
            {
                badText = "Чел чист!";
            }
        }

        foreach (var checkbox in checkboxListManager.checkboxes)
        {
            bool isLieType = person.lierTypes.Contains(checkbox.type);
            string label = checkbox.label;

            Debug.Log("checkbox: " + checkbox.CurrentState);

            if (checkbox.CurrentState == PersonCheckbox.CheckboxState.Decline)
            {
                if (isLieType)
                {
                    AddGold(10);
                    showText += label + " +10\n";
                }
                else
                {
                    ReduceGold(5);
                    showText += label + " -5\n";
                }
            }
            else if (checkbox.CurrentState == PersonCheckbox.CheckboxState.Default && isLieType)
            {
                ReduceGold(5);
                showText += label + " -5\n";
            }
        }

        RpcShowResult(result, showText, badText);
    }

    [ClientRpc]
    private void RpcShowResult(bool result, string goldText, string badText)
    {
        controllerGUI.ShowResult(result, goldText, badText);
        OnResult();
    }
}
