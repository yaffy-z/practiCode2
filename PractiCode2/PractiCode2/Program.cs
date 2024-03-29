﻿using PractiCode2;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
static HtmlElement Serialize(List<string> htmlLines)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlLines)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Helper.HtmlTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                // Handle class attribute
                var attributesClass = attributes.First(attr => attr.StartsWith("class"));
                var classes = attributesClass.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);

            // Handle ID
            var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Helper.SelfClosingTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}
var html = await Load("https://www.discountbank.co.il/");
var clean = new Regex("\\s+").Replace(html, " ");
var lines = new Regex("<(.*?>)").Split(clean).Where(l => l.Length > 0);
var root = Serialize(lines.ToList());
string query = "ul li span a";
var selector = Selector.FromQueryString(query);
Console.WriteLine("selector: ");
Console.WriteLine(selector);
var all =root.FindElements(selector);
foreach (var item in all)
{
    Console.WriteLine(item+ "\n \n");
}
foreach (var item in all)
{
    Console.WriteLine("\n \n \n ---Ancestors----");
     var ancestors=item.Ancestors();
    foreach (var i in ancestors)
    {
        Console.WriteLine(i);
    }
}
Console.ReadLine();
//$$("div#lang-switcher li.language-switcher__list-item")
//$$(".cookie-message #close-cookies.close-btn img")
//$$("ul li span a")


