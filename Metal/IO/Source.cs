namespace Metal.IO
{

  public class Source {
    string seperator = System.IO.Path.DirectorySeparatorChar.ToString();
    string directory = System.IO.Directory.GetCurrentDirectory();

    public Source() {
    }

    public Source(string source, bool isFile) {
      Initialize(null, source, 0, 0, 0, isFile);
    }

    public Source(string path, string source) {
      Initialize(path, source, 0, 0, 0, true);
    }

    public Source(string source, int line, int position, int column, bool isFile) {
      Initialize(null, source, line, position, column, isFile);
    }

    public Source(string path, string source, int line, int position, int column) {
      Initialize(path, source, line, position, column, true);
    }

    private void Initialize(string path, string source, int line, int position, int column, bool isFile) {
      Path = path;
      FileName = isFile ? source : "Interpreter";
      Line = line == 0 ? 1 : line;
      Position = position;
      Column = column == 0 ? 1 : column;
      File = isFile ? IO.File.ReadAllText(path + source) : source;
      Length = File.Length;
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="Metal.IO.Source"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="Metal.IO.Source"/>.</returns>
    public override string ToString() {
      return string.Format("FileName: '{0}', Path: '{1}', Line: {2}, Column: {3}, Position: {4}", FileName, Path, Line, Column, Position);
    }

    public string FileName { get; private set; }

    public string File { get; private set; }

    public string Path { get; private set; }

    public int Line { get; set; }

    public int Position { get; set; }

    public int Length { get; private set; }

    public int Column { get; set; }
  }
}

