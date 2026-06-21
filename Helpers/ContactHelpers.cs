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
    
    [SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper")]
    public static XElement? GetNodeByLocalName(this XElement element, string localName)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentException.ThrowIfNullOrWhiteSpace(localName);

        return
            !element.Elements().Any()
                ? null
                : element.Elements().FirstOrDefault(e => e.Name.LocalName == localName);
    }
}