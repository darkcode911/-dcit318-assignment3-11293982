using System;
using System.Collections.Generic;
using System.IO;

namespace GradingSystem
{
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();
            string[] lines = File.ReadAllLines(inputFilePath);

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = line.Split(',');

                if (fields.Length != 3)
                {
                    throw new MissingFieldException($"Error on line {i+1}: The line does not contain 3 fields. Content: '{line}'");
                }

                if (!int.TryParse(fields[0].Trim(), out int id))
                {
                }

                string fullName = fields[1].Trim();

                if (!int.TryParse(fields[2].Trim(), out int score))
                {
                    throw new InvalidScoreFormatException($"Error on line {i+1}: Score '{fields[2]}' is not a valid integer.", new FormatException());
                }

                students.Add(new Student(id, fullName, score));
            }
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("--- Student Grade Report ---");
                writer.WriteLine($"Report generated on: {DateTime.Now}");
                writer.WriteLine("----------------------------");
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
                writer.WriteLine("----------------------------");
                writer.WriteLine("Report finished.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "students.txt";
            string outputFile = "report.txt";
            var processor = new StudentResultProcessor();

            try
            {
                Console.WriteLine($"Reading from '{inputFile}'...");
                List<Student> students = processor.ReadStudentsFromFile(inputFile);
                Console.WriteLine("Successfully read and validated student data.");

                Console.WriteLine($"Writing report to '{outputFile}'...");
                processor.WriteReportToFile(students, outputFile);
                Console.WriteLine("Report successfully generated.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"ERROR: The input file was not found. Please ensure '{inputFile}' exists.");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"ERROR: A record was incomplete. {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"ERROR: A score was in an incorrect format. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}