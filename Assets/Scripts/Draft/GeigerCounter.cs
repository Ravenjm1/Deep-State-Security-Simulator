using System;
using Mirror;
using TMPro;
using UnityEngine;

public class GeigerCounter : NetworkBehaviour
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
        }
        visualText.text = "0";

        var carController = FindFirstObjectByType<Car>();
        if (carController != null)
        {
            person = carController.person;

            if (person == null) 
                return;
            
            if (person.radiation > 0f)
            {
                var dist = Vector3.Distance(transform.position, carController.transform.position);
                visualText.text = Math.Round(Mathf.Clamp((10 - dist) / 9, 0f, 1f) * person.radiation, 3).ToString();
            }
        }
    }
}
