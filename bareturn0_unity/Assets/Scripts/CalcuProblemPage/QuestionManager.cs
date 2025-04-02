using System;
using System.Collections.Generic;
using UnityEngine;

namespace CalcuProblemPage
{
    public class QuestionManager : MonoBehaviour
    {
        [Serializable]
        public class Question
        {
            public string text;
            public float answer;
        }

        private readonly List<string> _productNames = new()
        {
            "blank cards", "runes", "apples", "clothes", "daggers", "spears",
            "iron blocks", "silver blocks", "gold blocks", "beer", "healing potions"
        };

        private readonly List<string> _characterNames = new()
        {
            "Smith", "Natasha", "Charlie", "Nina", "Yuchuan", "Mary"
        };

        private readonly Dictionary<string, string> _characterGender = new()
        {
            { "Smith", "male" },
            { "Natasha", "female" },
            { "Charlie", "female" },
            { "Nina", "female" },
            { "Yuchuan", "male" },
            { "Mary", "female" }
        };

        private List<Question> _allQuestions = new();
        private int _currentIndex;

        void Start()
        {
            GenerateAllQuestions();
        }

        public void GenerateAllQuestions()
        {
            _allQuestions.Clear();
            int charIndex = 0;

            for (int level = 1; level <= 3; level++)
            {
                for (int i = 0; i < 2; i++)
                {
                    string character = _characterNames[charIndex % _characterNames.Count];
                    Question q = GenerateQuestion(level, character);
                    _allQuestions.Add(q);
                    charIndex++;
                }
            }

            _currentIndex = 0;
        }

        public Question GenerateQuestion(int level, string characterName)
        {
            string gender = _characterGender[characterName];
            string heShe = gender == "male" ? "he" : "she";
            string himHer = gender == "male" ? "him" : "her";

            string item1 = GetRandomItem();
            string item2 = GetRandomItem(exclude: item1);

            int quantity1 = UnityEngine.Random.Range(1, 5);
            int quantity2 = UnityEngine.Random.Range(1, 4);

            float price1 = RandomPrice();
            float price2 = RandomPrice();
            float discount = UnityEngine.Random.Range(0.6f, 0.9f);
            string questionText;
            float answer;

            if (level == 1)
            {
                answer = price1 * quantity1;
                questionText = $"{characterName} bought {quantity1} {item1} today, each for {price1:F2} coins. How much should I charge {himHer}?";
            }
            else if (level == 2)
            {
                answer = price1 * quantity1 + price2 * quantity2;
                questionText = $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each and {quantity2} {item2} at {price2:F2} coins each. How much should I charge {himHer}?";
            }
            else // level == 3
            {
                float originalTotal = price1 * quantity1;
                float discountedTotal = Mathf.Round(originalTotal * discount * 100f) / 100f;
                int discountPercent = Mathf.RoundToInt(discount * 100);

                if (UnityEngine.Random.value < 0.5f)
                {
                    float payment = discountedTotal + RandomPrice(2f, 10f);
                    payment = Mathf.Round(payment * 100f) / 100f;
                    answer = Mathf.Round((payment - discountedTotal) * 100f) / 100f;

                    questionText = $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each with a {discountPercent}% discount. {Capitalize(heShe)} gave me {payment:F2} coins. How much change should I give {himHer}?";
                }
                else
                {
                    answer = discountedTotal;
                    questionText = $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each with a {discountPercent}% discount. How much should I charge {himHer}?";
                }
            }

            answer = Mathf.Round(answer * 100f) / 100f;

            return new Question { text = questionText, answer = answer };
        }

        private string GetRandomItem(string exclude = "")
        {
            string item;
            do
            {
                item = _productNames[UnityEngine.Random.Range(0, _productNames.Count)];
            } while (item == exclude);
            return item;
        }

        private float RandomPrice(float min = 1.0f, float max = 30.0f)
        {
            float value = UnityEngine.Random.Range(min, max);
            return Mathf.Round(value * 100f) / 100f;
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public string GetCurrentQuestionText()
        {
            if (_currentIndex < _allQuestions.Count)
                return _allQuestions[_currentIndex].text;
            return "No more questions.";
        }

        public float GetCurrentAnswer()
        {
            if (_currentIndex < _allQuestions.Count)
                return _allQuestions[_currentIndex].answer;
            return -1f;
        }

        public void MoveToNextQuestion()
        {
            _currentIndex++;
        }

        public bool HasMoreQuestions()
        {
            return _currentIndex < _allQuestions.Count;
        }

        public int GetCurrentDifficulty()
        {
            if (_currentIndex < 2) return 1;
            if (_currentIndex < 4) return 2;
            return 3;
        }
    }
}