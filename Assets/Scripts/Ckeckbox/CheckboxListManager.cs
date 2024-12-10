using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxListManager : NetworkBehaviour
{
    [SerializeField] private GameObject checkboxPrefab; // Префаб чекбокса
    [SerializeField] private GameObject checkboxUIPrefab; // Префаб чекбокса
    [SerializeField] public Transform checkboxContainer; // Контейнер для чекбоксов
    [SerializeField] private Transform checkboxUIContainer; // Контейнер для чекбоксов на экране

    [NonSerialized] public List<PersonCheckbox> checkboxes = new List<PersonCheckbox>();
    public static Dictionary<(Person.LieType, PersonCheckbox.CheckboxState), string> CheckboxText;

    void Start()
    {
        CheckboxText = new Dictionary<(Person.LieType, PersonCheckbox.CheckboxState), string>
        {
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Default), "Id card" },
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Decline), "Id card is fake" },
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Approved), "Id карта проверена" },

            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Default), "License plate" },
            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Decline), "License plates are stolen" },
            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Approved), "Номера впорядке" },

            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Default), "Ultraviolet" },
            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Decline), "Ultraviolet detect something" },
            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Approved), "Мистики нет" },

            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Default), "Contraband" },
            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Decline), "Contraband here" },
            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Approved), "Контрабанды нет" },

            {(Person.LieType.Radiation, PersonCheckbox.CheckboxState.Default), "Radiation" },
            {(Person.LieType.Radiation, PersonCheckbox.CheckboxState.Decline), "Radiation exceeded" },
            {(Person.LieType.Radiation, PersonCheckbox.CheckboxState.Approved), "Радиация впорядке" },
        };
    }
    [Server]
    public void InitCheckboxes()
    {
        var items = new List<Person.LieType>
        {
            Person.LieType.IdCard,
            Person.LieType.LicensePlate,
            Person.LieType.Ultraviolet,
            Person.LieType.Contraband,
            Person.LieType.Radiation,
        };

        CreateCheckboxList(items);
    }

    [Server]
    void CreateCheckboxList(List<Person.LieType> items)
    {
        ClearList();

        foreach (var type in items)
        {
            var checkboxObject = Instantiate(checkboxPrefab, checkboxContainer);
            var checkbox = checkboxObject.GetComponent<PersonCheckbox>();

            // Устанавливаем текст и тип чекбокса
            checkbox.type = type;

            checkbox.OnStateChanged += state => OnCheckboxStateChanged(checkbox.type, state);

            checkboxes.Add(checkbox);
            NetworkServer.Spawn(checkboxObject);
        }
    }

    public GameObject AddCheckboxUI()
    {
        var chbxUI = Instantiate(checkboxUIPrefab, checkboxUIContainer);
        return chbxUI;
    }    
    [Server]
    private void ClearList()
    {
        foreach (Transform child in checkboxContainer)
        {
            NetworkServer.Destroy(child.gameObject);
        }
        checkboxes.Clear();
    }

    private void OnCheckboxStateChanged(Person.LieType type, PersonCheckbox.CheckboxState state)
    {
        Debug.Log($"Checkbox {type} changed state to: {state}");
    }
}
