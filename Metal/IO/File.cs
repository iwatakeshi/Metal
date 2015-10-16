using System;

namespace Metal.IO {
  public static class File {
    /// <summary>
    /// Reads all text.
    /// </summary>
    /// <returns>The all text.</returns>
    /// <param name="file">File.</param>
    public static string ReadAllText (string path) {
      using (System.IO.StreamReader reader = new System.IO.StreamReader (path, System.Text.Encoding.UTF8)) {
        return reader.ReadToEnd ();
      }
    }
		
  }
}

