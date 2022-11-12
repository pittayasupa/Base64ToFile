using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Base64ToFile
{
    internal class Program
    { 
        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None); 
        } 
        public static string Reverse(string text)
        {
            if (text == null) return null; 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }
        static char[] base64tables = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2','3','4','5','6','7','8','9',
            '+','/'
        };
        public static byte[] ConvertToBase64Buffer(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(base64String);
                char[] chars = Encoding.ASCII.GetString(bytes).ToCharArray();

                int len = (chars.Length * 6) / 8;
                byte[] buffer = new byte[len];
                int index = 0;

                string binaryStr = "";
                for (int i = 0; i < chars.Length; i++)
                {
                    int a = -1; 
                    if (chars[i] >= '0' && chars[i] <= '9')
                    {
                        a = (chars[i] - 48) + 52;
                    }
                    else if(chars[i] >= 'A' && chars[i] <= 'Z')
                    {
                        a = chars[i] - 65;
                    }
                    else if (chars[i] >= 'a' && chars[i] <= 'z')
                    {
                        a = (chars[i] - 97) + 26;
                    } 
                    else if (chars[i] == '+')
                    {
                        a = 62;
                    }
                    else if (chars[i] == '/')
                    {
                        a = 63;
                    }
                    if (a >= 0)
                    {
                        string s = Convert.ToString(a, 2).PadLeft(6, '0');
                        binaryStr += s;
                         
                        if (binaryStr.Length >= 8)
                        {
                            string sub = binaryStr.Substring(0, 8);
                            binaryStr = binaryStr.Substring(8);  
                            buffer[index] = Convert.ToByte(sub, 2); ;
                            index++;
                        } 
                    } 
                    if(i % 10000 == 0)
                    {
                        Console.WriteLine(i); 
                    }
                } 
                return buffer;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Convert to base64 buffer error: " + ex.Message);
                return null;
            } 
        }

        public static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if(a.Length > b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        } 
        public static string GetFileTypeFromBuffer(byte[] buffers)
        {
            if (buffers == null) return ""; 
            if (ByteArrayCompare(Encoding.ASCII.GetBytes(@"%PDF"), buffers)) return ".pdf"; 
            if (ByteArrayCompare(Encoding.ASCII.GetBytes(@"‰PNG"), buffers)) return ".png"; 
            return "";
        }

        static void Main(string[] args)
        {
            //string text = "JVBERi0xLjcKJfbk";
            //byte[] bb = ConvertToBase64Buffer(text);

            //Console.WriteLine("DDD: " + Encoding.ASCII.GetString(bb));

            //return;

           
            byte[] buffer = new byte[0xffffff];
            using(FileStream fs = File.OpenRead("PDF1_Sign.txt"))
            {
                int length = fs.Read(buffer, 0, buffer.Length);
                //char[] myChars = Encoding.ASCII.GetString(buffer, 0, length).ToCharArray();
                string resultString = Encoding.ASCII.GetString(buffer, 0, length);
                byte[] result = ConvertToBase64Buffer(resultString);
                File.WriteAllBytes("dddd8.pdf", result);
                //File.WriteAllBytes("dddd77.txt", result);
            }
        }
    }
}
