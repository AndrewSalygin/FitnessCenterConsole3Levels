using System;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;

namespace FitnessCenterConsole.Entities {
    public class Client {
        private static int _idGlobal = 0;

        private int _id;
        private string _surname;
        private string _name;
        private string _middleName;
        private DateTime _birthday;
        private string _phoneNumber;

        [JsonConstructor]
        public Client(string surname, string name, string middleName, DateTime birthday, string phoneNumber) {
            Id = IdGlobal;
            IdGlobal++;
            Surname = surname;
            Name = name;
            MiddleName = middleName;
            Birthday = birthday;
            if (phoneNumber.Length != 11) {
                PhoneNumber = "отсутствует";
            } else {
                PhoneNumber = phoneNumber;
            }
            
        }



        // Свойства
        public static int IdGlobal { get => _idGlobal; set => _idGlobal = value; }
        public int Id { get => _id; set => _id = value; }
        public string PhoneNumber { get => _phoneNumber; set => _phoneNumber = value; }
        public string Surname { get => _surname; set => _surname = value; }
        public string Name { get => _name; set => _name = value; }
        public string MiddleName { get => _middleName; set => _middleName = value; }
        public DateTime Birthday { get => _birthday; set => _birthday = value; }

        public override string ToString() {
            return $"ID: {Id} \n" +
                   $"Фамилия: {Surname} \n" +
                   $"Имя: {Name} \n" +
                   $"Отчество: {MiddleName} \n" +
                   $"Дата рождения: {Birthday.ToShortDateString()} \n" +
                   $"Номер телефона: {PhoneNumber} \n";
        }
    }
}
