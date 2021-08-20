using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Scripts.BasicModules.Misc
{
    public static class UnityUtil
    {
        public static void SetCanvasGroupActive(this CanvasGroup canvasGroup, bool active, bool blocksRaycasts = true)
        {
            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.blocksRaycasts = active && blocksRaycasts;
        }

        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            if (null != renderer)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            if (null != image)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        public static void SetAlpha(this RawImage image, float alpha)
        {
            if (null != image)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        public static void SetAlpha(this Text text, float alpha)
        {
            if (null != text)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        public static void SetAlpha(this TextMesh text, float alpha)
        {
            if (null != text)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        public static void SetColor(this Image image, Color color, float alpha)
        {
            if (null != image)
            {
                color.a = alpha;
                image.color = color;
            }
        }

        public static Color FormatColor(string str)
        {
            int color = int.Parse(str, NumberStyles.AllowHexSpecifier);
            var r = ((color >> 16) & 0xff) / 255f;
            var g = ((color >> 8) & 0xff) / 255f;
            var b = (color & 0xff) / 255f;
            return new Color(r, g, b, 255);
        }

        public static void SetColor(this Text text, string hex, float alpha = 1f)
        {
            Color color = new Color();
            color.a = alpha;
            ColorUtility.TryParseHtmlString(hex, out color);
            text.color = color;
        }

        public static string Md5Sum(this string strToEncrypt)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            MD5CryptoServiceProvider md5 =
                new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static bool IsPointerOverGameObject()
        {
            if (EventSystem.current)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = Input.mousePosition;

                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                return results.Count > 0;
            }

            return false;
        }

        public static string NumberHandleZero(this int value)
        {
            return value <= 0 ? "--" : value.ToString();
        }

        public static string NumberHandleZeroFormat(this int value)
        {
            return value <= 0 ? "--" : NumberStr(value);
        }

        public static string NumberHandleZeroFormat(this long value)
        {
            return value <= 0 ? "--" : NumberStr(value);
        }

        public static string NumberStr(long value)
        {
            return $"{value:#,0}";
        }

        public static string NumberFormat(long number)
        {
            if (number < 0)
            {
                return $"-{NumberFormat(-number)}";
            }

            if (number == 0)
            {
                return "0";
            }

            if (number < 10000)
            {
                return number.ToString();
            }

            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long) Math.Pow(10, (int) Math.Max(0, Math.Log10(number) - 2));
            number = number / i * i;

            if (number >= 100000000)
            {
                return (number / 1000000D).ToString("0.#M");
            }

            if (number >= 1000000)
            {
                return (number / 1000000D).ToString("0.##M");
            }

            if (number >= 100000)
            {
                return (number / 1000D).ToString("0.#k");
            }

            if (number >= 10000)
            {
                return (number / 1000D).ToString("0.##k");
            }

            return number.ToString();
        }

        public static void SetIntList(string key, List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                PlayerPrefs.DeleteKey(key);
                return;
            }

            var value = string.Join(",", numbers);
            PlayerPrefs.SetString(key, value);
        }

        public static List<int> GetIntList(string key)
        {
            if (!PlayerPrefs.HasKey(key) || string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
            {
                return new List<int>();
            }

            var value = PlayerPrefs.GetString(key);
            return value.Split(',').Select(int.Parse).ToList();
        }

        public static void SafeKillDoTween(this Sequence sequence)
        {
            if (sequence != null && sequence.active)
            {
                sequence.Kill();
            }
        }

        public static bool IsNull(this object @object)
        {
            return @object == null || @object.Equals(null);
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int) s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }

            return tempLen;
        }

        /// <summary>
        /// 保留指定长度字符串,一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <param name="length">目标长度</param>
        /// <returns></returns>
        public static string RetainSpecifyLengthStr(string inputString, int length)
        {
            int currentLength = StrLength(inputString);

            if (currentLength <= length)
            {
                return inputString;
            }

            StringBuilder builder = new StringBuilder();

            int tempLength = 0;
            ASCIIEncoding ascii = new ASCIIEncoding();
            for (var i = 0; i < inputString.Length; i++)
            {
                char @char = inputString[i];
                int add = ascii.GetBytes(@char.ToString())[0] == 63 ? 2 : 1;

                tempLength += add;

                if (tempLength <= length)
                {
                    builder.Append(@char);
                }
                else
                {
                    break;
                }
            }

            return builder.ToString();
        }

        static List<string> emojiPatterns = new List<string> {@"\p{Cs}", @"[\u2702-\u27B0]"};

        public static string RemoveEmoji(string s)
        {
            for (int i = 0; i < emojiPatterns.Count; ++i)
            {
                s = Regex.Replace(s, emojiPatterns[i], "");
            }

            return s;
        }

        public static readonly Vector2 ScreenSize = new Vector2(1080f, 1920f);

        public static bool IsPad()
        {
            return GetMatchWidthOrHeight() > 0;
        }

        public static float GetMatchWidthOrHeight()
        {
            float aspectRatio = GetAspectRatio();
            // 标准尺寸 1080x1920，宽高比0.5625f，大于此比例，UI高适配
            return aspectRatio > 0.5625f ? 1 : 0;
        }

        /// <summary>
        /// 获取Canvas实际屏幕宽度
        /// </summary>
        /// <returns></returns>
        public static float GetPadUIWidth()
        {
            float padWidth = ScreenSize.y / Screen.height * Screen.width;
            return padWidth;
        }

        //返回宽高比
        public static float GetAspectRatio()
        {
            float width = Screen.width;
            float height = Screen.height;
            return width / height;
        }

        // 判断网络是否链接
        public static bool IsInternetConnected()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public static bool SetActiveVirtual(this GameObject gameObject, bool isActive)
        {
            if (gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
                return true;
            }

            return false;
        }
    }
}