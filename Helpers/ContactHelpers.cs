using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace ContactToVCard.Helpers;

public static class ContactHelpers
{
    [SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper")]
    public static XElement? GetNodeByLocalName(this XDocument document, string localName)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(localName);

        return 
            !document.Descendants().Any()
                ? null
                : document.Descendants().FirstOrDefault(e => e.Name.LocalName == localName);
    }
    
    public static XElement? GetNodeByLocalName(this XDocument document, string[] localName) 
        => localName.Select(document.GetNodeByLocalName).OfType<XElement>().FirstOrDefault();

    extension(XElement element)
    {
        [SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper")]
        public XElement? GetNodeByLocalName(string localName)
        {
            ArgumentNullException.ThrowIfNull(element);
            ArgumentException.ThrowIfNullOrWhiteSpace(localName);

            return element.RecursivelyGetElement(localName, 2);
        }

        private XElement? RecursivelyGetElement(string name, int maxDepth = 2)
        {
            var elements = element.Elements().ToList();
        
            if (elements == null || !elements.Any() || maxDepth == 0) return null;
        
            var result = elements.FirstOrDefault(e => e.Name.LocalName == name);
            if (result != null) return result;

            foreach (var e in elements)
            {
                var re = e.RecursivelyGetElement(name, maxDepth - 1);
                
                if (re != null) return re;
            }
            
            return null;
        }
    }
}