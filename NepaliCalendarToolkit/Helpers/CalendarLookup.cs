using System;
using System.Collections.Generic;
using System.Globalization;

namespace NepaliCalendarToolkit.Helpers
{
    internal static class CalendarLookup
    {
        private static readonly string[] BsMonthNames =
        {
            "वैशाख", "जेठ", "असार", "श्रावण", "भाद्र", "आश्विन",
            "कार्तिक", "मंसिर", "पुस", "माघ", "फाल्गुण", "चैत्र"
        };

        private static readonly string[] BsMonthNamesEn =
        {
            "Baisakh", "Jestha", "Ashadh", "Shrawan", "Bhadra", "Ashwin",
            "Kartik", "Mangsir", "Poush", "Magh", "Falgun", "Chaitra"
        };

        private static readonly Dictionary<int, string> TithiNamesNp = new Dictionary<int, string>
        {
            [1] = "प्रतिपदा", [2] = "द्वितीया", [3] = "तृतीया", [4] = "चतुर्थी",
            [5] = "पञ्चमी", [6] = "षष्ठी", [7] = "सप्तमी", [8] = "अष्टमी",
            [9] = "नवमी", [10] = "दशमी", [11] = "एकादशी", [12] = "द्वादशी",
            [13] = "त्रयोदशी", [14] = "चतुर्दशी", [15] = "पूर्णिमा", [30] = "औंसी"
        };

        private static readonly Dictionary<int, string> TithiNamesEn = new Dictionary<int, string>
        {
            [1] = "Pratipada", [2] = "Dwitiya", [3] = "Tritiya", [4] = "Chaturthi",
            [5] = "Panchami", [6] = "Sasthi", [7] = "Saptami", [8] = "Astami",
            [9] = "Nawami", [10] = "Dashami", [11] = "Ekadashi", [12] = "Dwadashi",
            [13] = "Trayodashi", [14] = "Chaturdashi", [15] = "Purnima", [30] = "Aunsi"
        };

        private static readonly Dictionary<int, string> LunarMonthNamesNp = new Dictionary<int, string>
        {
            [12] = "वैशाख", [13] = "वैशाख", [14] = "ज्येष्ठ", [15] = "ज्येष्ठ",
            [16] = "आषाढ", [17] = "आषाढ", [18] = "श्रावण", [19] = "श्रावण",
            [20] = "भाद्र", [21] = "भाद्र", [22] = "आश्विन", [23] = "आश्विन",
            [24] = "कार्तिक", [1] = "कार्तिक", [2] = "मार्ग", [3] = "मार्ग",
            [4] = "पौष", [5] = "पौष", [6] = "माघ", [7] = "माघ",
            [8] = "फाल्गुण", [9] = "फाल्गुण", [10] = "चैत्र", [11] = "चैत्र"
        };

        private static readonly Dictionary<int, string> LunarMonthNamesEn = new Dictionary<int, string>
        {
            [12] = "Baishakh", [13] = "Baishakh", [14] = "Jestha", [15] = "Jestha",
            [16] = "Ashadh", [17] = "Ashadh", [18] = "Shrawan", [19] = "Shrawan",
            [20] = "Bhadra", [21] = "Bhadra", [22] = "Ashwin", [23] = "Ashwin",
            [24] = "Kartik", [1] = "Kartik", [2] = "Mangsir", [3] = "Mangsir",
            [4] = "Poush", [5] = "Poush", [6] = "Magh", [7] = "Magh",
            [8] = "Falgun", [9] = "Falgun", [10] = "Chaitra", [11] = "Chaitra"
        };

        private static readonly Dictionary<int, string> PakshaNamesNp = new Dictionary<int, string>
        {
            [1] = "शुक्ल", [2] = "शुक्ल", [3] = "कृष्ण", [4] = "कृष्ण"
        };

        private static readonly Dictionary<int, string> PakshaNamesEn = new Dictionary<int, string>
        {
            [1] = "Shukla", [2] = "Shukla", [3] = "Krishna", [4] = "Krishna"
        };

        private static readonly Dictionary<int, string> NepaliSambatMonthNames = new Dictionary<int, string>
        {
            [1] = "कछलाथ्व", [2] = "कछलागा", [3] = "थिंलाथ्व", [4] = "थिंलागा",
            [5] = "पोहेलाथ्व", [6] = "पोहेलागा", [7] = "सिल्लाथ्व", [8] = "सिल्लागा",
            [9] = "चिल्लाथ्व", [10] = "चिल्लागा", [11] = "चौलाथ्व", [12] = "चौलागा",
            [13] = "बछलाथ्व", [14] = "बछलागा", [15] = "तछलाथ्व", [16] = "तछलागा",
            [17] = "दिल्लाथ्व", [18] = "दिल्लागा", [19] = "गुँलाथ्व", [20] = "गुँलागा",
            [21] = "यंलाथ्व", [22] = "यंलागा", [23] = "कौलाथ्व", [24] = "कौलागा",
            [25] = "अनलाथ्व", [26] = "अनलागा"
        };

        public static string GetBsMonthName(int month)
        {
            return month >= 1 && month <= BsMonthNames.Length ? BsMonthNames[month - 1] : null;
        }

        public static string GetBsMonthNameEn(int month)
        {
            return month >= 1 && month <= BsMonthNamesEn.Length ? BsMonthNamesEn[month - 1] : null;
        }

        public static string GetTithiName(int? tithi)
        {
            return tithi.HasValue && TithiNamesNp.TryGetValue(tithi.Value, out var name) ? name : null;
        }

        public static string GetTithiNameEn(int? tithi)
        {
            return tithi.HasValue && TithiNamesEn.TryGetValue(tithi.Value, out var name) ? name : null;
        }

        public static string GetLunarMonthName(int? chandrama)
        {
            return chandrama.HasValue && LunarMonthNamesNp.TryGetValue(chandrama.Value, out var name) ? name : null;
        }

        public static string GetLunarMonthNameEn(int? chandrama)
        {
            return chandrama.HasValue && LunarMonthNamesEn.TryGetValue(chandrama.Value, out var name) ? name : null;
        }

        public static string GetPaksha(int? chandrama)
        {
            if (!chandrama.HasValue) return null;
            if (chandrama.Value < 1 || chandrama.Value > 24) return null;
            return PakshaNamesNp.TryGetValue(chandrama.Value % 2 == 1 ? 1 : 3, out var name) ? name : null;
        }

        public static string GetPakshaEn(int? chandrama)
        {
            if (!chandrama.HasValue) return null;
            if (chandrama.Value < 1 || chandrama.Value > 24) return null;
            return PakshaNamesEn.TryGetValue(chandrama.Value % 2 == 1 ? 1 : 3, out var name) ? name : null;
        }

        public static string GetNepaliSambatMonthName(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            if (!decimal.TryParse(code, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)) return null;
            var month = (int)value;
            return NepaliSambatMonthNames.TryGetValue(month, out var name) ? name : null;
        }
    }
}