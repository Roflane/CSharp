using HtmlAgilityPack;

namespace CurrencyServer.Parser;

public static class XeParser {
    public static string GetResult(string amount, string from, string to) {
        HtmlWeb htmlWeb = new();
        string html = $"https://www.xe.com/currencyconverter/convert/?Amount={amount}&From={from}&To={to}";
        var doc = htmlWeb.Load(html);
        var node = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'sc-c5062ab2-1 jKDFIr')]");
        return node.InnerText;
    }
}