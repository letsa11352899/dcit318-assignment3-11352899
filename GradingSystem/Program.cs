using System;
using System.Collections.Generic;
using System.IO;

// --- STUDENT CLASS ---
public class Student
{
    public int Id;
    public string FullName;
    public int Score;

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

    public override string ToString()
    {
        return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }
}

// --- CUSTOM EXCEPTIONS ---
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// --- MAIN PROGRAM ---
public class Program
{
    public static void Main()
    {
        List<Student> students = new List<Student>();

        Console.Write("How many students do you want to enter? ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\n--- Student {i + 1} ---");

            try
            {
                // ID
                Console.Write("Enter ID: ");
                string idInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(idInput))
                    throw new MissingFieldException("ID is missing.");
                int id = int.Parse(idInput);

                // Name
                Console.Write("Enter Full Name: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                    throw new MissingFieldException("Full name is missing.");

                // Score
                Console.Write("Enter Score: ");
                string scoreInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(scoreInput))
                    throw new MissingFieldException("Score is missing.");
                if (!int.TryParse(scoreInput, out int score))
                    throw new InvalidScoreFormatException("Score must be a number.");

                students.Add(new Student(id, name, score));
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                i--; // retry same student
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                i--; // retry same student
            }
        }

        // Output file
        Console.Write("\nEnter file path to save report (e.g., report.txt): ");
        string filePath = Console.ReadLine();

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine(student);
                }
            }
            Console.WriteLine($"Report saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }

        // Display final report
        Console.WriteLine("\n--- Student Report ---");
        foreach (var student in students)
        {
            Console.WriteLine(student);
        }
    }
}
