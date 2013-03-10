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
#pragma warning restore 649

        public void Execute(IEnumerable<string> arguments)
        {
            Tracing.Info("dough - create a new post");
            parameters.Parse(arguments);

            var postsDirectory = Path.Combine(parameters.Path, "_posts");
            if (Directory.Exists(postsDirectory))
            {
                var postPath = Path.Combine(postsDirectory, string.Format("{0:yyyy-MM-dd}-{1}.md", DateTime.Now, GetFileSystemFriendlyTitle(parameters.PostTitle)));
                var postContent = new StringBuilder()
                    .AppendLine("---")
                    .AppendLine("layout: post")
                    .AppendFormat("title: {0}", parameters.PostTitle).AppendLine()
                    .AppendLine("---")
                .ToString();

                fileSystem.File.WriteAllText(postPath, postContent, new UTF8Encoding(false));
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
