namespace NoP77svk.Text.IO
{
    using System;
    using System.IO;

    public class PathExt
    {
        public static char FolderDelimiter { get; set; } = Path.DirectorySeparatorChar;

        public static string? Sanitize(string? path, char folderDelimiter)
        {
            if (path is null)
                return path;
            else
                return folderDelimiter + path.Trim(folderDelimiter);
        }

        public static string? Sanitize(string? path)
        {
            return Sanitize(path, FolderDelimiter);
        }

        public static string? TrimLeadingPath(string? fullPath, string? leadingPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            else if (string.IsNullOrEmpty(leadingPath))
                return fullPath;
            else if (fullPath.StartsWith(leadingPath))
                return fullPath[leadingPath.Length..];
            else
                throw new ArgumentException($"There's no leading \"{leadingPath}\" to be trimmed from \"{fullPath}\"");
        }

        public static string? TrimLastLevel(string? path, char folderDelimiter)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            else
                return path.Substring(0, path.LastIndexOf(folderDelimiter));
        }

        public static string? TrimLastLevel(string? path)
        {
            return TrimLastLevel(path, FolderDelimiter);
        }

        public static string? GetLastLevel(string? path, char folderDelimiter)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            else
                return path.Substring(path.LastIndexOf(folderDelimiter) + 1);
        }

        public static string? GetLastLevel(string? path)
        {
            return GetLastLevel(path, FolderDelimiter);
        }
    }
}
