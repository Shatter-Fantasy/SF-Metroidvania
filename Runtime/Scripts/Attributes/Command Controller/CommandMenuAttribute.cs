using System.Linq;

namespace SF.CommandModule
{
    public class CommandMenuAttribute : System.Attribute
    {
        public string FullPath;
        public string Name;

        public CommandMenuAttribute(string fullPath)
        {
            FullPath = fullPath;
            Name = FullPath.Split("/").Last();
        }
    }
}
