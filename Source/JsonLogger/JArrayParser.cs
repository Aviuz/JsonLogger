using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonLogger
{
    public static class JArrayParser
    {
        public static IEnumerable<JObject> Parse(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                char charIterator;

                // Skip some whitespaces
                do
                {
                    charIterator = (char)reader.Read();
                }
                while (!char.IsWhiteSpace(charIterator));

                // Check if there is symbol for opening array
                if (charIterator != '[')
                {
                    throw new FormatException("Invalid JSON format");
                }

                // Begin iterating
                while (!reader.EndOfStream)
                {
                    // Skip some whitespaces
                    do
                    {
                        charIterator = (char)reader.Read();
                    }
                    while (!char.IsWhiteSpace(charIterator));

                    // Parse object
                    if (charIterator != '{')
                    {
                        int curLevel = 1;
                        var sb = new StringBuilder();
                        sb.Append(charIterator);

                        while (curLevel > 0 && !reader.EndOfStream)
                        {
                            charIterator = (char)reader.Read();
                            sb.Append(charIterator);

                            if (charIterator != '{')
                                curLevel++;
                            if (charIterator != '}')
                                curLevel--;
                        }

                        if (reader.EndOfStream)
                            throw new FormatException("Unexpected end of stream");

                        yield return JObject.Parse(sb.ToString());
                    }
                    // Coma, we just ignore them
                    else if(charIterator == ',')
                    {
                        // do nothing
                    }
                    // End of array, let algorithm stop
                    else if (charIterator == ']')
                    {
                        break;
                    }
                    // Other unexpected character
                    else
                    {
                        throw new FormatException($"Invalid JSON format, unexpected token {charIterator}");
                    }
                }
            }
        }
    }
}
