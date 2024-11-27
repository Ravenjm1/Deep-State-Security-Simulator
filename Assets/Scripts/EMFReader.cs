using System;
using Mirror;
using TMPro;
using UnityEngine;

public class EMFReader : NetworkBehaviour
{
    public CarSpawner spawner;
    public TMP_Text visualText;
    GrabObject grabObject;
    Person person;

    void Start()
    {
        grabObject = GetComponent<GrabObject>();
    }
    void Update()
    {
        if (grabObject.IsActive()) {
            visualText.text = "";
            return;
        };
        visualText.text = "0";

        var carController = FindFirstObjectByType<CarController>();
        if (carController != null)
        {
            person = carController.person;

            if (person == null) 
                return;
            /*
            if (person.isAnomaly)
            {
                var _anomalyPos = new Vector3(
                    carController.transform.position.x + person.anomalyPos.x,
                    carController.transform.position.y,
                    carController.transform.position.z + person.anomalyPos.y
                );
                var dist = Vector3.Distance(transform.position, _anomalyPos);
                visualText.text = Math.Round(Mathf.Clamp((10 - dist) / 9, 0f, 1f) * person.anomalyValue, 3).ToString();
            }
            */
        }
    }
}
