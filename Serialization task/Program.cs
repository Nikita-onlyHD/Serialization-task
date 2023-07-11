using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serialization_task
{
    class Program
    {
        static void Main(string[] args)
        {
            // Generation of 10000 random objects
            List<Person> persons = new List<Person>();

            var rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                int month = rnd.Next(1, 12);
                int day = rnd.Next(1, 28);
                int year = rnd.Next(1960, 2000);
                var birthDate = new DateTime(year, month, day);
                var age = DateTime.UtcNow.Year - birthDate.Year;
                var childBirthDate = new DateTime(year + rnd.Next(18, 23), month, day);


                var person = new Person()
                {
                    Id = i,
                    TransportId = Guid.NewGuid(),
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    SequenceId = i,
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

                persons.Add(person);
            }

            // Directory selection by user
            Console.WriteLine("Enter the path: ");
            string path = Console.ReadLine();
            var fileName = $"{path}/Persons.json";

            // Convert list to JSON and create Persons.json
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                MaxDepth = 5
            };

            var jsonString = JsonSerializer.Serialize(persons, serializeOptions);

            try
            {
                File.WriteAllText(fileName, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wrong path");
            }
            
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
}