using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DownMark
{
    public static class MarkdownBuilderExtensions
    {
        public static MarkdownBuilder Header(this MarkdownBuilder builder, string text, HeaderSize size = HeaderSize.H1)
        {
            StringBuilder stringBuilder = new();

            stringBuilder = size switch
            {
                HeaderSize.H1 => stringBuilder.Append('#'),
                HeaderSize.H2 => stringBuilder.Append("##"),
                HeaderSize.H3 => stringBuilder.Append("###"),
                HeaderSize.H4 => stringBuilder.Append("####"),
                HeaderSize.H5 => stringBuilder.Append("#####"),
                HeaderSize.H6 => stringBuilder.Append("######"),
                _ => stringBuilder.Append('#'),
            };

            stringBuilder.Append(' ').Append(text);
            var markdownText = stringBuilder.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder Code(this MarkdownBuilder builder, string code, string fileFormat = "")
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"```{fileFormat}")
                .AppendLine(code)
                .AppendLine("```");

            var markdownText = stringBuilder.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder Link(this MarkdownBuilder builder, string url, string title, string altText = "")
        {
            StringBuilder sb = new();
            sb.Append('[').Append(title).Append("](").Append(url);

            if (!string.IsNullOrWhiteSpace(altText))
            {
                sb.Append(' ').Append('"').Append(altText).Append('"');
            }
            sb.Append(')');

            var markdownText = sb.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder HorizontalLine(this MarkdownBuilder builder)
        {
            var markdownText = "***";
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder Image(this MarkdownBuilder builder, string url, string title, string altText = "")
        {
            StringBuilder sb = new();
            sb.Append("![").Append(title).Append("](").Append(url);

            if (!string.IsNullOrWhiteSpace(altText))
            {
                sb.Append(' ').Append('"').Append(altText).Append('"');
            }
            sb.Append(')');

            string markdownText = sb.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder Table(this MarkdownBuilder builder, DataTable table)
        {
            StringBuilder sb = new();

            sb.Append('|');
            foreach (DataColumn column in table.Columns)
            {
                sb.Append(column.ColumnName).Append('|');
            }
            sb.AppendLine().Append('|');
            foreach (DataColumn column in table.Columns)
            {
                sb.Append("---").Append('|');
            }
            sb.AppendLine();

            foreach (DataRow row in table.Rows)
            {
                sb.Append('|');
                foreach (object? cell in row.ItemArray)
                {
                    sb.Append(cell).Append('|');
                }
                sb.AppendLine();
            }
            var markdownText = sb.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static MarkdownBuilder TaskList(this MarkdownBuilder builder, List<TaskEntity> tasks)
        {
            StringBuilder stringBuilder = new();
            foreach (TaskEntity task in tasks)
            {
                char checkedChar = task.Checked ? 'x' : ' ';
                stringBuilder
                    .Append('-')
                    .Append(' ')
                    .Append('[')
                    .Append(checkedChar)
                    .Append(']')
                    .Append(' ')
                    .Append(task.Task)
                    .AppendLine();
            }
            string markdownText = stringBuilder.ToString();
            builder.Entities.Add(markdownText);
            return builder;
        }
        public static string Build(this MarkdownBuilder markdownBuilder)
        {
            StringBuilder stringBuilder = new();


            for (int i = 0; i < markdownBuilder.Entities.Count; i++)
            {
                stringBuilder.AppendLine(markdownBuilder.Entities[i]);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}