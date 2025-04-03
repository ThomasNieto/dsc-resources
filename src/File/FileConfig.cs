using System.Text;

public class FileConfig(string path, string contents, Encoding encoding)
{
    public string Path { get; } = path;
    public string Contents { get; } = contents;
    public Encoding Encoding { get; } = encoding;
}