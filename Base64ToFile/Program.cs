using System;
using System.Globalization;
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
        public static byte[] ConvertToBase64Buffer(string base64String, Encoding encoding = null, bool debug = false)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                Console.WriteLine("convert to base 64 error: base64String is null");
                return null;
            }
            try
            {
                if(encoding == null) encoding = Encoding.ASCII;
                byte[] bytes = encoding.GetBytes(base64String);
                char[] chars = encoding.GetString(bytes).ToCharArray();

                int length = (chars.Length * 6) / 8;
                byte[] buffer = new byte[length];
                int index = 0;

                string binaryStr = "";
                string readStr = "";
                for (int i = 0; i < chars.Length; i++)
                {
                    int base2char = -1;  
                    if(chars[i] >= 'A' && chars[i] <= 'Z') {
                        base2char = chars[i] - 'A';
                    }
                    else if (chars[i] >= 'a' && chars[i] <= 'z') {
                        base2char = (chars[i] - 'a') + 26;
                    }
                    else if (chars[i] >= '0' && chars[i] <= '9')
                    {
                        base2char = (chars[i] - '0') + 52;
                    }
                    else if (chars[i] == '+') {
                        base2char = 62; 
                    }
                    else if (chars[i] == '/') {
                        base2char = 63;
                    }
                    else if (chars[i] == '=') { // padding
                        base2char = 0;
                    }
                    if (base2char >= 0) {
                        readStr = Convert.ToString(base2char, 2).PadLeft(6, '0');
                        binaryStr += readStr; 
                        if (binaryStr.Length >= 8) {
                            string sub = binaryStr.Substring(0, 8);
                            binaryStr = binaryStr.Substring(8);  
                            buffer[index] = Convert.ToByte(sub, 2); 
                            index++;
                        } 
                    }
                    if (debug)
                    {
                        if (i % 100000 == 0 || i == chars.Length - 1)
                        {
                            double per = (i * 100.0) / chars.Length;
                            Console.WriteLine(Math.Round(per, 2) + "%");
                        }
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
            if(a == null || b == null) return false;
            if(a.Length > b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        } 
        public static string GetFileTypeFromBuffer(byte[] buffers)
        {
            if (buffers == null) return "txt";  
            if (ByteArrayCompare(new byte[] { 0x25, 0x50, 0x44, 0x46 }, buffers)) return "pdf";
            if (ByteArrayCompare(new byte[] { 0x0D, 0x44, 0x4F, 0x43 }, buffers)) return "doc"; 
            if (ByteArrayCompare(new byte[] { 0xA0, 0x46, 0x1D, 0xF0 }, buffers)) return "ppt";
            if (ByteArrayCompare(new byte[] { 0xEB, 0x3C, 0x90, 0x2A }, buffers)) return "img";
            if (ByteArrayCompare(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, buffers)) return "jpeg";
            if (ByteArrayCompare(new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, buffers)) return "jpg";
            if (ByteArrayCompare(new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }, buffers)) return "jpg"; 
            if (ByteArrayCompare(new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }, buffers)) return "7z";
            if (ByteArrayCompare(new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 }, buffers)) return "zip";
            if (ByteArrayCompare(new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07 }, buffers)) return "rar"; 
            if (ByteArrayCompare(new byte[] { 0x25, 0x62, 0x69, 0x74, 0x6D, 0x61, 0x70 }, buffers)) return "bmp";  
            if (ByteArrayCompare(new byte[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 }, buffers)) return "mp4";  
            if (ByteArrayCompare(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, buffers)) return "png";
            if (ByteArrayCompare(new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, buffers)) return "xls";
            if (ByteArrayCompare(new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }, buffers)) return "xlsx"; 
            if (ByteArrayCompare(new byte[] { 0x50, 0x4B }, buffers)) return "zip"; 
            return "txt";
        }

        static void Main(string[] args)
        {
            string sss = "Zm9vYg==";
            byte[] bytes = ConvertToBase64Buffer(sss, Encoding.ASCII);
            string res = Encoding.ASCII.GetString(bytes);
            Console.WriteLine(res); 
            //return; 


            byte[] buffer = new byte[0xffffff]; //16777215
            using (FileStream fs = File.OpenRead(@"Resources\base64String.txt"))
            {
                int length = fs.Read(buffer, 0, buffer.Length); 
                string resultString = Encoding.ASCII.GetString(buffer, 0, length); 
                byte[] result = ConvertToBase64Buffer(resultString, Encoding.ASCII, true);

                string fileName = "create_file";
                fileName += "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss", new CultureInfo("en-us"));
                fileName += "." + GetFileTypeFromBuffer(result);

                string currentDirectory = Environment.CurrentDirectory;
                string filePath = "ExportFiles";  
                string fileFullPath = Path.Combine(currentDirectory, filePath, fileName);

                if (!Directory.Exists(Path.GetDirectoryName(fileFullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath));
                } 
                File.WriteAllBytes(fileFullPath, result); 
            }
        }
    }
}
