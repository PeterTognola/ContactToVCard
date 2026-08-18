using System.Collections.Generic;

namespace ContactToVCard.Models;

public class Contact
{
    public string Name { get; set; }
    public string Title { get; set; }
    public IList<ContactField> PhoneNumbers { get; set; }
    public IList<ContactField> Emails { get; set; }
    public IList<ContactField> Addresses { get; set; }
    public IList<ContactField> Urls { get; set; }
}

public class ContactField
{
    public string Value { get; set; }
    
    public IList<string> Labels { get; set; }
}