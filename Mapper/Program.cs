using System;
using Mapper.Tools;

namespace Mapper
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonFull
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }

    internal static class Program
    {
        private static void Main()
        {
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var fullName = $"{person.FirstName} {person.LastName}";

            var personFull = Mapper<Person, PersonFull>
                .Create()
                .WithInputObject(person)
                .WithMapping("Full", "FullName")
                .WithMapping("FirstName", "LastName")
                .WithMapping("LastName", "FirstName")
                .WithMappingObject(new {Full = fullName})
                .Build()
                .Map();

            OutputProperties(personFull);
        }

        private static void OutputProperties<T>(T inputObject)
        {
            var properties = inputObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(inputObject)}");
            }
        }
    }
}