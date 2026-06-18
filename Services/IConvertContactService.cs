using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using ContactToVCard.Helpers;

namespace ContactToVCard.Services;

public interface IConvertContactService
{
    public bool ConvertAndSaveContact(string file, string outputFolder);
}

public class ConvertContactService : IConvertContactService
{
    /// <summary>
    /// Read and convert the CONTACT to VCard format, then save.
    ///
    /// This method ignores namespaces and uses the "LocalName" property to match elements.
    ///
    /// Note that the output name will match the input file name.
    /// </summary>
    /// <param name="file">The input .CONTACT XML file.</param>
    /// <param name="outputFolder">The folder location to save the VCard file.</param>
    /// <returns></returns>
    public bool ConvertAndSaveContact(string file, string outputFolder) // todo could be void if not validating.
    {
        // Load the CONTACT.
        var doc = XDocument.Load(file);
        
        var vcfPath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(file) + ".vcf");
        
        // Extract data from the CONTACT document and write it to a stream.
        using var writer = new StreamWriter(vcfPath);
        
        writer.WriteLine("BEGIN:VCARD");
        writer.WriteLine("VERSION:3.0");
        
        var names = GetNames(doc.GetNodeByLocalName("NameVerification")); // todo out variable would be pretty...
        writer.WriteLine($"N:{names.first};{names.last};;;");
        writer.WriteLine($"FN:{names.formatted}");

        var phones = GetPhones(doc.GetNodeByLocalName("PhoneNumberCollection"));
        foreach (var number in phones) writer.WriteLine($"TEL;TYPE={number.Type.ToString()},VOICE:{number.Number}");

        var email = GetEmail(doc.GetNodeByLocalName("EmailAddressCollection"));
        if (!string.IsNullOrEmpty(email)) writer.WriteLine($"EMAIL;TYPE=PREF,INTERNET:{email}");

        writer.WriteLine("END:VCARD");

        return true;
    }

    private static (string first, string last, string formatted) GetNames(XElement? nameNode)
    {
        var firstName = nameNode?.GetNodeByLocalName("GivenName")?.Value ?? "";
        var lastName = nameNode?.GetNodeByLocalName("FamilyName")?.Value ?? "";
        var formattedName = nameNode?.GetNodeByLocalName("FormattedName")?.Value ?? $"{firstName} {lastName}".Trim();
        
        return (firstName, lastName, formattedName);
    }

    private static IList<ContactNumber> GetPhones(XElement? phoneNode)
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

    private static string GetEmail(XElement? emailNode)
    {
        if (emailNode == null) return "";
        
        return emailNode.GetNodeByLocalName("EmailAddress")?.GetNodeByLocalName("Address")?.Value ?? "";
    }
}

public class ContactNumber
{
    public ContactNumberType Type { get; set; } = ContactNumberType.Cell;
    public string Number { get; set; } = "";
}

public enum ContactNumberType
{
    Cell = 1,
    Home = 2,
    Work = 3
}