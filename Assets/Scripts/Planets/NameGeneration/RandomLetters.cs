using System.Text;
using System;
using UnityEngine;

public class RandomLetters {
    System.Random random = new System.Random();
    int numberToGenerate;

    public RandomLetters(int numberOfLetters) {
        numberToGenerate = numberOfLetters;
    }

    public string generateString() {
        StringBuilder result = new StringBuilder();
        for (int i=0; i<numberToGenerate; i++) {
            int rInt = random.Next(0, 25);
            char newLetter = (char)(rInt + 65);
            result.Append(newLetter);
        }
        return result.ToString();
    }
}