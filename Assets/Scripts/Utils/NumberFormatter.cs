using System.Globalization;

public static class NumberFormatter
{
    public static string FormatNumber(float number)
    {
        return number.ToString("0.##00", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    public static string FormatNumberWithGrouping(float value)
    {
        // —начала форматируем число с нужным количеством знаков после зап€той (через FormatNumber)
        string formatted = FormatNumber(value);

        // –аздел€ем целую и дробную части
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

        // ‘орматируем целую часть с группировкой по 3 цифры через пробел
        // ƒл€ этого используем NumberFormatInfo с нужным разделителем групп
        var nfi = new NumberFormatInfo()
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ","
        };

        // ѕреобразуем целую часть в число дл€ форматировани€
        // »спользуем long, т.к. цела€ часть может быть большой
        if (!long.TryParse(integerPart, out long integerNumber))
        {
            // ≈сли не удалось распарсить, возвращаем исходное значение
            return formatted.Replace('.', ',');
        }

        string groupedInteger = integerNumber.ToString("N0", nfi);

        // ‘ормируем итоговую строку с дробной частью (замен€ем точку на зап€тую)
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
