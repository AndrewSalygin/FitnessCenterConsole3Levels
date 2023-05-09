using System;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;

namespace FitnessCenterConsole.Entities {
    public enum TypeOfGym {
        Common,
        Сardio,
        Gymnastics,
        Strength
    }
    public class Gym {
        private int _numberOfGym;
        private TypeOfGym _typeOfGym;

        [JsonConstructor]
        public Gym(int numberOfGym, TypeOfGym typeOfGym) {
            if (numberOfGym < 0) {
                throw new WrongValueException("Ошибка: номер зала должен быть не меньше 0.");
            } else {
                NumberOfGym = numberOfGym;
            }
            if ((int)typeOfGym < 0 || (int)typeOfGym >= Enum.GetNames(typeof(Education)).Length) {
                throw new WrongValueException($"Ошибка: неверно выбран тип зала номер {numberOfGym}.");
            } else {
                TypeOfGym = typeOfGym;
            }
        }

        public int NumberOfGym { get => _numberOfGym; set => _numberOfGym = value; }
        public TypeOfGym TypeOfGym { get => _typeOfGym; set => _typeOfGym = value; }

        public override string ToString() {
            return $"Номер зала: {NumberOfGym} \n" +
                   $"Тип зала: {typeOfGymToRussianLanguage(TypeOfGym)} \n";
        }

        private string typeOfGymToRussianLanguage(TypeOfGym typeOfGym) {
            switch (typeOfGym.ToString()) {
                case "Common": return "обычный";
                case "Сardio": return "кардио";
                case "Gymnastics": return "гимнастический";
                case "Strength": return "силовой";
            }
            return "обычный";
        }
    }
}
