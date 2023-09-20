using System.Collections.Generic;

namespace DownMark
{
    public class MarkdownBuilder
    {

        private readonly List<string> entities;

        internal List<string> Entities => entities;

        public MarkdownBuilder()
        {
            entities = new List<string>();
        }
    }
}