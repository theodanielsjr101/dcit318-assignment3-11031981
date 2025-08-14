using System;
using System.Collections.Generic;
using System.IO;

namespace Question4
{
    // a) Student
    public class Student
    {
        public int Id;
        public string FullName = "";
        public int Score;

        public string GetGrade() =>
            Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and <= 79 => "B",
                >= 60 and <= 69 => "C",
                >= 50 and <= 59 => "D",
                _ => "F"
            };
    }

    // b,c) Custom exceptions
    public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string m) : base(m) { } }
    public class MissingFieldException : Exception { public MissingFieldException(string m) : base(m) { } }

    // d) Processor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var result = new List<Student>();

            using var sr = new StreamReader(inputFilePath);
            string? line;
            int lineNo = 0;

            while ((line = sr.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length != 3)
                    throw new MissingFieldException($"Line {lineNo}: expected 3 fields (Id, FullName, Score).");

                var idStr = parts[0].Trim();
                var name = parts[1].Trim();
                var scoreStr = parts[2].Trim();

                if (!int.TryParse(idStr, out var id))
                    throw new Exception($"Line {lineNo}: invalid Id format.");

                if (!int.TryParse(scoreStr, out var score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: score '{scoreStr}' is not a valid integer.");

                result.Add(new Student { Id = id, FullName = name, Score = score });
            }

            return result;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var sw = new StreamWriter(outputFilePath);
            foreach (var s in students)
            {
                sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }

    // e) Main flow with error handling
    public class Program
    {
        public static void Main()
        {
            try
            {
                var processor = new StudentResultProcessor();

                // Adjust paths as needed (absolute or relative)
                var input = Path.Combine(AppContext.BaseDirectory, "input.txt");
                var output = Path.Combine(AppContext.BaseDirectory, "report.txt");

                var students = processor.ReadStudentsFromFile(input);
                processor.WriteReportToFile(students, output);

                Console.WriteLine($"Report written to: {output}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
