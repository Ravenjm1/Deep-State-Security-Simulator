using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LicensePlateChecker : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text resultText;
    [SerializeField] PersonGenerator personGenerator;
    private LookAtObject lookAtObject;
    void Start()
    {
        lookAtObject = GetComponent<LookAtObject>();
        lookAtObject.OnStartLook += OnStartLook;
        lookAtObject.OnStopLook += OnStopLook;
        inputField.interactable = false;
        inputField.onValueChanged.AddListener(OnTextChanged);
        LocationManager.OnResult += OnResult;
    }

    private void OnResult()
    {
        inputField.text = "";
        resultText.text = "";
    }

    private void OnTextChanged(string text)
    {
        // Преобразуем текст в верхний регистр
        inputField.text = text.ToUpper();
    }

    private void OnStartLook()
    {
        inputField.interactable = true;
        inputField.ActivateInputField();
    }

    private void OnStopLook()
    {
        inputField.DeactivateInputField();
        inputField.interactable = false;
    }

    
    public void CheckNumber()
    {
        string inputText = inputField.text;

        int findIndex = personGenerator.PersonList.FindIndex(
            person => person.licensePlate == inputText
        );

        if (findIndex != -1)
        {
            var findPlate = personGenerator.PersonList[findIndex];

            if (findPlate.lierTypes.Contains(Person.LieType.LicensePlate))
            {
                resultText.text = "stolen numbers!";
            }
            else
            {
                resultText.text = "numbers fine";
            }
        }
        else
        {
            resultText.text = "unknown number";
        }
    }

}
