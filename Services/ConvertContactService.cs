using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ContactToVCard.Helpers;

namespace ContactToVCard.Services;

public class ConvertContactService : IConvertContactService
{
    private const string NameNodeName = "NameVerification";
    private const string PhoneNodeName = "PhoneNumberCollection";
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
        
        var vcfPath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(file) + ".vcf");
        
        // Extract data from the CONTACT document and write it to a stream.
        using var writer = new StreamWriter(vcfPath);
        
        // Begin VCard.
        writer.WriteLine("BEGIN:VCARD");
        writer.WriteLine("VERSION:3.0");

        if (TryParseNames(doc.GetNodeByLocalName(NameNodeName), out var names))
        {
            writer.WriteLine($"N:{names.first};{names.last};;;");
            writer.WriteLine($"FN:{names.formatted}");
        }
        else
        {
            return false;
        }

        var phones = GetPhones(doc.GetNodeByLocalName(PhoneNodeName));
        foreach (var number in phones) writer.WriteLine($"TEL;TYPE={number.Type.ToString()},VOICE:{number.Number}");
        
        if (TryParseEmail(doc.GetNodeByLocalName(EmailNodeName), out var email)) writer.WriteLine($"EMAIL;TYPE=PREF,INTERNET:{email}");

        // End VCard.
        writer.WriteLine("END:VCARD");

        return true;
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

    private static List<ContactNumber> GetPhones(XElement? phoneNode)
    {
        // Extract Phone Numbers safely by matching LocalName and inner Label values
        if (phoneNode == null) return [];
        
        var nodes = phoneNode.Elements().Where(e => e.Name.LocalName == "PhoneNumber");
        
        return (from phone in nodes
            let labels = GetLabels(phone)
            select new ContactNumber
            {
                // Default type to cellular if nothing matches.
                Type = labels.Contains("Home")
                    ? ContactNumberType.Home
                    : labels.Contains("Work")
                        ? ContactNumberType.Work
                        : ContactNumberType.Cell,
                Number = phone.GetNodeByLocalName("Number")?.Value ?? ""
            }).ToList();

        IEnumerable<string> GetLabels(XElement phone) => phone.Descendants().Where(e => e.Name.LocalName == "Label").Select(l => l.Value);
    }

    private static bool TryParseEmail(XElement? emailNode, out string email)
    {
        email = emailNode?.GetNodeByLocalName("EmailAddress")?.GetNodeByLocalName("Address")?.Value ?? "";
        
        return !string.IsNullOrWhiteSpace(email);
    }
}