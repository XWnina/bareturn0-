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
        private Dictionary<string, double> _variables = new();

        public bool TryEvaluate(string code, out double result)
        {
            Debug.Log("=== 开始解析用户代码 ===");
            Debug.Log(code);

            _variables.Clear();
            result = 0;

            string[] lines = code.Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries);
            string lastLine = null;
            string lastVar = null;
            string lastExpression = null;

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                Debug.Log("解析行: " + line);

                bool isDeclaration = line.StartsWith("int ") || line.StartsWith("double ");
                string codeLine = isDeclaration ? line.Substring(line.IndexOf(' ') + 1).Trim() : line;

                string[] parts = codeLine.Split('=');
                if (parts.Length != 2)
                {
                    Debug.LogError("无效格式: " + line);
                    return false;
                }

                string varName = parts[0].Trim();
                string expression = parts[1].Trim();

                lastLine = line;
                lastVar = varName;
                lastExpression = expression;

                if (!TryEvaluateExpression(expression, out double value))
                {
                    Debug.LogError("无法求值表达式: " + expression);
                    return false;
                }

                if (isDeclaration && line.StartsWith("int "))
                {
                    value = Math.Floor(value); // int 转换
                }

                _variables[varName] = value;
                Debug.Log($"变量 [{varName}] = {value}");
            }

            // ❌ 不允许最后一行是单纯的字面量（不是表达式）
            if (!string.IsNullOrEmpty(lastExpression))
            {
                if (Regex.IsMatch(lastExpression, @"^\d+(\.\d+)?$"))
                {
                    Debug.LogError($"❌ 最后一行不能是硬编码常数: {lastExpression}");
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(lastVar) && _variables.ContainsKey(lastVar))
            {
                result = _variables[lastVar];
                return true;
            }

            return false;
        }

        private bool TryEvaluateExpression(string expr, out double result)
        {
            try
            {
                foreach (var pair in _variables)
                {
                    expr = Regex.Replace(expr, $@"\b{Regex.Escape(pair.Key)}\b", pair.Value.ToString(CultureInfo.InvariantCulture));
                }

                Debug.Log("最终表达式: " + expr);

                DataTable table = new();
                object evalResult = table.Compute(expr, "");
                result = Convert.ToDouble(evalResult, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("表达式计算失败: " + expr);
                Debug.LogException(e);
                result = 0;
                return false;
            }
        }
    }
}
