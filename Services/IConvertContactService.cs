namespace ContactToVCard.Services;

public interface IConvertContactService
{
    public bool ConvertAndSaveContact(string file, string outputFolder);
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