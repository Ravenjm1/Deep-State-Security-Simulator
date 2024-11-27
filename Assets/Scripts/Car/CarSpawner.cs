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
            currentCar.GetComponent<CarController>().spawner = this;
        }
    }

    [Server]
    public void RemoveCar()
    {
        var carСontroller = currentCar.GetComponent<CarController>();
        var idCard = carСontroller.idCard;
        var person = GetPerson();
        bool result;
        if ((carСontroller.passed && !person.lier)
        || (!carСontroller.passed && person.lier))
        {
            
            result = true;
        }
        else 
        {
            result = false;
        }
        locationManager.Result(result, person);

        NetworkServer.Destroy(idCard);
        NetworkServer.Destroy(currentCar);
        if (carСontroller.enemy)
            carСontroller.enemy.GetComponent<Enemy>().Kill();
            
        currentCar = null;
    }

    public GameObject GetCar()
    {
        return currentCar;
    }

    public Person GetPerson() => currentCar.GetComponent<CarController>().GetPerson();

    // Метод для обработки "Пропуска"
    [Server]
    public void CurrentCarPass()
    {
        if (currentCar != null)
        {
            var carController = currentCar.GetComponent<CarController>();
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
            var carController = currentCar.GetComponent<CarController>();
            if (carController != null)
            {
                carController.CmdReject(); // Машина движется назад
            }
        }
    }
}
