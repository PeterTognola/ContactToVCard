using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContactToVCard.Services;

public interface IConvertContactService
{
    public bool ConvertAsync(string file, string outputFolder);
}

public class ConvertContactService : IConvertContactService
{
    public bool ConvertAsync(string file, string outputFolder)
    {
        var doc = XDocument.Load(file);
        var ns = doc.Root.GetDefaultNamespace();
        
        var nameNode = doc.Descendants(ns + "NameVerification").FirstOrDefault();
        var firstName = nameNode?.Element(ns + "GivenName")?.Value ?? "";
        var lastName = nameNode?.Element(ns + "FamilyName")?.Value ?? "";
        var formattedName = nameNode?.Element(ns + "FormattedName")?.Value ?? $"{firstName} {lastName}".Trim();
        
        var phoneNodes = doc.Descendants(ns + "PhoneNumberCollection").Elements(ns + "PhoneNumber");
        var mobilePhone = phoneNodes.FirstOrDefault(p => p.Element(ns + "LabelCollection")?.Elements(ns + "Label").Any(l => l.Value == "Cellular") == true)?.Element(ns + "Number")?.Value ?? "";
        var homePhone = phoneNodes.FirstOrDefault(p => p.Element(ns + "LabelCollection")?.Elements(ns + "Label").Any(l => l.Value == "Home") == true)?.Element(ns + "Number")?.Value ?? "";
        var workPhone = phoneNodes.FirstOrDefault(p => p.Element(ns + "LabelCollection")?.Elements(ns + "Label").Any(l => l.Value == "Work") == true)?.Element(ns + "Number")?.Value ?? "";
        
        var emailNode = doc.Descendants(ns + "EmailAddressCollection").Elements(ns + "EmailAddress").FirstOrDefault();
        var email = emailNode?.Element(ns + "Address")?.Value ?? "";
        
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