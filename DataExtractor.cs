using HtmlAgilityPack;
using OpenQA.Selenium;

namespace CheapestFlights
{
    internal class DataExtractor
    {
        public HtmlNodeCollection ExtractData(IWebDriver driver, string xPath)
        {
            var pageSource = driver.PageSource;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageSource);
            return htmlDocument.DocumentNode.SelectNodes(xPath);
        }

        public bool CheckElementExists(IWebDriver driver, string xpath)
        {
            try
            {
                driver.FindElement(By.XPath(xpath));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
