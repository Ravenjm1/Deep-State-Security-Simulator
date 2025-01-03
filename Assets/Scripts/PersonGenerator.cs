using System.Collections.Generic;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Linq;
public class PersonGenerator : NetworkBehaviour
{
    public GameObject personIdPrefab;
    public RectTransform sheetCanvas;
    Vector2 startPos = new Vector2(-50f, 35f);
    Vector2 offset_y = new Vector2(0, -25f);
    Vector2 offset_x = new Vector2(100f, 0);
    [System.NonSerialized] public List<Person> PersonList = new List<Person>(); // Инициализируем список сразу
    private HashSet<int> usedIndices = new HashSet<int>(); // Индексы уже выданных объектов
    int personNum = 8;

    void OnEnable() => LocationContext.OnReady += Init;
    void OnDisable() => LocationContext.OnReady -= Init;

    [Server]
    void Init()
    {
        PersonList.Clear();
        for (int i = 0; i < personNum; i++)
        {
            var person = new Person();
            InitPerson(person);
            PersonList.Add(person);
        }
        RpcSetDocumentPerson(PersonList);
    }

    [ClientRpc]
    void RpcSetDocumentPerson(List<Person> listPerson)
    {
        PersonList = listPerson;
        Vector2 position = startPos;

        for (int i = 0; i < PersonList.Count; i++)
        {
            var person = PersonList[i];
            GameObject newCard = Instantiate(personIdPrefab, sheetCanvas);
            newCard.GetComponent<RectTransform>().anchoredPosition = position;
            newCard.GetComponentInChildren<TMP_Text>().text = $"{person.name}: {person.Id}";
            position += offset_y;

            if (i == PersonList.Count / 2 - 1)
            {
                position = startPos + offset_x;
            }
        }
    }

    public Person GetPerson()
    {
        // Если все элементы уже выданы, сбрасываем "использованные"
        if (usedIndices.Count >= PersonList.Count)
        {
            usedIndices.Clear();
        }

        int index;
        do
        {
            index = Random.Range(0, PersonList.Count);
        }
        while (usedIndices.Contains(index)); // Генерируем новый индекс, пока он не станет уникальным

        usedIndices.Add(index); // Помечаем индекс как использованный

        return PersonList[index]; // Возвращаем объект
    }

    void InitPerson(Person person)
    {
        person.Id = Random.Range(1000, 10000);
        person.cardId = person.Id;
        person.name = GetRandomName();
        person.licensePlate = GenerateLicensePlate();

        person.hasCargo = Chance.Check(80);
        if (person.hasCargo)
        {
            person.isRad = Chance.Check(70);
            if (person.isRad)
            {
                person.radiation = Random.Range(2f, 9f);
                person.idCardRadiation = person.radiation;
            }
        }
        person.lier = Chance.Check(66);
        person.lier = Chance.Check(66); // Шанс стать лжецом
        if (person.lier)
        {
            // Гарантировано добавляем один тип лжи
            AddLieType(person);

            // Шанс добавить дополнительные типы
            int additionalChance = 50; // Начинаем с 50%
            while (Chance.Check(additionalChance) && person.lierTypes.Count < System.Enum.GetValues(typeof(Person.LieType)).Length)
            {
                AddLieType(person);
            }
        }
    } 

    // Добавление уникального типа лжи
    void AddLieType(Person person)
    {
        var allTypes = System.Enum.GetValues(typeof(Person.LieType)).Cast<Person.LieType>();
        var availableTypes = allTypes.Where(type => !person.lierTypes.Contains(type)); // Исключаем уже добавленные типы

        if (availableTypes.Any())
        {
            var newLieType = availableTypes.ElementAt(Random.Range(0, availableTypes.Count()));
            person.lierTypes.Add(newLieType);

            // Применяем логики для каждого типа
            switch (newLieType)
            {
                case Person.LieType.IdCard:
                    person.cardId = Random.Range(1000, 10000);
                    break;
                case Person.LieType.LicensePlate:
                    person.fakeLicencePlate = true;
                    break;
                case Person.LieType.Ultraviolet:
                    person.ultraviolet = true;
                    break;
                case Person.LieType.Contraband:
                    person.contraband = true;
                    break;
                case Person.LieType.Radiation:
                    person.hasCargo = true;
                    person.isRad = true;
                    person.idCardRadiation -= Random.Range(1f, 3f);
                    break;
            }
        }
    }

    static string[] pullNames = { 
        "Chad", "Karen", "Biggie", "Elon", "Rambo", "Rocky", "Maverick", "Bruce", 
        "Duke", "Jack", "Delta", "Buzz", "Arnold", "Neo", "Blade", 
        "Spud", "Ace", "Tango", "John", "Chuck" 
    };
    static string[] pullSecNames = { 
        "Thunder", "Mega", "Danger", "Power", "Flex", 
        "Muscle", "Rage", "Wolf", "Storm", "Hawk", "Fury",
         "Crusher", "Burn", "Boulder", "Alpha", "Nitro", "Blaze", 
         "Steel", "Smash", "Bullet" 
    };

    public static string GetRandomName()
    {
        var _name = pullNames[Random.Range(0, pullNames.Length)];
        var _secondName = pullSecNames[Random.Range(0, pullSecNames.Length)];

        return _name + " " + _secondName;
    }

    /// <summary>
    /// Генерация случайного автомобильного номера
    /// </summary>
    /// <returns>Случайный номер</returns>
    ///
    public static string GenerateLicensePlate()
    {
        // Форматы номеров: ABC-1234 или 1ABC234
        string[] formats = { "ABC-1234", "1ABC234" };

        // Выбираем случайный формат
        string selectedFormat = formats[UnityEngine.Random.Range(0, formats.Length)];

        // Набор букв, исключая 'E'
        char[] letters = "ABCDDFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        // Генерируем номер по выбранному формату
        char[] licensePlate = new char[selectedFormat.Length];
        for (int i = 0; i < selectedFormat.Length; i++)
        {
            char c = selectedFormat[i];
            if (c == 'A')
            {
                // Генерация случайной буквы, исключая 'E'
                licensePlate[i] = letters[UnityEngine.Random.Range(0, letters.Length)];
            }
            else if (c == '1')
            {
                // Первая цифра (без нуля в начале)
                licensePlate[i] = (char)('1' + UnityEngine.Random.Range(0, 9));
            }
            else if (c == '2' || c == '3' || c == '4')
            {
                // Случайная цифра
                licensePlate[i] = (char)('0' + UnityEngine.Random.Range(0, 10));
            }
            else
            {
                // Прочие символы (например, "-")
                licensePlate[i] = c;
            }
        }

        return new string(licensePlate);
    }
 
    /*
    public static string GenerateLicensePlate()
    {
        // Форматы номеров: ABC-1234 или 1ABC234
        string[] formats = { "ABC-1234", "1ABC234" };

        // Выбираем случайный формат
        string selectedFormat = formats[Random.Range(0, formats.Length)];

        // Генерируем номер по выбранному формату
        char[] licensePlate = new char[selectedFormat.Length];
        for (int i = 0; i < selectedFormat.Length; i++)
        {
            char c = selectedFormat[i];
            if (c == 'A')
            {
                // Генерация случайной буквы
                licensePlate[i] = (char)Random.Range('A', 'Z' + 1);
            }
            else if (c == '1')
            {
                // Первая цифра (без нуля в начале)
                licensePlate[i] = (char)('1' + Random.Range(0, 9));
            }
            else if (c == '2' || c == '3' || c == '4')
            {
                // Случайная цифра
                licensePlate[i] = (char)('0' + Random.Range(0, 10));
            }
            else
            {
                // Прочие символы (например, "-")
                licensePlate[i] = c;
            }
        }

        return new string(licensePlate);
    }*/
    
}

[System.Serializable]
public class Person
{
    public enum LieType {IdCard, LicensePlate, Ultraviolet, Contraband, Radiation};
    public int Id;
    public int cardId;
    public string name;
    public string licensePlate;
    public bool fakeLicencePlate = false;
    public bool lier = false;
    public List<LieType> lierTypes = new List<LieType>(); // Список типов лжи
    public bool hasCargo = false;
    public bool contraband = false;
    public bool isRad = false;
    public float radiation = 0f;
    public float idCardRadiation = 0f;
    public bool ultraviolet;
}
