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
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Default), "Id карта" },
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Decline), "Id карта фейковая" },
            {(Person.LieType.IdCard, PersonCheckbox.CheckboxState.Approved), "Id карта проверена" },

            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Default), "Номера машины" },
            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Decline), "Номера угнаны" },
            {(Person.LieType.LicensePlate, PersonCheckbox.CheckboxState.Approved), "Номера впорядке" },

            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Default), "Ультрафиолет" },
            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Decline), "Мистика есть" },
            {(Person.LieType.Ultraviolet, PersonCheckbox.CheckboxState.Approved), "Мистики нет" },

            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Default), "Контрабанда" },
            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Decline), "Контрабанда есть" },
            {(Person.LieType.Contraband, PersonCheckbox.CheckboxState.Approved), "Контрабанды нет" },

            {(Person.LieType.Radiation, PersonCheckbox.CheckboxState.Default), "Радиация" },
            {(Person.LieType.Radiation, PersonCheckbox.CheckboxState.Decline), "Радиация превышена" },
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
