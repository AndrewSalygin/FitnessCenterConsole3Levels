using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;
using FitnessCenterConsole.ConsolePL;
using FitnessCenterConsole.Entities;

namespace FitnessCenterConsole.DAL {
    internal class Database : IDatabase {
        private Dictionary<string, Coach> _coaches;
        private Dictionary<string, Client> _clients;
        private Dictionary<int, Gym> _gyms;
        private Schedule _schedule;

        public Dictionary<string, Coach> Coaches {
            get => _coaches;
            set => _coaches = value;
        }

        public Dictionary<string, Client> Clients {
            get => _clients;
            set => _clients = value;
        }

        public Dictionary<int, Gym> Gyms {
            get => _gyms;
            set => _gyms = value;
        }

        public Schedule Schedule {
            get => _schedule;
            set => _schedule = value;
        }

        // конструктор для новой базы
        internal Database() {
            _coaches = new Dictionary<string, Coach>();
            _clients = new Dictionary<string, Client>();
            _gyms = new Dictionary<int, Gym>();
            _schedule = new Schedule();
        }

        [JsonConstructor]
        // конструктор для json
        public Database(Dictionary<string, Coach> coaches, Dictionary<string, Client> clients,
            Dictionary<int, Gym> gyms, Schedule schedule) {
            _coaches = coaches;
            _clients = clients;
            _gyms = gyms;
            _schedule = schedule;
        }

        // вывод исключений
        internal static void PrintToApp(string text) {
            Console.WriteLine(text);
        }

        // добавление сущностей
        public bool AddNewCoach(string surname, string name, string middleName, int experience,
            Education education, DateTime birthday, string phoneNumber) {
            Coach temp;
            try {
                temp = new Coach(Coaches.Count, surname, name, middleName, experience, education, birthday,
                    phoneNumber);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            try {
                Coaches.Add(surname + '_' + phoneNumber, temp);
            } catch (ArgumentException ex) {
                PrintToApp($"Ошибка: добавление тренера {surname} с уже существующим номером в базе.");
                return false;
            }

            return true;
        }

        public bool AddNewClient(string surname, string name, string middleName, DateTime birthday,
            string phoneNumber) {
            Client temp;
            try {
                temp = new Client(Clients.Count, surname, name, middleName, birthday, phoneNumber);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            try {
                Clients.Add(surname + '_' + phoneNumber, temp);
            } catch (ArgumentException ex) {
                PrintToApp($"Ошибка: добавление клиента {surname} с уже существующим номером в базе.");
                return false;
            }

            return true;
        }

        public bool AddNewGym(int numberOfGym, TypeOfGym typeOfGym) {
            Gym temp;
            try {
                temp = new Gym(numberOfGym, typeOfGym);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            try {
                Gyms.Add(numberOfGym, temp);
            } catch (ArgumentException ex) {
                PrintToApp($"Ошибка: добавление зала номер {numberOfGym} с уже существующим номером в базе.");
                return false;
            }

            return true;
        }

        // добавление клиентов в занятие
        public bool AddClientToTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            string searchElementCoach = new string(surnameCoach + '_' + phoneNumberCoach);
            Schedule.Training training = Schedule.GetTrainingCoach(searchElementCoach, dateTime);
            string searchElementClient = new string(surnameClient + '_' + phoneNumberClient);
            // если тренеровка, в которую необходимо добавить клиента существует
            try {
                if (training != null) {
                    // если пользователь есть в базе
                    if (Clients.ContainsKey(searchElementClient)) {
                        if (training.ClientKeys.Contains(searchElementClient)) {
                            throw new WrongValueException($"Ошибка: клиент {surnameClient} уже записан на это занятие");
                        }

                        DateTime smallerBorderTime = dateTime.AddHours(-1);
                        DateTime largeBorderTime = dateTime.AddHours(1);

                        // проверка: клиент не записан на занятие в промежуток времени нового занятия
                        foreach (Schedule.Training localTraining in Schedule.Trainings) {
                            if (training != localTraining) {
                                if (localTraining.ClientKeys.Contains(searchElementClient) &&
                                    localTraining.DateTime > smallerBorderTime &&
                                    largeBorderTime > localTraining.DateTime) {
                                    throw new WrongValueException(
                                        $"Ошибка: клиент {surnameClient} уже записан на другое занятие в это время.");
                                }
                            }
                        }

                        training.ClientKeys.Add(searchElementClient);
                        training.CountOfClients++;
                        return true;
                    } else {
                        throw new WrongValueException($"Ошибка: клиент {surnameClient} не найден в базе.");
                    }
                } else {
                    throw new WrongValueException("Ошибка: данной тренировки не найдено в базе.");
                }
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        // удаление клиентов из занятия
        public bool DeleteClientFromTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            string searchElementCoach = new string(surnameCoach + '_' + phoneNumberCoach);
            Schedule.Training training = Schedule.GetTrainingCoach(searchElementCoach, dateTime);
            string searchElementClient = new string(surnameClient + '_' + phoneNumberClient);
            try {
                // если тренеровка, в которую необходимо добавить клиента существует
                if (training != null) {
                    // если пользователь есть в базе
                    if (Clients.ContainsKey(searchElementClient)) {
                        // если выбранная тренировка содержит пользователя
                        if (training.ClientKeys.Contains(searchElementClient)) {
                            training.ClientKeys.Remove(searchElementClient);
                            training.CountOfClients--;
                            return true;
                        }

                        throw new WrongValueException($"Ошибка: клиент {surnameClient} не записан на это занятие");
                    } else {
                        throw new WrongValueException($"Ошибка: клиент {surnameClient} не найден в базе.");
                    }
                } else {
                    throw new WrongValueException("Ошибка: данной тренировки не найдено в базе.");
                }
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        // обертка для класса Schedule
        // реализуем полиморфизм, так как информация на этапе планирования расписания может отсутствовать
        public bool AddNewTraining(int gymKey, string surnameCoach, string phoneNumberCoach, HashSet<string> clientKeys,
            DateTime dateTime) {
            string coachKey = new string(surnameCoach + '_' + phoneNumberCoach);
            try {
                // если указанный зал существует
                if (GetGymByNumber(gymKey) != null) {
                    // если указанный тренер существует
                    if (GetCoachByTuple(coachKey) != null) {
                        // если все клиенты в базе существуют
                        foreach (string client in clientKeys) {
                            if (GetClientByTuple(client) == null) {
                                throw new WrongValueException(
                                    $"Ошибка: клиент {client.Split('_')[0]} не найден в базе.");
                            }
                        }

                        return Schedule.AddTraining(gymKey, coachKey, clientKeys, dateTime);
                    }

                    throw new WrongValueException($"Ошибка: тренер {coachKey.Split('_')[0]} в базе не найден.");
                }

                throw new WrongValueException($"Ошибка: зал номер {gymKey} в базе не найден.");
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        public bool AddNewTraining(int gymKey, string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            string coachKey = new string(surnameCoach + '_' + phoneNumberCoach);
            try {
                if (GetGymByNumber(gymKey) != null) {
                    if (GetCoachByTuple(coachKey) != null) {
                        return Schedule.AddTraining(gymKey, coachKey, dateTime);
                    }

                    throw new WrongValueException($"Ошибка: тренер {coachKey.Split('_')[0]} в базе не найден.");
                }

                throw new WrongValueException($"Ошибка: зал номер {gymKey} в базе не найден.");
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        public bool AddNewTraining(int gymKey, DateTime dateTime) {
            try {
                if (GetGymByNumber(gymKey) != null) {
                    return Schedule.AddTraining(gymKey, dateTime);
                }

                throw new WrongValueException($"Ошибка: зал номер {gymKey} в базе не найден.");
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        public bool DeleteTraining(int gymKey, DateTime dateTime) {
            try {
                return Schedule.DeleteTraining(gymKey, dateTime);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        // вывод полной информации о сущностях с актуальным
        public string GetInfoCoach(string surname, string phoneNumber) {
            string searchElement = new string(surname + '_' + phoneNumber);
            Coach temp = GetCoachByTuple(searchElement);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null
                ? "\nНайдено:" +
                  "\n--------------------\n" +
                  temp.ToString() +
                  localSchedule.stringScheduleCoach()
                : $"Ошибка: тренера {searchElement.Split('_')[0]} не найдено в базе.";
        }

        public string GetInfoGym(int numberOfGym) {
            Gym temp = GetGymByNumber(numberOfGym);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null
                ? "\nНайдено:" +
                  "\n--------------------\n" +
                  temp.ToString() +
                  localSchedule.ToString()
                : $"Ошибка: зала номер {numberOfGym} не найдено в базе.";
        }

        public string GetInfoClient(string surname, string phoneNumber) {
            string searchElement = new string(surname + '_' + phoneNumber);
            Client temp = GetClientByTuple(searchElement);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null
                ? "\nНайдено:" +
                  "\n--------------------\n" +
                  temp.ToString() +
                  localSchedule.stringScheduleClient()
                : $"Ошибка: клиента {searchElement.Split('_')[0]} не найдено в базе.";
        }

        // получение экземпляра класса
        private Client GetClientByTuple(string searchElement) {
            // получает null, если не найден
            return Clients.GetValueOrDefault(searchElement);
        }

        private Coach GetCoachByTuple(string searchElement) {
            return Coaches.GetValueOrDefault(searchElement);
        }

        private Gym GetGymByNumber(int number) {
            return Gyms.GetValueOrDefault(number);
        }

        // удаление сущностей
        public bool DeleteCoach(string surname, string phoneNumber) {
            string temp = new string(surname + '_' + phoneNumber);
            try {
                if (!Coaches.Remove(temp)) {
                    throw new WrongValueException($"Ошибка: тренера {temp.Split('_')[0]} не найдено в базе.");
                }
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            return true;
        }

        public bool DeleteClient(string surname, string phoneNumber) {
            string temp = new string(surname + '_' + phoneNumber);
            bool client = Clients.ContainsKey(temp);
            try {
                if (!Clients.Remove(temp)) {
                    throw new WrongValueException($"Ошибка: клиента {temp.Split('_')[0]} не найдено в базе.");
                }
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            return true;
        }

        public bool DeleteGym(int numberOfGym) {
            try {
                if (!Gyms.Remove(numberOfGym)) {
                    throw new WrongValueException($"Ошибка: зала номер {numberOfGym} не найдено в базе.");
                }
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            return true;
        }

        // поиск по дате и времени для клиента/тренера, занятость зала
        public string FindTrainingCoach(string surname, string phoneNumber, DateTime dateTime) {
            string searchElement = new string(surname + '_' + phoneNumber);
            Schedule.Training training = Schedule.GetTrainingCoach(searchElement, dateTime);
            if (training != null) {
                return training.stringForCoach();
            }

            return "Таких записей нет.";
        }

        public string FindTrainingClient(string surname, string phoneNumber, DateTime dateTime) {
            string searchElement = new string(surname + '_' + phoneNumber);
            Schedule.Training training = Schedule.GetTrainingClient(searchElement, dateTime);
            if (training != null) {
                return training.stringForClient();
            }

            return "Таких записей нет.";
        }

        public string FindTrainingGym(int numberOfGym, DateTime dateTime) {
            Schedule.Training training = Schedule.GetTrainingGym(numberOfGym, dateTime);
            if (training != null) {
                return training.ToString();
            }

            return "Таких записей нет.";
        }

        public void saveDatabase(string file) {
            using (FileStream fs = new FileStream($"{file}.json", FileMode.Create)) {
                JsonSerializer.SerializeAsync<Database>(fs, this).Wait();
            }
        }

        public IDatabase setDatabase(string file) {
            return JsonFileReader.Read<Database>($"{file}.json");
        }
    }
}