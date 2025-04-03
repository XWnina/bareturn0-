using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CalcuProblemPage
{
    public class CodeEvaluator
    {
        private readonly Dictionary<string, double> _variables = new();

        public bool TryEvaluate(string code, out double result, out CodeErrorType errorType)
        {
            Debug.Log("=== 开始解析用户代码 ===");
            Debug.Log(code);

            _variables.Clear();
            result = 0.0;
            errorType = CodeErrorType.None;

            string[] lines = code.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);
            string lastVar = null;
            string lastExpression = null;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                Debug.Log("解析行: " + line);

                // 判断是否是声明语句
                bool isDeclaration = line.StartsWith("int ") || line.StartsWith("double ");
                string codeLine = isDeclaration ? line.Substring(line.IndexOf(' ') + 1).Trim() : line;

                // 拆分变量与表达式
                string[] parts = codeLine.Split('=');
                if (parts.Length != 2)
                {
                    Debug.LogError("无效格式: " + line);
                    errorType = CodeErrorType.SyntaxError;
                    return false;
                }

                string varName = parts[0].Trim();
                string expression = parts[1].Trim();

                // 非声明赋值时，变量必须已声明
                if (!isDeclaration && !_variables.ContainsKey(varName))
                {
                    Debug.LogError($"变量 [{varName}] 未声明就赋值: {line}");
                    errorType = CodeErrorType.UndeclaredVariable;
                    return false;
                }

                // 检查表达式中的所有变量是否都已声明
                if (!ExpressionContainsOnlyKnownVariables(expression))
                {
                    Debug.LogError($"表达式包含未声明变量: {expression}");
                    errorType = CodeErrorType.UndeclaredVariable;
                    return false;
                }

                // 计算表达式
                if (!TryEvaluateExpression(expression, out double value))
                {
                    Debug.LogError("无法求值表达式: " + expression);
                    errorType = CodeErrorType.SyntaxError;
                    return false;
                }

                // 如果是 int 声明则向下取整（仍保留为 double 类型）
                if (line.StartsWith("int "))
                {
                    value = Math.Floor(value);
                }

                _variables[varName] = value;
                Debug.Log($"变量 [{varName}] = {value}");

                lastVar = varName;
                lastExpression = expression;
            }

            // 检查最后一行是否是硬编码常数
            if (!string.IsNullOrEmpty(lastExpression) &&
                Regex.IsMatch(lastExpression, @"^\d+(\.\d+)?$"))
            {
                Debug.LogError($"❌ 最后一行不能是硬编码常数: {lastExpression}");
                errorType = CodeErrorType.HardcodedConstant;
                return false;
            }

            if (!string.IsNullOrEmpty(lastVar) && _variables.TryGetValue(lastVar, out result))
            {
                return true;
            }

            errorType = CodeErrorType.SyntaxError;
            return false;
        }

        private bool TryEvaluateExpression(string expr, out double result)
        {
            try
            {
                // 替换表达式中的变量为实际值
                foreach (var pair in _variables)
                {
                    expr = Regex.Replace(
                        expr,
                        $@"\b{Regex.Escape(pair.Key)}\b",
                        pair.Value.ToString(CultureInfo.InvariantCulture)
                    );
                }

                Debug.Log("最终表达式: " + expr);

                // 使用 DataTable 计算表达式
                DataTable table = new();
                object evalResult = table.Compute(expr, "");

                result = Convert.ToDouble(evalResult, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("表达式计算失败: " + expr);
                Debug.LogException(e);
                result = 0.0;
                return false;
            }
        }

        private bool ExpressionContainsOnlyKnownVariables(string expression)
        {
            MatchCollection matches = Regex.Matches(expression, @"[a-zA-Z_]\w*");

            foreach (Match match in matches)
            {
                if (!_variables.ContainsKey(match.Value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
