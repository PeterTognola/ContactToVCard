using System.IO;
using System.Linq;

namespace ContactToVCard.Helpers;

public static class VcfWriterHelpers
{
    public static void WriteVcfLine(this StreamWriter writer, params string[] values)
    {
        switch (values.Length)
        {
            case 2:
                writer.WriteLine($"{values[0]}:{values[1]}");
                break;
            case 3:
                writer.WriteLine($"{values[0]};{values[1]}:{values[2]}");
                break;
            case > 3:
                writer.WriteLine($"{values[0]};{values[1]}:{string.Join(";", values.Skip(2))}");
                break;
        }
    }
}