using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TableConfig
{
    public static class TableParser<T> where T : BaseModel
    {
        private static char[] sep_1 = {',', '|'}; //数据分隔符，',' 或者 '|'，不可同时存在，使用'|'时可不加双引号
        private static char[] sep_2 = {':'}; //KV数据分隔符

        //表路径
        private const string TABLE_PATH = "csv/{0}";

        //字段信息行(当前.py中只导出字段与有效数据 所以为1 *这行之下必须数据)
        private const int PROPERTY_ROW = 1;

        private static string ReadFile(string PathName)
        {
            string strs = File.ReadAllText(PathName);

            return strs;
        }

        public static void Parse(string tableName, Action<T[]> onComplete)
        {
            string path = string.Format(TABLE_PATH, tableName);
            string text = "";
#if UNITY_STANDALONE && !UNITY_EDITOR
            text = ReadFile("IW_Configs/ExcelToCsv/" + tableName + ".csv");
#else
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            Debug.AssertFormat(textAsset != null, "{0} not found.", path);
            if (textAsset != null)
            {
                text = textAsset.text;
            }
#endif
            ParseByPath(text, onComplete);
        }

        public static T[] ParseByPath(string text, Action<T[]> onComplete)
        {
            T[] list = ParseText(text);
            onComplete?.Invoke(list);

            return list;
        }

        public static T[] ParseLocalFile(string path, Action<T[]> onComplete)
        {
            string text = File.ReadAllText(path);

            T[] list = ParseText(text);
            onComplete?.Invoke(list);

            return list;
        }

        public static T[] ParseText(string text)
        {
            string[] lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(lines.Length > PROPERTY_ROW, "table row number error or no data.");

            //分析字段信息
            Dictionary<int, FieldInfo> propertyInfos = GetGetPropertyInfos<T>(lines[PROPERTY_ROW - 1]);

            //分析数据
            T[] list = new T[lines.Length - PROPERTY_ROW];
            for (int i = PROPERTY_ROW; i < lines.Length; i++)
            {
                T t = ParseObject(lines[i], propertyInfos);
                if (t == null) continue;
                list[i - PROPERTY_ROW] = t;
            }

            return list;
        }

        static void ParsePropertyValue(T obj, FieldInfo fieldInfo, string valueStr)
        {
            System.Object val;
            if (string.IsNullOrEmpty(valueStr) || valueStr.ToLower() == "null")
            {
                val = default(T);
                fieldInfo.SetValue(obj, val);
                obj.ParsePerValue.Add(fieldInfo.Name, valueStr);
                return;
            }

            if (fieldInfo.FieldType == typeof(int))
                val = valueStr.StartsWith("0x")
                    ? int.Parse(valueStr.Remove(0, 2).Replace("_", ""), NumberStyles.HexNumber)
                    : int.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(long))
                val = long.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(short))
                val = short.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(byte))
                val = byte.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(float))
                val = float.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(double))
                val = double.Parse(valueStr);
            else if (fieldInfo.FieldType == typeof(bool))
                val = int.Parse(valueStr) != 0;
            else if (fieldInfo.FieldType == typeof(string))
                val = valueStr;
            else if (fieldInfo.FieldType == typeof(Vector2))
                val = SplitStringToVector2(valueStr, sep_1);
            else if (fieldInfo.FieldType == typeof(Vector3))
                val = SplitStringToVector3(valueStr, sep_1);
            else if (fieldInfo.FieldType == typeof(string[]))
                val = SplitStringToStringArray(valueStr, sep_1);
            else if (fieldInfo.FieldType == typeof(int[]))
                val = SplitStringToIntArray(valueStr, sep_1);
            else if (fieldInfo.FieldType == typeof(float[]))
                val = SplitStringToFloatArray(valueStr, sep_1);
            else if (fieldInfo.FieldType == typeof(Vector2[]))
                val = SplitStringToVector2Array(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType == typeof(Vector3[]))
                val = SplitStringToVector3Array(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType == typeof(Dictionary<int, int>))
                val = SplitStringToDictInt_Int(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType == typeof(Dictionary<int, float>))
                val = SplitStringToDictInt_Float(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType == typeof(Dictionary<string, int>))
                val = SplitStringToDictStr_Int(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType == typeof(Dictionary<string, float>))
                val = SplitStringToDictStr_Float(valueStr, sep_1, sep_2);
            else if (fieldInfo.FieldType.IsEnum)
                val = Enum.Parse(fieldInfo.FieldType, valueStr);
            else
                val = default(T);
            obj.ParsePerValue.Add(fieldInfo.Name, valueStr);
            fieldInfo.SetValue(obj, val);
        }

        #region SplitString

        static Vector2 SplitStringToVector2(string valueStr, char[] separator)
        {
            string[] datas = valueStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return new Vector2(float.Parse(datas[0]), float.Parse(datas[1]));
        }

        static Vector3 SplitStringToVector3(string valueStr, char[] separator)
        {
            string[] datas = valueStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return new Vector3(float.Parse(datas[0]), float.Parse(datas[1]), float.Parse(datas[2]));
        }

        static string[] SplitStringToStringArray(string valueStr, char[] separator)
        {
            string[] datas = valueStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string[] array = new string[datas.Length];
            for (int i = 0; i < datas.Length; i++)
                array[i] = valueStr.StartsWith("0x")
                    ? datas[i].Remove(0, 2).Replace("_", "")
                    : datas[i];
            return array;
        }

        static int[] SplitStringToIntArray(string valueStr, char[] separator)
        {
            string[] datas = valueStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            int[] array = new int[datas.Length];
            for (int i = 0; i < datas.Length; i++)
                array[i] = valueStr.StartsWith("0x")
                    ? int.Parse(datas[i].Remove(0, 2).Replace("_", ""), NumberStyles.HexNumber)
                    : int.Parse(datas[i]);
            return array;
        }

        static float[] SplitStringToFloatArray(string valueStr, char[] separator)
        {
            string[] datas = valueStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            float[] array = new float[datas.Length];
            for (int i = 0; i < datas.Length; i++)
                array[i] = float.Parse(datas[i]);
            return array;
        }

        static Vector2[] SplitStringToVector2Array(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Vector2[] array = new Vector2[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                array[i] = SplitStringToVector2(datas[i], sep_2);
            }

            return array;
        }

        static Vector3[] SplitStringToVector3Array(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Vector3[] array = new Vector3[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                array[i] = SplitStringToVector3(datas[i], sep_2);
            }

            return array;
        }

        static Dictionary<int, int> SplitStringToDictInt_Int(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < datas.Length; i++)
            {
                var tmp = SplitStringToIntArray(datas[i], sep_2);
                dict.Add(tmp[0], tmp[1]);
            }

            return dict;
        }

        static Dictionary<int, float> SplitStringToDictInt_Float(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<int, float> dict = new Dictionary<int, float>();
            for (int i = 0; i < datas.Length; i++)
            {
                var tmp = SplitStringToFloatArray(datas[i], sep_2);
                dict.Add((int) tmp[0], tmp[1]);
            }

            return dict;
        }

        static Dictionary<string, int> SplitStringToDictStr_Int(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < datas.Length; i++)
            {
                string[] strs = datas[i].Split(sep_2, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(strs[0], Convert.ToInt32(strs[1]));
            }

            return dict;
        }

        static Dictionary<string, float> SplitStringToDictStr_Float(string valueStr, char[] sep_1, char[] sep_2)
        {
            string[] datas = valueStr.Split(sep_1, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, float> dict = new Dictionary<string, float>();
            for (int i = 0; i < datas.Length; i++)
            {
                string[] strs = datas[i].Split(sep_2, StringSplitOptions.RemoveEmptyEntries);
                dict.Add(strs[0], Convert.ToSingle(strs[1]));
            }

            return dict;
        }

        #endregion

        static Dictionary<int, FieldInfo> GetGetPropertyInfos<T>(string memberLine)
        {
            Type tType = typeof(T);

            string[] members = memberLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Dictionary<int, FieldInfo> propertyInfos = new Dictionary<int, FieldInfo>();

            //表中字段信息对应Model字段
            for (int i = 0; i < members.Length; i++)
            {
                //在脚本内获取某个字段
                FieldInfo fieldInfo = tType.GetField(members[i]);
                if (fieldInfo == null) continue;
                propertyInfos[i] = fieldInfo;
            }

            //表中字段信息未查找到有效数据,就以Model字段顺序为字段信息
            if (propertyInfos.Count == 0)
            {
                FieldInfo[] fieldInfos = tType.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                    propertyInfos[i] = fieldInfos[i];
            }

            return propertyInfos;
        }

        static T ParseObject(string line, Dictionary<int, FieldInfo> propertyInfos)
        {
            T obj = Activator.CreateInstance<T>();
            List<string> values = ParseLine(line);

            foreach (var item in propertyInfos)
            {
                if (item.Key >= values.Count) break;

                string _value = values[item.Key];
                try
                {
                    ParsePropertyValue(obj, item.Value, _value);
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("ParseError: Column={0} Name={1} Want={2} Get={3} Line={4}",
                        item.Key + 1,
                        item.Value.Name,
                        item.Value.FieldType.Name,
                        _value, line));
                    Debug.LogError(e);
                }
            }

            return obj;
        }

        private const char _csvSeparator = ',';

        private static List<string> ParseLine(string line)
        {
            StringBuilder _columnBuilder = new StringBuilder();
            List<string> Fields = new List<string>();
            bool inColumn = false; //是否是在一个列元素里
            bool inQuotes = false; //是否需要转义
            bool isNotEnd = false; //读取完毕未结束转义
            _columnBuilder.Remove(0, _columnBuilder.Length);

            // 遍历行中每一个字符
            for (int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                // 当前不在列中
                if (!inColumn)
                {
                    // 如果当前字符是双引号 字段需要转义
                    inColumn = true;
                    if (character == '"')
                    {
                        inQuotes = true;
                        continue;
                    }
                }

                // 需要转义
                if (inQuotes)
                {
                    if ((i + 1) == line.Length) //这个字符已经结束了整行
                    {
                        if (character == '"') //正常转义结束，且该行已经结束
                        {
                            inQuotes = false;
                            continue; //当前字符不用添加，跳出后直结束后会添加该元素
                        }
                        else //异常结束，转义未收尾
                        {
                            isNotEnd = true;
                        }
                    }
                    else if (character == '"' && line[i + 1] == _csvSeparator) //结束转义，且后面有可能还有数据
                    {
                        inQuotes = false;
                        inColumn = false;
                        i++; //跳过下一个字符
                    }
                    else if (character == '"' && line[i + 1] == '"') //双引号转义
                    {
                        i++; //跳过下一个字符
                        if (line.Length - 1 == i) //异常结束，转义未收尾
                        {
                            isNotEnd = true;
                        }
                    }
                    else if (character == '"') //双引号单独出现（这种情况实际上已经是格式错误，为了兼容可暂时不处理）
                    {
                        throw new Exception(string.Format("[{0}]:格式错误，错误的双引号转义 near [{1}] ", i, line));
                    }

                    //其他情况直接跳出，后面正常添加
                }
                else if (character == _csvSeparator)
                    inColumn = false;

                // If we are no longer in the column clear the builder and add the columns to the list
                if (!inColumn) //结束该元素时inColumn置为false，并且不处理当前字符，直接进行Add
                {
                    Fields.Add(_columnBuilder.ToString());
                    _columnBuilder.Remove(0, _columnBuilder.Length);
                }
                else // append the current column
                    _columnBuilder.Append(character);
            }

            // If we are still inside a column add a new one
            // （标准格式一行结尾不需要逗号结尾，而上面for是遇到逗号才添加的，为了兼容最后还要添加一次）
            if (inColumn)
            {
                Fields.Add(_columnBuilder.ToString());
            }
            //如果inColumn为false，说明已经添加，因为最后一个字符为分隔符，所以后面要加上一个空元素
            //另外一种情况是line为""空行，（空行也是一个空元素,一个逗号是2个空元素），正好inColumn为默认值false，在此处添加一空元素
            //当前导出规则不会出现
            else
            {
                Fields.Add("");
            }

            Debug.AssertFormat(!isNotEnd, "格式错误 转义字符结束 缺少 \" line: {0}", line);
            return Fields;
        }
    }
}