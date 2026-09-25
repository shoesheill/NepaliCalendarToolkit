export const BS_MONTH_NAMES_NP = [
  "वैशाख", "जेठ", "असार", "श्रावण", "भाद्र", "आश्विन",
  "कार्तिक", "मंसिर", "पुस", "माघ", "फाल्गुण", "चैत्र",
] as const;

export const TITHI_NAMES_NP: Readonly<Record<number, string>> = {
  1: "प्रतिपदा", 2: "द्वितीया", 3: "तृतीया", 4: "चतुर्थी",
  5: "पञ्चमी", 6: "षष्ठी", 7: "सप्तमी", 8: "अष्टमी",
  9: "नवमी", 10: "दशमी", 11: "एकादशी", 12: "द्वादशी",
  13: "त्रयोदशी", 14: "चतुर्दशी", 15: "पूर्णिमा", 30: "औंसी",
};

export const TITHI_NAMES_EN: Readonly<Record<number, string>> = {
  1: "Pratipada", 2: "Dwitiya", 3: "Tritiya", 4: "Chaturthi",
  5: "Panchami", 6: "Sasthi", 7: "Saptami", 8: "Astami",
  9: "Nawami", 10: "Dashami", 11: "Ekadashi", 12: "Dwadashi",
  13: "Trayodashi", 14: "Chaturdashi", 15: "Purnima", 30: "Aunsi",
};

export const CHANDRAMA_NAMES_NP: Readonly<Record<number, string>> = {
  12: "वैशाख कृष्ण", 13: "वैशाख शुक्ल", 14: "ज्येष्ठ कृष्ण", 15: "ज्येष्ठ शुक्ल",
  16: "आषाढ कृष्ण", 17: "आषाढ शुक्ल", 18: "श्रावण कृष्ण", 19: "श्रावण शुक्ल",
  20: "भाद्र कृष्ण", 21: "भाद्र शुक्ल", 22: "आश्विन कृष्ण", 23: "आश्विन शुक्ल",
  24: "कार्तिक कृष्ण", 1: "कार्तिक शुक्ल", 2: "मार्ग कृष्ण", 3: "मार्ग शुक्ल",
  4: "पुस कृष्ण", 5: "पुस शुक्ल", 6: "माघ कृष्ण", 7: "माघ शुक्ल",
  8: "फाल्गुण कृष्ण", 9: "फाल्गुण शुक्ल", 10: "चैत्र कृष्ण", 11: "चैत्र शुक्ल",
};

export const LUNAR_MONTH_NAMES_NP: Readonly<Record<number, string>> = {
  12: "वैशाख", 13: "वैशाख", 14: "ज्येष्ठ", 15: "ज्येष्ठ",
  16: "आषाढ", 17: "आषाढ", 18: "श्रावण", 19: "श्रावण",
  20: "भाद्र", 21: "भाद्र", 22: "आश्विन", 23: "आश्विन",
  24: "कार्तिक", 1: "कार्तिक", 2: "मार्ग", 3: "मार्ग",
  4: "पौष", 5: "पौष", 6: "माघ", 7: "माघ",
  8: "फाल्गुण", 9: "फाल्गुण", 10: "चैत्र", 11: "चैत्र",
};

export const LUNAR_MONTH_NAMES_EN: Readonly<Record<number, string>> = {
  12: "Baishakh", 13: "Baishakh", 14: "Jestha", 15: "Jestha",
  16: "Ashadh", 17: "Ashadh", 18: "Shrawan", 19: "Shrawan",
  20: "Bhadra", 21: "Bhadra", 22: "Ashwin", 23: "Ashwin",
  24: "Kartik", 1: "Kartik", 2: "Mangsir", 3: "Mangsir",
  4: "Poush", 5: "Poush", 6: "Magh", 7: "Magh",
  8: "Falgun", 9: "Falgun", 10: "Chaitra", 11: "Chaitra",
};

export const PAKSHA_NAMES_NP: Readonly<Record<"shukla" | "krishna", string>> = {
  shukla: "शुक्ल",
  krishna: "कृष्ण",
};

export const PAKSHA_NAMES_EN: Readonly<Record<"shukla" | "krishna", string>> = {
  shukla: "Shukla",
  krishna: "Krishna",
};

export function pakshaKey(chandrama?: number): "shukla" | "krishna" | undefined {
  if (chandrama == null || chandrama < 1 || chandrama > 24) return undefined;
  // The API's chandrama code names the fortnight directly: odd = Shukla, even = Krishna
  // (see CHANDRAMA_NAMES_*). Deriving it from a day-of-month formula got 13..24 wrong.
  return chandrama % 2 === 1 ? "shukla" : "krishna";
}

export const CHANDRAMA_NAMES_EN: Readonly<Record<number, string>> = {
  12: "Baishakh Krishna", 13: "Baishakh Shukla", 14: "Jestha Krishna", 15: "Jestha Shukla",
  16: "Ashadh Krishna", 17: "Ashadh Shukla", 18: "Shrawan Krishna", 19: "Shrawan Shukla",
  20: "Bhadra Krishna", 21: "Bhadra Shukla", 22: "Aswin Krishna", 23: "Aswin Shukla",
  24: "Kartik Krishna", 1: "Kartik Shukla", 2: "Marga Krishna", 3: "Marga Shukla",
  4: "Poush Krishna", 5: "Poush Shukla", 6: "Magh Krishna", 7: "Magh Shukla",
  8: "Falgun Krishna", 9: "Falgun Shukla", 10: "Chaitra Krishna", 11: "Chaitra Shukla",
};

export const NS_MONTH_NAMES_NP: Readonly<Record<number, string>> = {
  1: "कछलाथ्व", 2: "कछलागा", 3: "थिंलाथ्व", 4: "थिंलागा",
  5: "पोहेलाथ्व", 6: "पोहेलागा", 7: "सिल्लाथ्व", 8: "सिल्लागा",
  9: "चिल्लाथ्व", 10: "चिल्लागा", 11: "चौलाथ्व", 12: "चौलागा",
  13: "बछलाथ्व", 14: "बछलागा", 15: "तछलाथ्व", 16: "तछलागा",
  17: "दिल्लाथ्व", 18: "दिल्लागा", 19: "गुँलाथ्व", 20: "गुँलागा",
  21: "यंलाथ्व", 22: "यंलागा", 23: "कौलाथ्व", 24: "कौलागा",
  25: "अनलाथ्व", 26: "अनलागा",
};

export function nsMonthNumber(code?: string): number | undefined {
  if (!code) return undefined;
  const value = Number(code);
  return Number.isFinite(value) ? Math.trunc(value) : undefined;
}