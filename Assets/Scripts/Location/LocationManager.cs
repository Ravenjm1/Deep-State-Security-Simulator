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
    private float setTime = 360;
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
        visualTime = Math.Abs(time - visualTime) >= 5? time : visualTime;
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
        if (time > 0)
        {
            carSpawner.SpawnCar();
            checkboxListManager.InitCheckboxes();
        }
        else 
        {
            EndDay();
        }

        ShowResult(result, person);
    }

    void ShowResult(bool result, Person person)
    {
        var showText = "";
        var badText = "";

        var badDict = new Dictionary<Person.LieType, string>()
        {
            { Person.LieType.IdCard, "Айди Карта фейковая!" },
            { Person.LieType.LicensePlate, "Номера машины угнаны!" },
            { Person.LieType.Ultraviolet, "Мистика на машине!" },
            { Person.LieType.Contraband, "Контрабанда!" },
            { Person.LieType.Radiation, "Радиация превышена!" },
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

            if (checkbox.CurrentState == PersonCheckbox.CheckboxState.Decline)
            {
                if (isLieType)
                {
                    AddGold(10);
                    showText += "\n+ 10: " + label;
                }
                else
                {
                    ReduceGold(5);
                    showText += "\n- 5: " + label;
                }
            }
            else if (checkbox.CurrentState == PersonCheckbox.CheckboxState.Default && isLieType)
            {
                ReduceGold(5);
                showText += "\n- 5: " + label;
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
