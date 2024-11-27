using UnityEngine;
using Mirror;
using System;
using TMPro;

public class CarSpawner : NetworkBehaviour
{
    public GameObject carPrefab;
    public Transform spawnPoint;
    [SerializeField] LocationManager locationManager;

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

        // Уничтожение врага, если он есть
        carController.enemy?.GetComponent<Enemy>()?.Kill();
        
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
            var carController = currentCar.GetComponent<Car>();
            if (carController != null)
            {
                carController.CmdReject(); // Машина движется назад
            }
        }
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
