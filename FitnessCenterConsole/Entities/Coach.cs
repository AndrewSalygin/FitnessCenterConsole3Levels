using System;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;

namespace FitnessCenterConsole.Entities {
    public enum Education {
        None,
        Secondary,
        College,
        Higher
    };
    public class Coach {
        private static int _idGlobal = 0;

        private int _id;
        private string _surname;
        private string _name;
        private string _middleName;
        private int _experience;
        private Education _education;
        private DateTime _birthday;
        private string _phoneNumber;

        [JsonConstructor]
        public Coach(string surname, string name, string middleName, int experience, 
            Education education, DateTime birthday, string phoneNumber) {
            Id = IdGlobal;
            IdGlobal++;
            Surname = surname;
            Name = name;
            MiddleName = middleName;
            if (experience < 0)
                throw new WrongValueException($"Ошибка: опыт тренера {Surname} должен быть не меньше 0.");
            else
                Experience = experience;
            Birthday = birthday;
            if ((int)education < 0 || (int)education >= Enum.GetNames(typeof(Education)).Length) {
                throw new WrongValueException($"Ошибка: неверно выбран тип обучения для тренера {Surname}.");
            }
            else {
                Education = education;
            }
            if (phoneNumber.Length != 11) {
                PhoneNumber = "отсутствует";
            } else {
                PhoneNumber = phoneNumber;
            }
        }

        // свойства
        public static int IdGlobal { get => _idGlobal; set => _idGlobal = value; }
        public int Id { get => _id; set => _id = value; }
        public string PhoneNumber { get => _phoneNumber; set => _phoneNumber = value; }
        public int Experience { get => _experience; set => _experience = value; }
        public DateTime Birthday { get => _birthday; set => _birthday = value; }
        public Education Education { get => _education; set => _education = value; }
        public string Surname { get => _surname; set => _surname = value; }
        public string Name { get => _name; set => _name = value; }
        public string MiddleName { get => _middleName; set => _middleName = value; }

        public override string ToString() {
            return $"ID: {Id} \n" +
                   $"Фамилия: {Surname} \n" +
                   $"Имя: {Name} \n" +
                   $"Отчество: {MiddleName} \n" +
                   $"Дата рождения: {Birthday.ToShortDateString()} \n" +
                   $"Образование: {educationToRussianLanguage(Education)} \n" +
                   $"Опыт: {Experience} \n" +
                   $"Номер телефона: {PhoneNumber} \n"; 
        }

        private string educationToRussianLanguage(Education education) {
            switch (education.ToString()) {
                case "None": return "отсутствует";
                case "Secondary": return "среднее";
                case "College": return "колледж";
                case "Higher": return "высшее";
            }
            return "отсутствует";
        }
    }
}
