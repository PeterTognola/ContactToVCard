using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactToVCard.Services;

public interface IConvertContactService
{
    public Task ConvertAsync(List<string> files, string outputFolder);
}

public class ConvertContactService : IConvertContactService
{
    public Task ConvertAsync(List<string> files, string outputFolder)
    {
        throw new System.NotImplementedException();
    }
}