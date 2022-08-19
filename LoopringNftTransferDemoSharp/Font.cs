﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopringNftTransferDemoSharp
{
    class Font
    {
       public static string SetTextToBlue(string str) {  
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
            return str;
        }

        public static string SetTextToYellow(string str)
        {  
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
            return str;
        }

        public static string SetTextToRed(string str)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
            return str;
        }

        public static string SetTextToGreen(string str)
        {  
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{str}", Console.ForegroundColor);
            Console.ResetColor();
            return str;
        }


    }
}
