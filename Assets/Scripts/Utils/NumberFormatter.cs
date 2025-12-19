using System.Globalization;

public static class NumberFormatter
{
    public static string FormatNumber(float number)
    {
        return number.ToString("0.##00", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    public static string FormatNumber(float number, int minDigits = 1, int maxDigits = 1)
    {
        string template = "0.";
        for (int i = 0; i < maxDigits; i++)
        {
            template += (i < minDigits) ? "#" : "0";
        }

        string res = number.ToString(template, CultureInfo.InvariantCulture);

        if (res.Contains("."))
        {
            res = res.TrimEnd('0').TrimEnd('.');
        }

        return res;
    }

    public static string FormatNumberWithGrouping(float value)
    {
        string formatted = FormatNumber(value);

        bool isNegative = formatted.StartsWith("-");
        if (isNegative) formatted = formatted.Substring(1);  // Убираем минус временно

        string integerPart, fractionalPart = null;
        if (formatted.Contains("."))
        {
            var parts = formatted.Split('.');
            integerPart = parts[0];
            fractionalPart = parts[1];
        }
        else
        {
            integerPart = formatted;
        }

        var nfi = new NumberFormatInfo()
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ","
        };

        if (!long.TryParse(integerPart, out long integerNumber))
        {
            return formatted.Replace('.', ',');
        }

        string groupedInteger = integerNumber.ToString("N0", nfi);

        if (isNegative) groupedInteger = "-" + groupedInteger;  // Возвращаем минус

        if (fractionalPart != null)
        {
            return groupedInteger + nfi.NumberDecimalSeparator + fractionalPart;
        }
        else
        {
            return groupedInteger;
        }
    }

}
