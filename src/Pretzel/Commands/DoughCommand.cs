using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Pretzel.Logic.Commands;
using Pretzel.Logic.Extensions;
using Pretzel.Logic.Templating.Context;

namespace Pretzel.Commands
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [CommandInfo(CommandName = "dough")]
    class DoughCommand : ICommand
    {
#pragma warning disable 649
        [Import] IFileSystem fileSystem;
        [Import] CommandParameters parameters;
        [Import] SiteContextGenerator contextGenerator;
#pragma warning restore 649

        public void Execute(IEnumerable<string> arguments)
        {
            Tracing.Info("dough - create a new post");
            parameters.Parse(arguments);

            var postsDirectory = Path.Combine(parameters.Path, "_posts");
            if (Directory.Exists(postsDirectory))
            {
                var postPath = Path.Combine(postsDirectory, string.Format("{0:yyyy-MM-dd}-{1}.md", DateTime.Now, GetFileSystemFriendlyTitle(parameters.PostTitle)));
                using (var stream = new StreamWriter(postPath, false, new UTF8Encoding(false)))
                {
                    stream.WriteLine("---");
                    stream.WriteLine("layout: post");
                    stream.WriteLine("title: {0}", parameters.PostTitle);
                    stream.WriteLine("---");
                    stream.WriteLine();
                }
            }
            else
            {
                Tracing.Error("Could not find posts directory. Make sure to run `pretzel create` before making dough.");
            }
        }

        string GetFileSystemFriendlyTitle(string title)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                title = title.Replace(c.ToString(), "");

            title = title.Replace(".", "");
            title = title.Replace(" ", "-");
            return title;
        }

        public void WriteHelp(TextWriter writer)
        {
            parameters.WriteOptions(writer, "-d", "-l", "-n");
        }
    }
}
