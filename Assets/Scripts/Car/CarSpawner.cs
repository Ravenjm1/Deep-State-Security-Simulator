using UnityEngine;
using Mirror;
using System;
using TMPro;
using Unity.VisualScripting;

public class CarSpawner : NetworkBehaviour
{
    public GameObject carPrefab;
    public Transform spawnPoint;
    [SerializeField] LocationManager locationManager;
    [SerializeField] BoxCollider explosionCollider;
    [SerializeField] PersonGenerator personGenerator;

    [NonSerialized] public GameObject currentCarObject;
    private Car currentCar;

    [Server]
    public void SpawnCar()
    {
        if (currentCarObject == null) 
        {
            currentCarObject = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);
            NetworkServer.Spawn(currentCarObject); // Синхронизируем спавн машины для всех игроков

            currentCar = currentCarObject.GetComponent<Car>();
            currentCar.spawner = this;
            var _person = personGenerator.GetPerson();
            Debug.Log(_person);
            currentCar.SetPerson(_person);
            currentCar.Init();
        }
    }

    [Server]
    public void RemoveCar()
    {
        var carController = currentCarObject.GetComponent<Car>();
        var person = GetPerson();
        
        // Логика определения результата
        bool result = carController.passed != person.lier;

        // Удаление объектов через NetworkServer
        NetworkServer.Destroy(carController.idCard);
        NetworkServer.Destroy(currentCarObject);
        
        // Сброс текущей машины
        currentCarObject = null;

        // Отправка результата в LocationManager
        locationManager.Result(result, person);
    }

    public GameObject GetCar()
    {
        return currentCarObject;
    }

    public Person GetPerson() => currentCarObject.GetComponent<Car>().GetPerson();

    // Метод для обработки "Пропуска"
    [Server]
    public void CurrentCarPass()
    {
        if (currentCarObject != null)
        {
            var carController = currentCarObject.GetComponent<Car>();
            if (carController != null)
            {
                Debug.Log("CurrentCarPass");
                carController.CmdPass(); // Машина движется вперёд
            }
        }
    }

    // Метод для обработки "Отказа"
    [Server]
    public void CurrentCarCancel()
    {
        RpcExplosion();

        if (currentCarObject != null)
        {
            var carController = currentCarObject.GetComponent<Car>();
            carController.CmdReject(); // Машина взрывается
        }
    }

    [ClientRpc]
    void RpcExplosion()
    {
        var player = LocationContext.GetDependency.Player;
        if (IsObjectInsideBox(player.transform.position, explosionCollider))
        {
            player.Stats.GetDamage(100);
        }
        if (isServer)
        {
            var enemies = FindObjectsByType(typeof(Enemy), FindObjectsSortMode.None);
            foreach (var enemyObj in enemies)
            {
                var enemy = enemyObj.GetComponent<Enemy>();
                if (IsObjectInsideBox(enemy.transform.position, explosionCollider))
                {
                    enemy.Kill();
                }
            }
        }
    }

    private bool IsObjectInsideBox(Vector3 objectPosition, BoxCollider box)
    {
        return box.bounds.Contains(objectPosition);
    }

    public void AllowToCome()
    {
        CmdAllowToCome();
    }

    [Command (requiresAuthority = false)]
    void CmdAllowToCome()
    {
        if (currentCarObject != null)
        {
            currentCarObject.GetComponent<Car>().AllowToCome();
        }
    }
}
