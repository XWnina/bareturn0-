using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace CalcuTeaching
{
    public class CodeEvaluator : MonoBehaviour
    {
        public GameObject teachingPanel;
        public GameObject npcDialog;
        public TextMeshProUGUI npcText;
        public TMP_InputField codeInput;
        public TextMeshProUGUI questionText;
        public NpcDialogueController npcDialogueController; // ✅ 引入 NPC 对话控制器

        public bool hasSubmitted;
        public bool inputCorrect;

        private int _currentQuestionIndex;
        private const string InvalidResultFlag = "__invalid__";


        private readonly string[] _questions = new[]
        {
            "Please declare an int variable and assign it an integer. (e.g. int apple = 3;)",
            "Please declare a double variable and assign it a decimal number. (e.g. double price = 3.14;)",
            "Please declare a float variable and assign it a value like 2.5f. (e.g. float distance = 2.5f;)",
            "Try declaring a variable and assigning it a sum. (e.g. int sum = 2 + 3;)",
            "Give it a try! Declare a variable and assign it a subtraction result. (e.g. int result = 7 - 2;)",
            "Try writing a multiplication statement on your own! (e.g. int product = 4 * 5;)",
            "Now give it a go — write a division statement! (e.g. int result = 10 / 2;)",
            "Try writing a modulo expression yourself! (e.g. int result = 5 % 2;)",
            "If you have: int a = 10;Try writing a line of code that increments a variable",
            "If you have: int a = 10; Write a line of code that uses the decrement operator.",
            "Try combining what you've learned! (e.g. int apple = 3; int pear = 4; int sum = apple + pear;)"



        };


        private readonly string[] _answerPatterns = new[]
        {
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*;$",
            @"^double\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\.\d+\s*;$",
            @"^float\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\.\d+f\s*;$",
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*\+\s*\d+\s*;$",
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*-\s*\d+\s*;$",
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*\*\s*\d+\s*;$",
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*/\s*\d+\s*;$",
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*%\s*\d+\s*;$",
            @"^a\s*\+\+\s*;$",  // 只允许 a++
            @"^a\s*--\s*;$",    // 只允许 a--
            @"^int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*;\s*int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*\d+\s*;\s*int\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*[a-zA-Z_][a-zA-Z0-9_]*\s*\+\s*[a-zA-Z_][a-zA-Z0-9_]*\s*;$"




        };

        private void TriggerNextDialogue()
        {
            if (_currentQuestionIndex == 1)
            {
                npcDialogueController.StartDoubleTeachingDialogue(_currentQuestionIndex);
            }
            else if (_currentQuestionIndex == 2)
            {
                npcDialogueController.StartFloatTeachingDialogue();
            }
            else if (_currentQuestionIndex == 3)
            {
                npcDialogueController.StartAdditionTeachingDialogue();
            }
            else if (_currentQuestionIndex == 4)
            {
                npcDialogueController.StartSubtractionTeachingDialogue();
            }
            else if (_currentQuestionIndex == 5)
            {
                npcDialogueController.StartMultiplicationTeachingDialogue();
            }
            else if (_currentQuestionIndex == 6)
            {
                npcDialogueController.StartDivisionTeachingDialogue();
            }
            else if (_currentQuestionIndex == 7)
            {
                npcDialogueController.StartModuloTeachingDialogue();
            }
            else if (_currentQuestionIndex == 8)
            {
                npcDialogueController.StartIncrementTeachingDialogue();
            }
            else if (_currentQuestionIndex == 9)
            {
                npcDialogueController.StartDecrementTeachingDialogue();
            }
            else if (_currentQuestionIndex == 10)
            {
                npcDialogueController.StartMixedTeachingDialogue();
            }


        }



        public void OnSubmit()
        {
            string userCode = codeInput.text.Replace("\n", " ").Trim();
            Debug.Log("用户输入代码: [" + userCode + "]");

            teachingPanel.SetActive(false);
            npcDialog.SetActive(true);
            hasSubmitted = true;

            if (CheckCode(userCode))
            {
                inputCorrect = true;

                // 获取代码执行结果
                string result = GetVariableValue(); // 获取计算结果
                if (result == InvalidResultFlag)
                {
                    inputCorrect = false;
                    hasSubmitted = true; // 保持 true，等待 Update 里处理

                    teachingPanel.SetActive(false);
                    npcDialog.SetActive(true);
                    npcText.text = "Oops, that's incorrect. You typed: \"" + userCode + "\". Please try again!";
                    return;
                }

                // 在 NPC 的反馈中加入结果
                npcText.text = $"That's right! Well done.\nNow {result}";

                _currentQuestionIndex++;
                if (_currentQuestionIndex < _questions.Length)
                {
                    Invoke(nameof(TriggerNextDialogue), 2f);
                }
                else
                {
                    // 所有关卡完成后的结语
                    Invoke(nameof(ShowCompletionDialogue), 2f);
                }

            }
            else
            {
                inputCorrect = false;
                npcText.text = "Oops, that's incorrect. You typed: \"" + userCode + "\". Please try again!";
            }
        }



        public string GetVariableValue()
        {
            string result = codeInput.text.Trim();  // 默认返回用户输入的代码

            // 获取用户输入的代码
            string userInput = codeInput.text.Trim();
            try
            {
                if (_currentQuestionIndex == 0) // `int` 类型
                {
                    result =  CleanVariableExpression(userInput);
                }
                else if (_currentQuestionIndex == 1) // `double` 类型
                {
                    result = CleanVariableExpression(userInput);
                }
                else if (_currentQuestionIndex == 2) // `float` 类型
                {
                    result = CleanVariableExpression(userInput);
                }
                else if (_currentQuestionIndex == 3) // 加法
                {
                    result = EvaluateIntExpression(userInput);
                }
                else if (_currentQuestionIndex == 4) // 减法
                {
                    result = EvaluateIntExpression(userInput);
                }
                else if (_currentQuestionIndex == 5) // 乘法
                {
                    result = EvaluateIntExpression(userInput);
                }
                else if (_currentQuestionIndex == 6) // 除法
                {
                    result = EvaluateIntExpression(userInput);
                }
                else if (_currentQuestionIndex == 7) // 模运算
                {
                    result = EvaluateIntExpression(userInput);
                }
                else if (_currentQuestionIndex == 8) // 自增
                {
                    result = EvaluateIncrement(userInput);
                }
                else if (_currentQuestionIndex == 9) // 自减
                {
                    result = EvaluateDecrement(userInput);
                }
                else if (_currentQuestionIndex == 10) // 混合运算
                {
                    result = EvaluateMixedExpression(userInput);
                }

            }
            catch (FormatException ex)
            {
                // 捕获格式异常，提示用户重新输入
                Debug.LogError($"格式错误: {ex.Message}");
                npcText.text = "输入的代码格式不正确，请重新输入。";
                codeInput.text = "";  // 清空输入框
                return "Invalid input, please try again.";  // 提示用户重新输入
            }

            return result;
        }
    
    
        private string EvaluateIntExpression(string expression)
        {
            string variableName = "";
            int result = 0;

            Debug.Log($"Evaluating expression: {expression}");

            if (expression.Contains("="))
            {
                string[] parts = expression.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Log($"Parts after splitting by '=' : {string.Join(", ", parts)}");

                if (parts.Length == 2)
                {
                    // ✅ 解析左侧变量名，去掉类型声明（如 int）
                    string[] nameParts = parts[0].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length == 2)
                    {
                        variableName = nameParts[1];  // 只获取变量名
                    }
                    else
                    {
                        variableName = parts[0].Trim();  // 兜底
                    }
                    Debug.Log($"Variable name: {variableName}");

                    string valuePart = parts[1].Trim().Replace(";", "");
                    Debug.Log($"Value part (before evaluation): {valuePart}");

                    result = EvaluateExpression(valuePart);
                    Debug.Log($"Calculated result: {result}");
                    if (result == int.MinValue)
                    {
                        Debug.LogError("❌ 表达式计算失败，非法操作（如除以 0）");
                        return InvalidResultFlag;
                    }
                }
            }

            return $"{variableName} = {result}";
        }


        private int EvaluateExpression(string expression)
        {
            expression = expression.Replace(" ", "");  // 去掉空格
            Debug.Log($"Evaluating simple expression: {expression}");

            try
            {
                // 处理加法运算
                if (expression.Contains("+"))
                {
                    string[] parts = expression.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        Debug.Log($"Parsed addition: {parts[0].Trim()} + {parts[1].Trim()}");
                        return int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());  // 返回加法结果
                    }
                }

                // 处理减法运算
                if (expression.Contains("-"))
                {
                    string[] parts = expression.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        Debug.Log($"Parsed subtraction: {parts[0].Trim()} - {parts[1].Trim()}");
                        return int.Parse(parts[0].Trim()) - int.Parse(parts[1].Trim());  // 返回减法结果
                    }
                }

                // 处理乘法运算
                if (expression.Contains("*"))
                {
                    string[] parts = expression.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        Debug.Log($"Parsed multiplication: {parts[0].Trim()} * {parts[1].Trim()}");
                        return int.Parse(parts[0].Trim()) * int.Parse(parts[1].Trim());  // 返回乘法结果
                    }
                }

                // 处理除法运算
                if (expression.Contains("/"))
                {
                    string[] parts = expression.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        int left = int.Parse(parts[0].Trim());
                        int right = int.Parse(parts[1].Trim());

                        if (right == 0)
                        {
                            Debug.LogError("❌ 除数不能为 0！");
                            return int.MinValue; // 用于标记非法结果
                        }

                        Debug.Log($"Parsed division: {left} / {right}");
                        return left / right;
                    }
                }

                
                // 处理模运算（取余数）
                if (expression.Contains("%"))
                {
                    string[] parts = expression.Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        int left = int.Parse(parts[0].Trim());
                        int right = int.Parse(parts[1].Trim());

                        if (right == 0)
                        {
                            Debug.LogError("❌ 不能对 0 取余！");
                            return int.MinValue;  // 返回非法值
                        }

                        Debug.Log($"Parsed modulo: {left} % {right}");
                        return left % right;
                    }
                }


                // 如果是纯数字字符串（没有运算符），直接返回数字
                return int.Parse(expression);
            }
            catch (FormatException ex)
            {
                Debug.LogError($"格式错误: {ex.Message}");
                npcText.text = "输入的代码格式不正确，请重新输入。";
                return 0;  // 默认返回 0，提示错误
            }
        }

        private string EvaluateIncrement(string expression)
        {
            string variableName = expression.Replace("++", "").Replace(";", "").Trim();
            int value = 11;  // ✅ 直接设为 101
            return $"{variableName} = {value}";
        }


// 处理自减的函数
        private string EvaluateDecrement(string expression)
        {
            string variableName = expression.Replace("--", "").Replace(";", "").Trim();
            int value = 9;  // ✅ 直接设为 99
            return $"{variableName} = {value}";
        }
    
        private string EvaluateMixedExpression(string expression)
        {
            Debug.Log($"混合运算表达式: {expression}");

            string[] lines = expression.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 3)
            {
                string var1Decl = lines[0].Trim();
                string var2Decl = lines[1].Trim();
                string sumDecl = lines[2].Trim();

                Dictionary<string, int> vars = new Dictionary<string, int>();

                try
                {
                    foreach (string decl in new[] { var1Decl, var2Decl })
                    {
                        string[] parts = decl.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        string varName = parts[0].Replace("int", "").Trim();
                        int value = int.Parse(parts[1].Trim());
                        vars[varName] = value;
                    }

                    string[] sumParts = sumDecl.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    string sumVarName = sumParts[0].Replace("int", "").Trim();

                    string[] operands = sumParts[1].Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                    string op1 = operands[0].Trim();
                    string op2 = operands[1].Trim();

                    // ✅ 检查变量是否存在于字典中
                    if (!vars.ContainsKey(op1) || !vars.ContainsKey(op2))
                    {
                        Debug.LogError($"❌ 未找到变量：{op1} 或 {op2}");
                        return InvalidResultFlag;
                    }

                    int result = vars[op1] + vars[op2];

                    return $"{sumVarName} = {result}";
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ 解析混合表达式时出错: {ex.Message}");
                    return InvalidResultFlag;
                }
            }

            return  InvalidResultFlag;
        }






    
        private string CleanVariableExpression(string expression)
        {
            Debug.Log($"原始输入: {expression}");  // 打印原始输入表达式

            // 去掉变量声明中的数据类型（如 int、double、float 等）
            string[] parts = expression.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Log($"分割后的部分: {string.Join(", ", parts)}");  // 打印分割后的各部分

            if (parts.Length == 2)
            {
                string variableName = parts[0].Trim(); // 获取变量名部分
                string valuePart = parts[1].Trim();    // 获取赋值部分

                // 去掉数据类型，例如 "int", "float", "double"
                variableName = Regex.Replace(variableName, @"\b(int|float|double)\b", "").Trim();
                Debug.Log($"清理后的变量名: {variableName}");  // 打印清理后的变量名

                // 去掉尾部的分号（如果有的话）
                if (valuePart.EndsWith(";"))
                {
                    valuePart = valuePart.Substring(0, valuePart.Length - 1).Trim();  // 去掉结尾的分号
                    Debug.Log($"去掉分号后的赋值部分: {valuePart}");  // 打印去掉分号后的结果
                }

                // 如果是 float 类型，去掉末尾的 "f"
                if (valuePart.EndsWith("f"))
                {
                    valuePart = valuePart.Substring(0, valuePart.Length - 1).Trim();  // 去掉末尾的 "f"
                    Debug.Log($"去掉 'f' 后的赋值部分: {valuePart}");  // 打印去掉 "f" 后的结果
                }

                // 返回清理后的表达式，例如："a = 3"
                string cleanedExpression = $"{variableName} = {valuePart}";
                Debug.Log($"清理后的表达式: {cleanedExpression}");  // 打印最终结果
                return cleanedExpression;
            }

            Debug.Log($"没有 '=' 符号，原始表达式返回: {expression}");  // 没有 '=' 时返回原始表达式
            return expression;  // 如果没有 '='，直接返回原始表达式
        }

        private bool CheckCode(string code)
        {
            if (_currentQuestionIndex >= _answerPatterns.Length) return false;
            string pattern = _answerPatterns[_currentQuestionIndex];

            // 如果代码格式匹配，则返回 true
            if (Regex.IsMatch(code, pattern))
            {
                return true;
            }
    
            // 如果代码格式不匹配，则返回 false
            return false;
        }
    
    


        private void Update()
        {
            // 检查 Ctrl + Enter（适合提交）
            if (teachingPanel.activeSelf && Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                Debug.Log("Detected Ctrl+Enter, submitting code...");
                OnSubmit();
            }
            if (hasSubmitted && !inputCorrect && !teachingPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                npcDialog.SetActive(false);
                teachingPanel.SetActive(true);
                hasSubmitted = false;
            }

        }

        public void PrepareNextQuestion(int index)
        {
            npcDialog.SetActive(false);
            teachingPanel.SetActive(true);
            hasSubmitted = false;
            inputCorrect = false;
            codeInput.text = "";
            questionText.text = _questions[index];
        }

        public int GetCurrentQuestionIndex()
        {
            return _currentQuestionIndex;
        }
        private void ShowCompletionDialogue()
        {
            // 显示最终对话
            npcText.text = "Incredible! You've learned all the basic calculations.\\n";

            // 设置全局变量
            PlayerPrefs.SetString("PreviousScene", "calcuTeaching");
            PlayerPrefs.Save(); // ✅ 推荐保存
            // ✅ 立即读取并打印验证
            string testValue = PlayerPrefs.GetString("PreviousScene", "NotFound");
            Debug.Log("✅ PreviousScene 存储值为: " + testValue);
        
        
            StartCoroutine(UpdateProgress(3));  // 你可以根据当前关卡传入对应数字
        
            // 跳转到 draftMap 场景（延迟一两秒更自然）
            Invoke(nameof(LoadDraftMapScene), 3f); // 3秒后跳转
        }
        private IEnumerator UpdateProgress(int progress)
        {
            string token = PlayerPrefs.GetString("token", "");
            string saveName = PlayerPrefs.GetString("currentSaveName", "");
            Debug.Log($"📂 当前存档名为: {saveName}");

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("❌ No Token Found! Player is not authenticated.");
                yield break;
            }

            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogError("❌ No SaveName Found! Cannot update progress.");
                yield break;
            }

            string url = $"http://localhost:3000/savefiles/{saveName}/updateProgress";
            string jsonData = JsonUtility.ToJson(new ProgressWrapper(progress));
            Debug.Log("📤 发送的 JSON 数据：" + jsonData);

            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
            {
                request.method = "PUT";
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + token);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Progress updated successfully: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("❌ Failed to update progress: " + request.error);
                }
            }
        }

        [Serializable]
        private class ProgressWrapper
        {
            public int progress;
            public ProgressWrapper(int p)
            {
                this.progress = p;
            }
        }


        private void LoadDraftMapScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("draftMap");
        }


    }
} 
