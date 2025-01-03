using UnityEngine;
using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;

public class Car : NetworkBehaviour
{
    float moveSpeed = 5f;
    Vector3 waitingPosition = Vector3.zero; // Центр сцены
    [NonSerialized] public CarSpawner spawner;
    private PersonGenerator personGenerator;
    private Driver driver;
    public Transform IdCardSpawner;
    public GameObject idCard;
    public Transform itemSpawner;
    public GameObject itemPrefab;
    public GameObject enemyPrefab;
    [SerializeField] Transform enemySpawner;
    [SerializeField] private TMP_Text textLicensePlate;
    [SerializeField] private UltravioletObject ultravialetObject;
    [SerializeField] private List<Transform> contrabandSpawnPoints;
    [SerializeField] private GameObject _idCardPrefab;

    private Vector3 cancelPoint = new Vector3(-10f, 0, 0);
    private Vector3 passPoint = new Vector3(20f, 0, 0);
    private bool isComing = false;
    [SyncVar] private bool isWaiting = false; // Синхронизируем состояние "ожидания" между клиентами
    private Vector3 targetPosition;
    private bool decided = false;
    public bool passed {private set; get; } = false;
    [SyncVar(hook = nameof(OnPersonChanged))] [NonSerialized] public Person person;

    public Enemy enemy {private set; get; }

    private CarExplosion _explosion;
    public bool IsMoving { private set; get; }

    private int chanceToSpawnEnemy = 0;

    void Awake()
    {
        driver = GetComponentInChildren<Driver>();
        _explosion = GetComponent<CarExplosion>();
    }

    public void SetPerson(Person person) 
    {
        this.person = person;
    }

    [Server]
    public void Init()
    {
        Debug.Log("pers: " + person);
        if (person.hasCargo)
        {
            for (int i = 0; i < 4; i++)
            {
                var itemObj = Instantiate(itemPrefab, itemSpawner);
                Vector3 randomPosition = new Vector3(
                    Random.Range(-1f, 1f), 
                    Random.Range(0f, 0.3f),
                    Random.Range(-0.6f, 0.6f)
                );

                var _color = Color.gray;
                if (person.isRad)
                    _color = Color.green;

                var bagComp = itemObj.GetComponent<Baggage>();
                bagComp.SetColor(_color);
                bagComp.SetPositon(randomPosition);
                bagComp.radiation = person.radiation;

                NetworkServer.Spawn(itemObj);
            }
        }
        if (person.contraband)
        {
            var contrabandTransform = contrabandSpawnPoints[Random.Range(0, contrabandSpawnPoints.Count - 1)];
            var contrabandObj = Instantiate(itemPrefab, contrabandTransform);
            var bagComp = contrabandObj.GetComponent<Baggage>();
            bagComp.SetColor(Color.red);

            NetworkServer.Spawn(contrabandObj);
        }
        SpawnIdCard();
        driver.SetIdCard(idCard.GetComponent<GrabObject>());
    }

    public Person GetPerson() => person;

    void OnPersonChanged(Person oldPerson, Person newPerson)
    {
        if (newPerson != null) 
        {
            textLicensePlate.text = newPerson.licensePlate;
            if (newPerson.ultraviolet)
            {
                ultravialetObject.gameObject.SetActive(true);
            }
        }
    }
    [Server]
    void Update()
    {
        IsMoving = false;

        if (!isWaiting && isComing)
        {
            MoveToWaitingPosition();
            IsMoving = true;
        }
        if (decided && passed)
        {
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                IsMoving = true;
            }
            else // Удаляем машину после движения
            {
                spawner.RemoveCar();
            }
        }
    }
    [Server]
    void MoveToWaitingPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, waitingPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, waitingPosition) < 0.1f)
        {
            isWaiting = true; // Машина достигла центра и ждёт действия

            if (person.lier && Chance.Check(50))
            {
                StartCoroutine(ChanceToSpawnEnemy());
            }
        }
    }
    [Server]
    public void SpawnIdCard()
    {
        // Создаем объект idCard
        idCard = Instantiate(_idCardPrefab, driver.transform);

        // Получаем компонент Personality и устанавливаем данные Person
        Personality personality = idCard.GetComponent<Personality>();
        Debug.Log("SpawnIdCard pers: " + person.name);
        personality.InitializePerson(person);
        // Спавним объект
        NetworkServer.Spawn(idCard);
    }

    private IEnumerator ChanceToSpawnEnemy()
    {
        while (!Chance.Check(chanceToSpawnEnemy))
        {
            chanceToSpawnEnemy += 1;
            yield return new WaitForSeconds(3f);
        }
        var objEnemy = Instantiate(enemyPrefab, enemySpawner.position, Quaternion.identity);
        enemy = objEnemy.GetComponent<Enemy>();
        NetworkServer.Spawn(objEnemy);
    } 

    [Server]
    public void CmdPass()
    {
        if (isWaiting && !decided)
        {
            decided = true;
            passed = true;
            targetPosition = passed? passPoint : cancelPoint;
            RpcDecide(true);
        }
    }
    [Server]
    public void CmdReject()
    {
        if (isWaiting && !decided)
        {
            decided = true;
            passed = false;
            RpcDecide(false);
        }
    }

    [ClientRpc] // Обновляем позицию у всех клиентов
    private void RpcDecide(bool _passed)
    {
        decided = true;
        passed = _passed;
        
        if (!passed)
        {
            _explosion.Explode();
            if (isServer)
            {
                spawner.RemoveCar();
            }
        }
        
    }

    [Server]
    public void AllowToCome()
    {
        isComing = true;
    }
}
