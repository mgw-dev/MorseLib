using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Text;

namespace MorseLib
{
    public static class Morse
    {
        private const char wordSpacer = '|';
        private const char letterSpacer = ' ';
        private const char dot = '.';
        private const char dash = '-';

        public static readonly Dictionary<char, string> latinToMorse = new()
        {
            { '0' , "-----"  },
            { '1' , ".----"  },
            { '2' , "..---"  },
            { '3' , "...--"  },
            { '4' , "....-"  },
            { '5' , "....."  },
            { '6' , "-...."  },
            { '7' , "--..."  },
            { '8' , "---.."  },
            { '9' , "----."  },
            { 'A' , ".-"     },
            { 'B' , "-..."   },
            { 'C' , "-.-."   },
            { 'D' , "-.."    },
            { 'E' , "."      },
            { 'F' , "..-."   },
            { 'G' , "--."    },
            { 'H' , "...."   },
            { 'I' , ".."     },
            { 'J' , ".---"   },
            { 'K' , "-.-"    },
            { 'L' , ".-.."   },
            { 'M' , "--"     },
            { 'N' , "-."     },
            { 'O' , "---"    },
            { 'P' , ".--."   },
            { 'Q' , "--.-"   },
            { 'R' , ".-."    },
            { 'S' , "..."    },
            { 'T' , "-"      },
            { 'U' , "..-"    },
            { 'V' , "...-"   },
            { 'W' , ".--"    },
            { 'X' , "-..-"   },
            { 'Y' , "-.--"   },
            { 'Z' , "--.."   },
            { '.' , ".-.-.-" },
            { ',' , "--..--" },
            { '?' , "..--.." },
            { '\'', ".----." },
            { '!' , "-.-.--" },
            { '/' , "-..-."  },
            { '(' , "-.--."  },
            { ')' , "-.--.-" },
            { '&' , ".-..."  },
            { ':' , "---..." },
            { ';' , "-.-.-." },
            { '=' , "-...-"  },
            { '+' , ".-.-."  },
            { '-' , "-....-" },
            { '_' , "..--.-" },
            { '"' , ".-..-." },
            { '$' , "...-..-"},
            { '@' , ".--.-." },
            //{ '' , "" },
        };

        //public static readonly Dictionary<string, char> morseToLatin = new()
        //{
        //    { "-----", '0' },
        //    { ".----", '1' },
        //    { "..---", '2' },
        //    { "...--", '3' },
        //    { "....-", '4' },
        //    { ".....", '5' },
        //    { "-....", '6' },
        //    { "--...", '7' },
        //    { "---..", '8' },
        //    { "----.", '9' },
        //    { ".-"   , 'A' },
        //    { "-..." , 'B' },
        //    { "-.-." , 'C' },
        //    { "-.."  , 'D' },
        //    { "."    , 'E' },
        //    { "..-." , 'F' },
        //    { "--."  , 'G' },
        //    { "...." , 'H' },
        //    { ".."   , 'I' },
        //    { ".---" , 'J' },
        //    { "-.-"  , 'K' },
        //    { ".-.." , 'L' },
        //    { "--"   , 'M' },
        //    { "-."   , 'N' },
        //    { "---"  , 'O' },
        //    { ".--." , 'P' },
        //    { "--.-" , 'Q' },
        //    { ".-."  , 'R' },
        //    { "..."  , 'S' },
        //    { "-"    , 'T' },
        //    { "..-"  , 'U' },
        //    { "...-" , 'V' },
        //    { ".--"  , 'W' },
        //    { "-..-" , 'X' },
        //    { "-.--" , 'Y' },
        //    { "--.." , 'Z' }
        //};


        /// <summary>
        /// Converts a string to Morse Code.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMorse(this string str)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str.ToUpperInvariant())
            {
                if (c == ' ')
                {
                    sb.Append(wordSpacer);
                }
                else
                {
                    if (latinToMorse.TryGetValue(c, out var s))
                    {
                        sb.Append(s);
                        sb.Append(letterSpacer);
                    }
                    else
                    {
                        // Simply ignore invalid characters.
                    }
                }
                
            }

            return sb.ToString();
        }

        //public static string ToLatin(this string input)
        //{
        //    string[] code = input.Split(' ');
        //    StringBuilder sb = new StringBuilder();

        //    foreach (string s in code)
        //    {
        //        if (s == "/")
        //        {
        //            sb.Append(' ');
        //        }
        //        else
        //        {
        //            if (morseToLatin.TryGetValue(s, out var c))
        //            {
        //                sb.Append(c);
        //            }
        //            else
        //            {
        //                // Simply ignore invalid characters.
        //            }
        //        }
        //    }

        //    return sb.ToString();
        //}

        /// <summary>
        /// Takes a morse code string input, and plays it as audio
        /// </summary>
        /// <param name="input">Must be a properly formatted morse code string.</param>
        /// <param name="dotDuration">Number of milliseconds for a dot. Dash and space durations are calculated from this value.</param>
        /// <param name="gain">Gain.</param>
        /// <param name="frequency">Frequency.</param>
        public static void PlayMorse(string input, int dotDuration = 70, double gain = 1, double frequency = 450)
        {
            SignalGeneratorType type = SignalGeneratorType.Sin;
            List<ISampleProvider> samples = new List<ISampleProvider>();
            foreach (char c in input)
            {
                ISampleProvider tone = new SignalGenerator() { Gain = gain, Frequency = frequency, Type = type };
                ISampleProvider silence = new SilenceProvider(tone.WaveFormat).ToSampleProvider();

                switch (c)
                {
                    case dot:
                        samples.Add(tone.Take(TimeSpan.FromMilliseconds(dotDuration)));
                        samples.Add(silence.Take(TimeSpan.FromMilliseconds(dotDuration)));
                        break;
                    case dash:
                        samples.Add(tone.Take(TimeSpan.FromMilliseconds(dotDuration * 3)));
                        samples.Add(silence.Take(TimeSpan.FromMilliseconds(dotDuration)));
                        break;
                    case letterSpacer:
                        samples.Add(silence.Take(TimeSpan.FromMilliseconds(dotDuration * 3)));
                        break;
                    case wordSpacer:
                        samples.Add(silence.Take(TimeSpan.FromMilliseconds(dotDuration * 7)));
                        break;

                }
            }

            ISampleProvider morseCode = new ConcatenatingSampleProvider(samples);
            using (var wo = new WaveOutEvent())
            {
                wo.Init(morseCode);
                wo.Play();
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1);
                }
            }

        }
    }
}