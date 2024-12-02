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

    [NonSerialized] public GameObject currentCar;

    [Server]
    public void SpawnCar()
    {
        if (currentCar == null) 
        {
            currentCar = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);
            NetworkServer.Spawn(currentCar); // Синхронизируем спавн машины для всех игроков
            currentCar.GetComponent<Car>().spawner = this;
        }
    }

    [Server]
    public void RemoveCar()
    {
        var carController = currentCar.GetComponent<Car>();
        var person = GetPerson();
        
        // Логика определения результата
        bool result = carController.passed != person.lier;

        // Удаление объектов через NetworkServer
        NetworkServer.Destroy(carController.idCard);
        NetworkServer.Destroy(currentCar);
        
        // Сброс текущей машины
        currentCar = null;

        // Отправка результата в LocationManager
        locationManager.Result(result, person);
    }

    public GameObject GetCar()
    {
        return currentCar;
    }

    public Person GetPerson() => currentCar.GetComponent<Car>().GetPerson();

    // Метод для обработки "Пропуска"
    [Server]
    public void CurrentCarPass()
    {
        if (currentCar != null)
        {
            var carController = currentCar.GetComponent<Car>();
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
        if (currentCar != null)
        {
            ExplosionCar();

            var carController = currentCar.GetComponent<Car>();
            carController.CmdReject(); // Машина взрывается
        }
    }

    [ClientRpc]
    void ExplosionCar()
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
        if (currentCar != null)
        {
            currentCar.GetComponent<Car>().AllowToCome();
        }
    }
}
