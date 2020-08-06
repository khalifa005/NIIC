using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Builder;

namespace Application
{
    public static class Env
    {
        public static void GetSetCultureInDifferentWays()
        {
            var test = Thread.CurrentThread.CurrentCulture.Name;
            var test2 = System.Globalization.CultureInfo.CurrentCulture.Name;
            var test3 = Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-EG");
            var test4 = Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-EG");
            var test5 = Thread.CurrentThread.CurrentCulture.Name;

            // var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture;
            //var test=requestCulture.Culture.Name;
            var test6 = System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        public static bool IsArabic(string lang)
            => lang == "ar";

        public static bool IsArabic()
            => Thread.CurrentThread.CurrentCulture.Name == "ar-EG";

        public static bool IsUKEnglish()
            => Thread.CurrentThread.CurrentCulture.Name == "en-GB";

        public static bool IsUSEnglish()
            => Thread.CurrentThread.CurrentCulture.Name == "en-US";

        public static bool IsFrench(string code)
            => code?.Equals("fr", System.StringComparison.InvariantCulture) ?? false;

        public static bool IsFrench()
            => Thread.CurrentThread.CurrentCulture.Name == "fr-FR";

        public static void SetArabic()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-EG");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-EG");
        }

        public static void SetUKEnglish()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
        }

        public static void SetUSEnglish()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        }

        public static string CurrentCultureCode => Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
    }
}
