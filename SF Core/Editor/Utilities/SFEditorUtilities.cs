using System.IO;

using UnityEngine;

namespace SFEditor
{
    public static class SFEditorUtilities
    {
        public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars)
        {
            if(string.IsNullOrEmpty(filename))
            {
                return filename;
            }

            filename = filename.Trim();
            if(string.IsNullOrEmpty(filename))
            {
                return filename;
            }

            string text = new string(Path.GetInvalidFileNameChars());
            string text2 = "";
            bool flag = false;
            string text3 = filename;
            for(int i = 0; i < text3.Length; i++)
            {
                char value = text3[i];
                if(text.IndexOf(value) == -1)
                {
                    text2 += value;
                }
                else
                {
                    flag = true;
                }
            }

            if(flag && logIfInvalidChars)
            {
                string displayStringOfInvalidCharsOfFileName = GetDisplayStringOfInvalidCharsOfFileName(filename);
                if(displayStringOfInvalidCharsOfFileName.Length > 0)
                {
                    Debug.LogWarningFormat("A filename cannot contain the following character{0}:  {1}", (displayStringOfInvalidCharsOfFileName.Length > 1) ? "s" : "", displayStringOfInvalidCharsOfFileName);
                }
            }

            return text2;
        }

        public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
        {
            if(string.IsNullOrEmpty(filename))
            {
                return "";
            }

            string text = new string(Path.GetInvalidFileNameChars());
            string text2 = "";
            for(int i = 0; i < filename.Length; i++)
            {
                char value = filename[i];
                if(text.IndexOf(value) >= 0 && text2.IndexOf(value) == -1)
                {
                    if(text2.Length > 0)
                    {
                        text2 += " ";
                    }

                    text2 += value;
                }
            }

            return text2;
        }
    }
}
