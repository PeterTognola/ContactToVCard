using System.IO;
using System.Linq;
using System.Xml.Linq;
using ContactToVCard.Helpers;

namespace ContactToVCard.Services;

public interface IConvertContactService
{
    public bool ConvertAndSaveContact(string file, string outputFolder);
}

public class ConvertContactService : IConvertContactService
{
    public bool ConvertAndSaveContact(string file, string outputFolder)
    {
        // Load the XML document safely
        var doc = XDocument.Load(file);

        // Bypassing namespaces completely by checking the 'LocalName' property
        var nameNode = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "NameVerification");
        var firstName = nameNode?.Elements().FirstOrDefault(e => e.Name.LocalName == "GivenName")?.Value ?? "";
        var lastName = nameNode?.Elements().FirstOrDefault(e => e.Name.LocalName == "FamilyName")?.Value ?? "";
        var formattedName = nameNode?.Elements().FirstOrDefault(e => e.Name.LocalName == "FormattedName")?.Value ?? $"{firstName} {lastName}".Trim();

        // Extract Phone Numbers safely by matching LocalName and inner Label values
        var phoneCollection = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "PhoneNumberCollection");
        var phoneNodes = phoneCollection?.Elements().Where(e => e.Name.LocalName == "PhoneNumber") ?? Enumerable.Empty<XElement>();

        var mobilePhone = "";
        var homePhone = "";
        var workPhone = "";

        foreach (var phone in phoneNodes)
        {
            string number = phone.Elements().FirstOrDefault(e => e.Name.LocalName == "Number")?.Value ?? "";
            var labels = phone.Descendants().Where(e => e.Name.LocalName == "Label").Select(l => l.Value);

            if (labels.Contains("Cellular")) mobilePhone = number;
            else if (labels.Contains("Home")) homePhone = number;
            else if (labels.Contains("Work")) workPhone = number;
        }

        // Extract Email Address safely
        var emailCollection = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "EmailAddressCollection");
        var emailNode = emailCollection?.Elements().FirstOrDefault(e => e.Name.LocalName == "EmailAddress");
        string email = emailNode?.Elements().FirstOrDefault(e => e.Name.LocalName == "Address")?.Value ?? "";

        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
        var vcfPath = Path.Combine(outputFolder, fileNameWithoutExt + ".vcf");
        
        // Save it.
        using var writer = new StreamWriter(vcfPath);
        
        writer.WriteLine("BEGIN:VCARD");
        writer.WriteLine("VERSION:3.0");
        writer.WriteLine($"N:{lastName};{firstName};;;");
        writer.WriteLine($"FN:{formattedName}");

        if (!string.IsNullOrEmpty(mobilePhone))
            writer.WriteLine($"TEL;TYPE=CELL,VOICE:{mobilePhone}");

        if (!string.IsNullOrEmpty(homePhone))
            writer.WriteLine($"TEL;TYPE=HOME,VOICE:{homePhone}");

        if (!string.IsNullOrEmpty(workPhone))
            writer.WriteLine($"TEL;TYPE=WORK,VOICE:{workPhone}");

        if (!string.IsNullOrEmpty(email))
            writer.WriteLine($"EMAIL;TYPE=PREF,INTERNET:{email}");

        writer.WriteLine("END:VCARD");

        return true; // handle non error failure.
    }
}