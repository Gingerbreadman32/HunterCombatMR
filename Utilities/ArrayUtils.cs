﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Utilities
{
    public static class ArrayUtils
    {
        public static void Add<T>(ref T[] array, T value)
        {
            int originalLength = array?.Length ?? 0;

            Array.Resize(ref array, originalLength + 1);

            array[originalLength] = value;
        }

        public static void AddOrInsert<T>(ref T[] array, 
            T value, 
            int index)
        {
            if (index >= array.Length)
                Add(ref array, value);

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index cannot be less than 0!");

            T[] start = new T[index];

            if (index > 0)
            {
                Array.Copy(array, 0, start, 0, index);
            }

            Add(ref start, value);

            var newLength = array.Length - index;
            var newArray = new T[start.Length + newLength];
            start.CopyTo(newArray, 0);
            Array.Copy(array, index, newArray, start.Length, newLength);

            array = newArray;
        }

        public static void Remove<T>(ref T[] array, int index)
        {
            int arrayLength = array?.Length ?? 0;

            if (arrayLength <= 1 || index < 0 || index >= arrayLength)
            {
                array = new T[0];

                return;
            }

            T[] newArray = new T[arrayLength - 1];

            Array.Copy(array, newArray, arrayLength - 1);

            if (index < arrayLength)
            {
                Array.Copy(array, index + 1, newArray, index, arrayLength - (index + 1));
            }

            array = newArray;
        }

        /// <summary>
        /// Resizes an array while setting new indices' values to the provided default value.
        /// </summary>
        public static void ResizeAndFillArray<T>(ref T[] array, int newLength, T defaultValue)
        {
            if (newLength < 0)
            {
                throw new ArgumentException($"{nameof(newLength)} must be more than or equal to 0.");
            }

            if (array == null)
            {
                array = new T[newLength];

                for (int i = 0; i < newLength; i++)
                {
                    array[i] = defaultValue;
                }
            }

            int oldLength = array.Length;

            if (newLength == oldLength)
            {
                return;
            }

            var newArray = new T[newLength];

            if (newLength < array.Length)
            {
                for (int i = 0; i < newLength; i++)
                {
                    newArray[i] = array[i];
                }
            }
            else
            {
                for (int i = 0; i < oldLength; i++)
                {
                    newArray[i] = array[i];
                }

                for (int i = oldLength; i < newLength; i++)
                {
                    newArray[i] = defaultValue;
                }
            }

            array = newArray;
        }

        /// <summary> Reduce the array's size whenever it has null values at the end of it. </summary>
        public static bool TryShrinking<T>(ref T[] array) where T : class
        {
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] != null)
                {
                    int minimalSize = i + 1;

                    if (minimalSize < array.Length)
                    {
                        Array.Resize(ref array, minimalSize);

                        return true;
                    }

                    return false;
                }
            }

            array = new T[0];

            return true;
        }

        public static IDictionary<int, IEnumerable<T>> JaggedArraytoDictionary<T>(T[][] array)
        {
            var dictionary = new Dictionary<int, IEnumerable<T>>();

            for (int i = 0; i < array.Length; i++)
            {
                dictionary.Add(i, array[i]);
            }

            return dictionary;
        }

        public static T[][] DictionarytoJaggedArray<T>(IDictionary<int, IEnumerable<T>> dictionary)
        {
            var array = new T[][] { };
            ResizeAndFillArray(ref array, dictionary.Keys.Distinct().Count() + 1, new T[0]);

            foreach (var row in dictionary.Keys.Distinct())
            {
                array[row] = dictionary[row].ToArray();
            }

            return array;
        }
    }
}