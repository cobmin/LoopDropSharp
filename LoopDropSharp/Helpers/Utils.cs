﻿using LoopDropSharp;
using Nethereum.Signer.EIP712;
using PoseidonSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public static class Utils
    {
        public static BigInteger ParseHexUnsigned(string toParse)
        {
            toParse = toParse.Replace("0x", "");
            var parsResult = BigInteger.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
            if (parsResult < 0)
                parsResult = BigInteger.Parse("0" + toParse, System.Globalization.NumberStyles.HexNumber);
            return parsResult;
        }

        public static string CheckYesOrNo(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "yes") && (userResponse != "no"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes or no.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckOneOrMany(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while ((userResponse != "one") && (userResponse != "many"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer one or many.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }

        public static string CheckYes(string userResponse)
        {
            userResponse = Console.ReadLine().ToLower();
            while (userResponse != "yes")
            {
                LoopDropSharp.Font.SetTextToYellow("Please answer yes when you are ready.");
                userResponse = Console.ReadLine().ToLower();
            }
            return userResponse;
        }
        public static string CheckUtilityNumber(string userResponse)
        {
            userResponse = Console.ReadLine();
            while ((userResponse != "1") && (userResponse != "2") && (userResponse != "3") && (userResponse != "4") && (userResponse != "5") && (userResponse != "6") && (userResponse != "7"))
            {
                LoopDropSharp.Font.SetTextToYellow("Please type a number between 1 and 7.");
                userResponse = Console.ReadLine();
            }
            return userResponse;
        }
        public static int Check1Or2(int userResponse)
        {
            bool validOrNot = false;
            var counter = 0;
            do
            {
                if (counter == 0)
                {
                    validOrNot = int.TryParse(Console.ReadLine()?.ToLower().Trim(), out userResponse);
                    counter++;
                }
                else
                {
                    LoopDropSharp.Font.SetTextToYellow("Please answer 1 or 2.");
                    validOrNot = int.TryParse(Console.ReadLine()?.ToLower().Trim(), out userResponse);
                }
            } while (((userResponse != 1) && (userResponse != 2)) || validOrNot == false);

            return userResponse;
        }

        public static int GetUnixTimestamp() => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;



    }
}
