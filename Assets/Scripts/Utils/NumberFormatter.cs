using System.Globalization;

public static class NumberFormatter
{
    public static string FormatNumber(float number)
    {
        return number.ToString("0.##00", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    public static string FormatNumberWithGrouping(float value)
    {
        // ������� ����������� ����� � ������ ����������� ������ ����� ������� (����� FormatNumber)
        string formatted = FormatNumber(value);

        // ��������� ����� � ������� �����
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

        // ����������� ����� ����� � ������������ �� 3 ����� ����� ������
        // ��� ����� ���������� NumberFormatInfo � ������ ������������ �����
        var nfi = new NumberFormatInfo()
        {
            NumberGroupSeparator = " ",
            NumberDecimalSeparator = ","
        };

        // ����������� ����� ����� � ����� ��� ��������������
        // ���������� long, �.�. ����� ����� ����� ���� �������
        if (!long.TryParse(integerPart, out long integerNumber))
        {
            // ���� �� ������� ����������, ���������� �������� ��������
            return formatted.Replace('.', ',');
        }

        string groupedInteger = integerNumber.ToString("N0", nfi);

        // ��������� �������� ������ � ������� ������ (�������� ����� �� �������)
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
