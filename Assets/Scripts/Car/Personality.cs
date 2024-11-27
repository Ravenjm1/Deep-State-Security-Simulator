using Mirror;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Personality : NetworkBehaviour
{
    public TMP_Text cardText;
    public UnityEngine.UI.Image avatar;
    public bool lier = false;

    private Person person;

    // Метод для инициализации объекта на сервере
    public void InitializePerson(Person person)
    {
        this.person = person;
        AddPersonality(person);
        Debug.Log("lie types: " + string.Join(", ", person.lierTypes));
    }

    public void AddPersonality(Person person)
    {
        string str_id = person.cardId.ToString();
        cardText.text = person.name + ": " + str_id;
        if (person.isRad)
        {
            cardText.text += "\nrad: " + System.Math.Round(person.idCardRadiation, 2);
        }
        lier = person.lier;
    }

    // Метод для доступа к объекту Person на клиенте
    public Person GetPerson()
    {
        return person;
    }

    // Сериализация данных
    public override void OnSerialize(NetworkWriter writer, bool initialState)
    {
        // Сериализуем данные Person
        writer.WriteInt(person.Id);
        writer.WriteInt(person.cardId);
        writer.WriteString(person.name);
        writer.WriteString(person.licensePlate);
        writer.WriteBool(person.fakeLicencePlate);
        writer.WriteBool(person.lier);

        // Сериализуем список lierTypes
        writer.WriteInt(person.lierTypes.Count);
        foreach (var lieType in person.lierTypes)
        {
            writer.WriteInt((int)lieType);
        }

        writer.WriteBool(person.hasCargo);
        writer.WriteBool(person.contraband);
        writer.WriteBool(person.isRad);
        writer.WriteFloat(person.radiation);
        writer.WriteFloat(person.idCardRadiation);
        writer.WriteBool(person.ultraviolet);
    }

    // Десериализация данных
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        // Десериализуем данные Person
        person = new Person();
        person.Id = reader.ReadInt();
        person.cardId = reader.ReadInt();
        person.name = reader.ReadString();
        person.licensePlate = reader.ReadString();
        person.fakeLicencePlate = reader.ReadBool();
        person.lier = reader.ReadBool();

        // Десериализуем список lierTypes
        int lierTypesCount = reader.ReadInt();
        person.lierTypes = new List<Person.LieType>();
        for (int i = 0; i < lierTypesCount; i++)
        {
            person.lierTypes.Add((Person.LieType)reader.ReadInt());
        }

        person.hasCargo = reader.ReadBool();
        person.contraband = reader.ReadBool();
        person.isRad = reader.ReadBool();
        person.radiation = reader.ReadFloat();
        person.idCardRadiation = reader.ReadFloat();
        person.ultraviolet = reader.ReadBool();

        // Инициализируем объект на клиенте
        AddPersonality(person);
    }
}
