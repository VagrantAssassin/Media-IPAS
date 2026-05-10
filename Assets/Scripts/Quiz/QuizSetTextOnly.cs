using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Media IPAS/Quiz/Quiz Set (Text Only)", fileName = "QuizSetTextOnly")]
public class QuizSetTextOnly : ScriptableObject
{
    [Serializable]
    public class Question
    {
        [TextArea(2, 6)] public string questionText;

        [TextArea(1, 3)] public string choiceA;
        [TextArea(1, 3)] public string choiceB;
        [TextArea(1, 3)] public string choiceC;
        [TextArea(1, 3)] public string choiceD;

        [Tooltip("Jawaban benar: 0=A, 1=B, 2=C, 3=D (sebelum diacak)")]
        [Range(0, 3)] public int correctChoiceIndex;
    }

    public List<Question> questions = new();
}