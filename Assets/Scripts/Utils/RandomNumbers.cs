using System;
using System.Collections.Generic;

public class RandomNumbers
{
    public static List<int> GetUniqueRandomNumbers(int n, int min, int max)
    {
        if (max - min + 1 < n)
            throw new ArgumentException("Диапазон слишком мал для выбора n уникальных чисел.");

        List<int> numbers = new List<int>();
        for (int i = min; i <= max; i++)
            numbers.Add(i);

        Random rnd = new Random();
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            // Обмениваем местами элементы
            int temp = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = temp;
        }

        // Возвращаем первые n элементов перемешанного списка
        return numbers.GetRange(0, n);
    }
}
