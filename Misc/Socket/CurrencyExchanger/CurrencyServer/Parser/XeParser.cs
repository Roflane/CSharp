using HtmlAgilityPack;

namespace CurrencyServer.Parser;

public static class XeParser {
    public static string GetResult(HtmlWeb web, string amount, string from, string to) {
        string html = $"https://www.xe.com/currencyconverter/convert/?Amount={amount}&From={from}&To={to}";
        var doc = web.Load(html);
        return doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'sc-c5062ab2-1 jKDFIr')]").InnerText;
    }
}