using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serialization_task
{
    #region Models
    class Person
    {
        public Int32 Id { get; set; }
        public Guid TransportId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Int32 SequenceId { get; set; }
        public String[] CreditCardNumbers { get; set; }
        public Int32 Age { get; set; }
        public String[] Phones { get; set; }
        public Int64 BirthDate { get; set; }
        public Double Salary { get; set; }
        public Boolean IsMarred { get; set; }
        public Gender Gender { get; set; }
        public Child[] Children { get; set; }
    }
    class Child
    {
        public Int32 Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Int64 BirthDate { get; set; }
        public Gender Gender { get; set; }
    }
    enum Gender
    {
        Male,
        Female
    }
    #endregion

    class Program
    {
        private static readonly Random rnd = new();

        private static Person GenerateRandomPerson(int i)
        {
            int month = rnd.Next(1, 12);
            int day = rnd.Next(1, 28);
            int year = rnd.Next(1960, 2000);
            var birthDate = new DateTime(year, month, day);
            var age = DateTime.UtcNow.Year - birthDate.Year;
            var childBirthDate = new DateTime(year + rnd.Next(18, 23), month, day);

            var person = new Person()
            {
                Id = -1,
                TransportId = Guid.NewGuid(),
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                SequenceId = -1,
                CreditCardNumbers = new[] { $"{i}", $"{i + 1}" },
                Age = age,
                Phones = new[] { $"+7{i}" },
                BirthDate = ((DateTimeOffset)birthDate).ToUnixTimeSeconds(),
                Salary = (double)rnd.Next(20000, 90000),
                IsMarred = 1 == rnd.Next(2),
                Gender = rnd.Next(2) == 1 ? Gender.Male : Gender.Female,
                Children = new[]
                {
                    new Child()
                    {
                        Id = 0,
                        FirstName = $"ChildFirstName{i}",
                        LastName = $"ChildLastName{i}",
                        BirthDate = ((DateTimeOffset)childBirthDate).ToUnixTimeSeconds(),
                        Gender = rnd.Next(2) == 1 ? Gender.Male : Gender.Female
                    }
                }
            };

            return person;
        }

        private static List<Person> GenerateListRandomPersons(int count)
        {
            List<Person> persons = new List<Person>();

            for (int i = 0; i < count; i++)
            {
                var person = GenerateRandomPerson(i);

                persons.Add(person);
            }

            return persons;
        }

        static void Main(string[] args)
        {
            // Directory selection by user
            string path;

            while (true)
            {
                Console.Write("Enter the path: ");
                path = Console.ReadLine()!;
                

                if (Directory.Exists(path))
                {
                    break;
                }

                Console.WriteLine("Invalid path");
            }

            var fileName = $"{path}\\Persons.json";

            // Generation of 10000 random objects
            var persons = GenerateListRandomPersons(10000);

            // Convert list to JSON and create Persons.json
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                MaxDepth = 4
            };

            var jsonString = JsonSerializer.Serialize(persons, serializeOptions);

            File.WriteAllText(fileName, jsonString);
            

            // Clear memory
            persons.Clear();

            // Read objects from file
            jsonString = File.ReadAllText(fileName);
            persons = JsonSerializer.Deserialize<List<Person>>(jsonString, serializeOptions);

            // Display in console persons count, persons credit card count, the average value of child age
            var personsCreditCardCount = 0;
            var childCount = 0;
            var ageSum = 0;

            foreach (var p in persons)
            {
                personsCreditCardCount += p.CreditCardNumbers.Length;

                if (p.Children != null)
                {
                    childCount += p.Children.Length;
                    foreach (var child in p.Children)
                    {
                        var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        var birthDate = unixTime.AddSeconds(child.BirthDate).ToUniversalTime();

                        ageSum += DateTime.UtcNow.Year - birthDate.Year;
                    }
                }
            }

            Console.WriteLine($"Persons count: {persons.Count}");
            Console.WriteLine($"Persons credit card count: {personsCreditCardCount}");
            Console.WriteLine($"Average value of child age: {ageSum / childCount}");
        }
    }
}