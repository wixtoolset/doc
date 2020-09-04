// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixBuildTools.XsdToMarkdown
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public sealed class Program
    {
        public static int Main(string[] args)
        {
            if (!CommandLine.TryParseArguments(args, out var commandLine))
            {
                CommandLine.ShowHelp();
                return 1;
            }

            // Load 'em up.
            var xsds = commandLine.Files.Select(path => new Xsd(XDocument.Load(path)));
            Console.WriteLine($"XsdToMarkdown: Processing {xsds.Count()} XSDs.");

            // Put 'em together.
            var finalizedXsds = XsdFinalizer.Finalize(xsds);

            // Convert 'em to Markdown.
            var converter = new ConvertXsdToMarkdownCommand();
            var pages = finalizedXsds.SelectMany(xsd => converter.Convert(xsd));

            foreach (var page in pages)
            {
                var markdown = String.Join(Environment.NewLine, page.Content);
                var dir = Path.Combine(commandLine.OutputFolder, page.Id);
                var mdPath = Path.Combine(dir, "index.html.md");
                Directory.CreateDirectory(dir);
                File.WriteAllText(mdPath, markdown);
            }

            Console.WriteLine($"XsdToMarkdown: Wrote {pages.Count()} Markdown pages.");

            return 0;
        }
    }
}
