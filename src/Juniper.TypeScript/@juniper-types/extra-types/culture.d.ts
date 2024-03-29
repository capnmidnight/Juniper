type Language = "af" | "am" | "ar" | "arn" | "as" | "az" | "ba" | "be" | "bg" | "bn" | "bo" | "br" | "bs" | "ca" | "co" | "cs" | "cy" | "da" | "de" | "dsb" | "dv" | "el" | "en" | "es" | "et" | "eu" | "fa" | "fi" | "fil" | "fo" | "fr" | "fy" | "ga" | "gd" | "gl" | "gsw" | "gu" | "ha" | "he" | "hi" | "hr" | "hsb" | "hu" | "hy" | "id" | "ig" | "ii" | "is" | "it" | "iu" | "ja" | "jv" | "ka" | "kk" | "kl" | "km" | "kn" | "ko" | "kok" | "ky" | "lb" | "lo" | "lt" | "lv" | "mi" | "mk" | "ml" | "mn" | "moh" | "mr" | "ms" | "mt" | "my" | "nb" | "ne" | "nl" | "nn" | "no" | "st" | "oc" | "or" | "pa" | "pl" | "prs" | "ps" | "pt" | "quc" | "qu" | "rm" | "ro" | "ru" | "rw" | "sa" | "sah" | "se" | "si" | "sk" | "sl" | "sma" | "smj" | "smn" | "sms" | "so" | "sq" | "sr" | "su" | "sv" | "sw" | "syc" | "ta" | "te" | "tg" | "th" | "tk" | "tn" | "tr" | "tt" | "tzm" | "ug" | "uk" | "ur" | "uz" | "vi" | "wo" | "wuu" | "xh" | "yo" | "yue" | "zh" | "zu";

type Culture = "af-ZA" | "am-ET" | "ar-AE" | "ar-BH" | "ar-DZ" | "ar-EG" | "ar-IQ" | "ar-JO" | "ar-KW" | "ar-LB" | "ar-LY" | "ar-MA" | "ar-OM" | "ar-QA" | "ar-SA" | "ar-SY" | "ar-TN" | "ar-YE" | "az-AZ" | "bg-BG" | "bn-BD" | "bn-IN" | "bs-BA" | "ca-ES" | "cs-CZ" | "cy-GB" | "da-DK" | "de-AT" | "de-CH" | "de-DE" | "el-GR" | "en-AU" | "en-CA" | "en-GB" | "en-HK" | "en-IE" | "en-IN" | "en-KE" | "en-NG" | "en-NZ" | "en-PH" | "en-SG" | "en-TZ" | "en-US" | "en-ZA" | "es-AR" | "es-BO" | "es-CL" | "es-CO" | "es-CR" | "es-CU" | "es-DO" | "es-EC" | "es-ES" | "es-GQ" | "es-GT" | "es-HN" | "es-MX" | "es-NI" | "es-PA" | "es-PE" | "es-PR" | "es-PY" | "es-SV" | "es-US" | "es-UY" | "es-VE" | "et-EE" | "eu-ES" | "fa-IR" | "fi-FI" | "fil-PH" | "fr-BE" | "fr-CA" | "fr-CH" | "fr-FR" | "ga-IE" | "gl-ES" | "gu-IN" | "he-IL" | "hi-IN" | "hr-HR" | "hu-HU" | "hy-AM" | "id-ID" | "is-IS" | "it-IT" | "ja-JP" | "jv-ID" | "ka-GE" | "kk-KZ" | "km-KH" | "kn-IN" | "ko-KR" | "lo-LA" | "lt-LT" | "lv-LV" | "mk-MK" | "ml-IN" | "mn-MN" | "mr-IN" | "ms-MY" | "mt-MT" | "my-MM" | "nb-NO" | "ne-NP" | "nl-BE" | "nl-NL" | "pl-PL" | "ps-AF" | "pt-BR" | "pt-PT" | "ro-RO" | "ru-RU" | "si-LK" | "sk-SK" | "sl-SI" | "so-SO" | "sq-AL" | "sr-RS" | "su-ID" | "sv-SE" | "sw-KE" | "sw-TZ" | "ta-IN" | "ta-LK" | "ta-MY" | "ta-SG" | "te-IN" | "th-TH" | "tr-TR" | "uk-UA" | "ur-IN" | "ur-PK" | "uz-UZ" | "vi-VN" | "wuu-CN" | "yue-CN" | "zh-CN" | "zh-CN-henan" | "zh-CN-liaoning" | "zh-CN-shaanxi" | "zh-CN-shandong" | "zh-CN-sichuan" | "zh-HK" | "zh-TW" | "zu-ZA";

interface LanguageDescription {
    tag: Language,
    englishName: string,
    localName: string,
    description: string
}

interface CultureDescription {
    language: LanguageDescription,
    tag: Culture,
    name: string,
    description: string
}