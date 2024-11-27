using UnityEngine;

public static class Chance
{
    static public bool Check(int percent)
    {
        return Random.Range(0, 100) < percent? true : false;
    }

    /// <summary>
    /// Выбирает случайный элемент из массива любого типа.
    /// </summary>
    /// <typeparam name="T">Тип элементов массива</typeparam>
    /// <param name="array">Массив, из которого выбирается элемент</param>
    /// <returns>Случайный элемент массива</returns>
    public static T Choose<T>(T[] array)
    {
        if (array == null || array.Length == 0)
        {
            throw new System.ArgumentException("Массив не должен быть пустым или null.");
        }

        return array[Random.Range(0, array.Length)];
    }

    public static T ChooseEnum<T>() where T : System.Enum
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        System.Random random = new System.Random();
        return (T)values.GetValue(random.Next(values.Length));
    }

    static public bool ChooseBool()
    {
        return Check(50)? true : false;
    }
}
