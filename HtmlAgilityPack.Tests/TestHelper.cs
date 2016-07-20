
namespace HtmlAgilityPack.Tests
{
    using System;
    using System.IO;

    static class TestHelper
    {
        private static string directory;

        static TestHelper()
        {
            directory = Path.Combine(Directory.GetCurrentDirectory(),
                "HtmlAgilityPack.Tests\\Resources");
        }

        public static string GetLocalPath(string file)
        {
            return Path.Combine(directory, file);
        }

        public static HtmlDocument Load(string file)
        {
            HtmlDocument doc;

            using (var reader = new StreamReader(Path.Combine(directory, file)))
            {
                doc = HtmlDocument.Load(reader);
            }

            return doc;
        }

        public static void Save(HtmlDocument doc, string file)
        {
            using (var writer = new StreamWriter(file, false, doc.GetOutEncoding()))
            {
                doc.Save(writer);
            }
        }
    }
}
