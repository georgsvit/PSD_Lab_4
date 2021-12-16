namespace PSD_Lab_4.Client
{
    public static class EncoderDecoder
    {
        public static char ConvertChar(char ch, int key)
        {
            if (!char.IsLetter(ch))
            {
                return ch;
            }

            char delta = char.IsUpper(ch) ? 'A' : 'a';

            return (char)(((ch + key - delta) % 26) + delta);
        }

        public static string Encode(string input, int key)
        {
            string output = string.Empty;

            foreach (char ch in input)
            {
                output += ConvertChar(ch, key);
            }

            return output;
        }

        public static string Decode(string input, int key)
        {
            return Encode(input, 26 - key);
        }
    }
}
