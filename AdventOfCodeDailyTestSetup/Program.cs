using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AdventOfCode2021;

namespace AdventOfCodeDailyTestSetup {
    internal class Program {
        [STAThread]
        public static void Main(string[] args) {
            string solutionRoot = args[0];
            if (string.IsNullOrEmpty(solutionRoot)) {
                Console.WriteLine("You need to add solution root as argument");
                return;
            }

            var referenceType = typeof(Day01);
            int day = FindLastDay(referenceType) + 1;

            string clipboard = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboard)) {
                Console.WriteLine("You need to copy the puzzle input into the clipboard");
                return;
            }

            File.WriteAllText(Path.Combine(solutionRoot, $"AdventOfCode2021/Input/day{day}.txt"), clipboard);
            
            CopyTemplate("ExerciseTemplate.txt", day, Path.Combine(solutionRoot, $"AdventOfCode2021/Day{day}.cs"));
            CopyTemplate("TestTemplate.txt", day, Path.Combine(solutionRoot, $"AdventOfCode2021.Test/Day{day}Tests.cs"));

            UpdateSlnFile(solutionRoot, day);
            UpdateTestSlnFile(solutionRoot, day);
        }

        private static void CopyTemplate(string templateFile, int day, string exerciseClassPath) {
            string[] exerciseLines = File.ReadAllLines(templateFile)
                .Select(x => x.Replace("{day}", day.ToString()))
                .ToArray();
            File.WriteAllLines(exerciseClassPath, exerciseLines);
        }

        private static void UpdateSlnFile(string solutionRoot, int day) {
            string slnPath = Path.Combine(solutionRoot, "AdventOfCode2021/AdventOfCode2021.csproj");
            var lines = File.ReadAllLines(slnPath).ToList();
            int index = lines.FindLastIndex(x => x.Contains("Include=\"Input\\day")) + 3;
            lines.Insert(index, "    </Content>");
            lines.Insert(index, "      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
            lines.Insert(index, $"    <Content Include=\"Input\\day{day}.txt\">");

            index = lines.FindLastIndex(x => x.Contains("<Compile Include=\"Day"));
            lines.Insert(index + 1, $"    <Compile Include=\"Day{day}.cs\" />");

            File.WriteAllLines(slnPath, lines);
        }
        
        private static void UpdateTestSlnFile(string solutionRoot, int day) {
            string slnPath = Path.Combine(solutionRoot, "AdventOfCode2021.Test/AdventOfCode2021.Test.csproj");
            var lines = File.ReadAllLines(slnPath).ToList();
            int index = lines.FindLastIndex(x => x.Contains("<Compile Include=\"Day"));
            lines.Insert(index + 1, $"    <Compile Include=\"Day{day}Tests.cs\" />");

            File.WriteAllLines(slnPath, lines);
        }
        
        
        private static int FindLastDay(Type referenceType)
        {
            return Enumerable.Range(1, 24).Last(i => referenceType.Assembly.GetType($"{referenceType.Namespace}.Day{i}") != null);
        }
    }
}