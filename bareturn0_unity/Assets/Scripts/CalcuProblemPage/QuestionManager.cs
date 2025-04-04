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
            public double answer;
        }

        public bool skipToLast = false;

        [Header("Demo Mode(Press \u2192 to skip)")]
        public bool demoMode = false; // ✅ Inspector 中可勾选


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
            if (skipToLast)
            {
                _currentIndex = 5; // ✅ 跳到第6题
            }
            else
            {
                _currentIndex = 0;
            }
        }

        public void GenerateAllQuestions()
        {
            _allQuestions.Clear();
            int charIndex = 0;

            for (int level = 1; level <= 3; level++)
            {
                int numQuestions = 2; // ✅ demo 模式下每个 level 出一题

                for (int i = 0; i < numQuestions; i++)
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

            double price1 = RoundToTwoDecimals(RandomPrice());
            double price2 = RoundToTwoDecimals(RandomPrice());
            double discount = RandomDiscount();


            string questionText;
            double answer;

            if (level == 1)
            {
                answer = price1 * quantity1;
                questionText =
                    $"{characterName} bought {quantity1} {item1} today, each for {price1:F2} coins. How much should I charge {himHer}?";
            }
            else if (level == 2)
            {
                answer = price1 * quantity1 + price2 * quantity2;
                questionText =
                    $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each and {quantity2} {item2} at {price2:F2} coins each. How much should I charge {himHer}?";
            }
            else // level == 3
            {
                double originalTotal = price1 * quantity1;
                double discountedTotal = originalTotal * discount;
                int discountPercent = Mathf.RoundToInt((float)(discount * 100));

                if (UnityEngine.Random.value < 0.5f)
                {
                    double payment = RoundToTwoDecimals(discountedTotal + RandomPrice(2f, 10f));
                    answer = payment - discountedTotal;

                    questionText =
                        $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each with a {discountPercent}% discount. {Capitalize(heShe)} gave me {payment:F2} coins. How much change should I give {himHer}?";
                }
                else
                {
                    answer = discountedTotal;
                    questionText =
                        $"{characterName} bought {quantity1} {item1} at {price1:F2} coins each with a {discountPercent}% discount. How much should I charge {himHer}?";
                }
            }

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

        private double RandomPrice(double min = 1.0, double max = 30.0)
        {
            return UnityEngine.Random.Range((float)min, (float)max);
        }

        private double RoundToTwoDecimals(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
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

        public double GetCurrentAnswer()
        {
            if (_currentIndex < _allQuestions.Count)
                return _allQuestions[_currentIndex].answer;
            return -1.0;
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

        private double RandomDiscount(double min = 0.6, double max = 0.9)
        {
            return Math.Round(UnityEngine.Random.Range((float)min, (float)max), 4);
        }

        public string GetCurrentCharacterGender()
        {
            int indexToCheck = Mathf.Clamp(_currentIndex, 0, _allQuestions.Count - 1);

            string text = _allQuestions[indexToCheck].text;

            foreach (var entry in _characterGender)
            {
                if (text.Contains(entry.Key))
                    return entry.Value;
            }

            return "male"; // fallback
        }
    }
}