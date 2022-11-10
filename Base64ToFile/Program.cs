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
                            //Console.WriteLine("sub: " + sub);
                            //Console.WriteLine("binaryStr: " + binaryStr);


                            byte rByte = Convert.ToByte(sub, 2);
                            //Console.WriteLine("rByte: " + Convert.ToChar(rByte));

                            buffer[index] = rByte;
                            index++;
                        }
                        
                    }
                    /*for (int j = 0; j < base64tables.Length; j++)
                    {
                        if (base64tables[j] == chars[i])
                        {
                            string s = Convert.ToString(j, 2).PadLeft(6, '0');
                            binaryStr += s;
                        }
                    }*/
                    if(i % 10000 == 0)
                    {
                        Console.WriteLine(i);
                        //binaryStr = "";
                    }
                }
                /*int length = binaryStr.Length;
                int cnt = length / 8; 
                int index = 0;
                byte[] buffer = new byte[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    string sub = binaryStr.Substring((i * 8), 8);
                    buffer[index] = Convert.ToByte(sub, 2);
                    index++;
                }*/
                return buffer;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Convert to base64 buffer error: " + ex.Message);
                return null;
            } 
        }


        static void Main(string[] args)
        {
            //string text = "JVBERi0xLjcKJfbk";
            //byte[] bb = ConvertToBase64Buffer(text);

            //Console.WriteLine("DDD: " + Encoding.ASCII.GetString(bb));

            //return;

            /*string text = "JVBERi0xLjcKJfbk/N8KMS";
            //string text = "aaa";
            byte[] b1 = Encoding.ASCII.GetBytes(text);
            char[] c1 = Encoding.ASCII.GetString(b1).ToCharArray();

            string textBinary = "";
            for(int i = 0; i < c1.Length; i++)
            {
                for(int j = 0; j < base64tables.Length; j++)
                {
                    if(base64tables[j] == c1[i])
                    {
                        string s = Convert.ToString(j, 2).PadLeft(6, '0');
                        //string s2 = Reverse(s);
                        //Console.WriteLine("set: " + s); 
                        textBinary += s; 
                    }
                }
            }

            Console.WriteLine("textBinary: " + textBinary);

            //textBinary = Reverse(textBinary);

            int length = textBinary.Length;
            int d1 = length / 8;

            Console.WriteLine("length: " + length);
            Console.WriteLine("d1: " + d1);

            int index = 0;
            byte[] buffer = new byte[d1];
            for(int i = 0; i < d1; i++)
            {
                string sub = textBinary.Substring((i * 8), 8);
                //string rev = Reverse(sub);

                Console.WriteLine("sub: " + sub);
                //Console.WriteLine("rev: " + rev);

                byte e1 = Convert.ToByte(sub, 2);
                //byte e2 = Convert.ToByte(rev, 2);

                Console.WriteLine("sub: " + e1.ToString("X2"));
                //Console.WriteLine("rev: " + e2.ToString("X2"));

                buffer[index] = Convert.ToByte(sub, 2);

                Console.WriteLine(buffer[index].ToString("X2"));


                index++;
                  
            }

            Console.WriteLine("Str: " + Encoding.ASCII.GetString(buffer));

            //string s = Convert.ToString(2, 2).PadLeft(6, '0');
            //char[] charArray = s.ToCharArray();
            //Array.Reverse(charArray);
            //string s2 = Reverse(s);

            //Console.WriteLine($" {s2} ");
            */

            byte[] buffer = new byte[0xffffff];
            using(FileStream fs = File.OpenRead("PDF1_Sign.txt"))
            {
                int length = fs.Read(buffer, 0, buffer.Length);
                /*char[] myChars = Encoding.ASCII.GetString(buffer, 0, length).ToCharArray();

                string ss = "";
                for(int i = myChars.Length - 30; i < myChars.Length; i++)
                {
                    ss += myChars[i];
                }
                Console.WriteLine(ss);
                
                for(int i = 0; i < myChars.Length; i++)
                {
                    if(myChars[i] == '.')
                    {
                        Console.WriteLine("valid: " + myChars[i]);
                    }
                }*/

                string resultString = Encoding.ASCII.GetString(buffer, 0, length);
                byte[] result = ConvertToBase64Buffer(resultString);
                File.WriteAllBytes("dddd7.pdf", result);
                //File.WriteAllBytes("dddd77.txt", result);

                //string message = myChars.ToString();

                //Console.WriteLine($"{IsBase64String(message)}");
                //Console.WriteLine($"{((length * 6) % 8)}");

                //byte[] buff = Convert.FromBase64CharArray(myChars, 0, myChars.Length);
                //File.WriteAllBytes("dddd6.pdf", buff);


            }
        }
    }
}
