using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace FillAPixRobot.FilePersistance
{
    public class FilePersistanceManager
    {
        public ICollection<IActionMemory> LoadActionMemories(string fileName, ICollection<IActionMemory> actionMemories = null)
        {
            List<IActionMemory> result = new List<IActionMemory>();
            List<string> lines = new List<string>();

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }
            file.Close();

            result.AddRange(FileActionMemory.Parse(lines, actionMemories));

            return result;
        }

        public bool SaveActionMemories(string fileName, ICollection<IActionMemory> actionMemories)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach(IActionMemory actionMemory in actionMemories)
            {
                fileContent.AppendLine(FileActionMemory.Convert(actionMemory));
            }

            System.IO.File.WriteAllText(fileName, fileContent.ToString());
            return true;
        }
    }
}
