using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using ContactToVCard.Helpers;

namespace ContactToVCard.Services;

public class ConvertContactService : IConvertContactService
{
    private static readonly string[] NameNodeName =
    [
        "NameVerification",
        "NameCollection"
    ];

    private const string PhoneNodeName = "PhoneNumberCollection";
    private const string AddressNodeName = "PhysicalAddressCollection";
    private const string EmailNodeName = "EmailAddressCollection";
    
    /// <summary>
    /// Read and convert the CONTACT to VCard format, then save.
    ///
    /// This method ignores namespaces and uses the "LocalName" property to match elements.
    ///
    /// Note that the output name will match the input file name.
    /// </summary>
    /// <param name="file">The input .CONTACT XML file.</param>
    /// <param name="outputFolder">The folder location to save the VCard file.</param>
    /// <returns>True if document is valid and false on unrecoverable failure.</returns>
    public bool ConvertAndSaveContact(string file, string outputFolder)
    {
        // Load the CONTACT.
        var doc = XDocument.Load(file);
        
        // Get the output path.
        var vcfPath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(file) + ".vcf");
        
        // Write the VCard.
        return WriteBlueprint(vcfPath, writer =>
        {
            if (TryParseNames(doc.GetNodeByLocalName(NameNodeName), out var names))
            {
                writer.WriteLine($"N:{names.last};{names.first};;;");
                writer.WriteLine($"FN:{names.formatted}");
            }
            else
            {
                return false;
            }
            
            WritePhones(writer, doc.GetNodeByLocalName(PhoneNodeName), PhoneNodeName);
        
            // todo import type ADR;TYPE=work:;;STREET;CITY;COUNTY;POSTCODE;COUNTRY
            WriteAddresses(writer, doc.GetNodeByLocalName(AddressNodeName), AddressNodeName);
        
            if (TryParseEmail(doc.GetNodeByLocalName(EmailNodeName), out var email)) writer.WriteLine($"EMAIL;TYPE=PREF,INTERNET:{email}");

            return true;
        });
    }

    private static bool WriteBlueprint(string vcfPath, Func<StreamWriter, bool> contents)
    {
        // Extract data from the CONTACT document and write it to a stream.
        using var writer = new StreamWriter(vcfPath);
        
        // Begin VCard.
        writer.WriteLine("BEGIN:VCARD");
        writer.WriteLine("VERSION:3.0");
        
        var result = contents(writer);
        
        // End VCard.
        writer.WriteLine("END:VCARD");

        return result;
    }

    private static bool TryParseNames(XElement? nameNode, out (string first, string last, string formatted) result)
    {
        result = ("", "", "");
        if (nameNode == null) return false;

        var firstName = nameNode.GetNodeByLocalName("GivenName")?.Value ?? "";
        var lastName = nameNode.GetNodeByLocalName("FamilyName")?.Value ?? "";
        var formattedName = nameNode.GetNodeByLocalName("FormattedName")?.Value ?? $"{firstName} {lastName}".Trim();

        result = (firstName, lastName, formattedName);
        return true;
    }
    
    private static void WritePhones(StreamWriter writer, XElement? collectionNode, string collectionName)
    {
        collectionName = collectionName.Replace("Collection", "");
        
        if (collectionNode == null) return;
        
        foreach (var node in collectionNode.Elements().Where(x => x.Name.LocalName == collectionName))
        {
            if (TryParsePhone(node, out var phone)) writer.WriteLine($"TEL;TYPE={phone.Type.ToString()},VOICE:{phone.Number}");
        }
    }
    
    private static bool TryParsePhone(XElement? phoneNode, out ContactNumber phone)
    {
        phone = new ContactNumber();
        if (phoneNode == null) return false;

        phone = new ContactNumber
        {
            Number = phoneNode.GetNodeByLocalName("Number")?.Value ?? "",
            Type = ParseLabel(phoneNode)
        };

        return phone.Number.Length > 0;
    }

    private static bool TryParseEmail(XElement? emailNode, out string email)
    {
        email = emailNode?.GetNodeByLocalName("EmailAddress")?.GetNodeByLocalName("Address")?.Value ?? "";
        
        return !string.IsNullOrWhiteSpace(email);
    }

    private static void WriteAddresses(StreamWriter writer, XElement? collectionNode, string collectionName)
    {
        collectionName = collectionName.Replace("Collection", "");
        
        if (collectionNode == null) return;
        
        foreach (var node in collectionNode.Elements().Where(x => x.Name.LocalName == collectionName))
        {
            if (TryParseAddress(node, out var address)) writer.WriteLine($"ADR:;;{address.street};{address.city};{address.county};{address.postcode};{address.country}");
        }
    }
    
    private static bool TryParseAddress(XElement? nameNode, out (string street, string city, string county, string postcode, string country) result)
    {
        result = ("", "", "", "", "");
        if (nameNode == null) return false;

        var street = nameNode.GetNodeByLocalName("Street")?.Value.Replace("\n", ", ") ?? "";
        var city = nameNode.GetNodeByLocalName("City")?.Value.Replace("\n", "") ?? "";
        var county = nameNode.GetNodeByLocalName("State")?.Value.Replace("\n", "") ?? "";
        var postcode = nameNode.GetNodeByLocalName("PostalCode")?.Value.Replace("\n", "") ?? "";
        var country = nameNode.GetNodeByLocalName("Country")?.Value.Replace("\n", "") ?? "";
        
        result = (street, city, county, postcode, country);
        return true;
    }
    
    // todo for now we are not following the spec of multiple possible types. We are sticking to first. Also possible bug if used elsewhere it would default to Cell (e.g., for address this would be wrong default).
    private static ContactNumberType ParseLabel(XElement node)
    {
        var label = node.GetNodeByLocalName("Label").Value;

        return label.Contains("Home")
            ? ContactNumberType.Home
            : label.Contains("Work")
                ? ContactNumberType.Work
                : ContactNumberType.Cell;
    }
}