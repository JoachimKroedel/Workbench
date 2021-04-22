using FillAPixRobot.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace FillAPixRobot.FilePersistance
{
    public class FilePersistanceManager
    {
        private readonly string _fileName = @"D:\Temp\Conceptis Puzzles\ActionMemory.cpm"; // .cpm = conceptis puzzles memory

        public ICollection<IActionMemory> LoadActionMemories()
        {
            List<IActionMemory> result = new List<IActionMemory>();


            List<string> lines = new List<string>();

            System.IO.StreamReader file = new System.IO.StreamReader(_fileName);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }
            file.Close();

            result.AddRange(FileActionMemory.Parse(lines, null));

            return result;
        }

        public bool SaveActionMemories(IList<IActionMemory> actionMemories)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach(IActionMemory actionMemory in actionMemories)
            {
                fileContent.AppendLine(FileActionMemory.Convert(actionMemory));
            }

            System.IO.File.WriteAllText(_fileName, fileContent.ToString());
            return true;
        }
    }
}
