using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public static class Config
    {
        public const int CommonPageSize = 50;
        public const int NoneOptionValue = -2;
        public const int AllOptionValue = -1;
        public const string ApiSessionHeader = "X-LMAP-UserSessionId";
        public const string ApiSessionObject = "LMAP-API-Session-object";
        public const int TrueValue = 1;
        public const int FalseValue = 0;
        public const string DefaultCountry = "EGY";
    }

    public static class StringLength
    {
        public const int One = 1;
        public const int Two = 2;
        public const int Three = 3;
        public const int Five = 5;
        public const int Twelve = 12;
        public const int Thirteen = 13;
        public const int Twenty = 20;
        public const int Fifty = 50;
        public const int Fifteen = 15;
        public const int Hundred = 100;
        public const int TwoHundredFifty = 250;
        public const int FiveHundred = 500;
        public const int FiveHundredFifty = 550;
        public const int Thousand = 1000;
    }

    public class Global
    {
    }
}
