namespace BookStore.Helpers;

public static class DeclensionHelper
{
    public static string ToDeclension(this int number, string one, string two, string five)
    {
        var n = Math.Abs(number) % 100;

        if (n >= 11 && n <= 19)
            return five;

        var lastDigit = n % 10;

        if (lastDigit == 1)
            return one;

        if (lastDigit >= 2 && lastDigit <= 4)
            return two;

        return five;
    }
}