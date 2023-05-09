using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;
using FitnessCenterConsole.Entities;

namespace FitnessCenterConsole.DAL {
    internal class Database : IDatabase {
        private Dictionary<SearchElement, Coach> _coaches;
        private Dictionary<SearchElement, Client> _clients;
        private Dictionary<int, Gym> _gyms;
        private Schedule _schedule;

        public Dictionary<SearchElement, Coach> Coaches { get => _coaches; set => _coaches = value; }
        public Dictionary<SearchElement, Client> Clients { get => _clients; set => _clients = value; }
        public Dictionary<int, Gym> Gyms { get => _gyms; set => _gyms = value; }
        public Schedule Schedule { get => _schedule; set => _schedule = value; }

        // конструктор для новой базы
        internal Database() {
            _coaches = new Dictionary<SearchElement, Coach>();
            _clients = new Dictionary<SearchElement, Client>();
            _gyms = new Dictionary<int, Gym>();
            _schedule = new Schedule();
        }

        [JsonConstructor]
        // конструктор для json
        public Database(Dictionary<SearchElement, Coach> coaches, Dictionary<SearchElement, Client> clients, Dictionary<int, Gym> gyms, Schedule schedule) {
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
            Education education, int age, DateTime birthday, string phoneNumber) {
            Coach temp;
            try {
                temp = new Coach(Coaches.Count, surname, name, middleName, experience, education, birthday, phoneNumber);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
            try {
                Coaches.Add(new SearchElement(surname, phoneNumber), temp);
            } catch (ArgumentException ex) {
                PrintToApp($"Ошибка: добавление тренера {surname} с уже существующей фамилией в базе.");
                return false;
            }
            return true;
        }

        public bool AddNewClient(string surname, string name, string middleName, DateTime birthday, string phoneNumber) {
            Client temp;
            try {
                temp = new Client(Clients.Count, surname, name, middleName, birthday, phoneNumber);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
            try {
                Clients.Add(new SearchElement(surname, phoneNumber), temp);
            } catch (ArgumentException ex) {
                PrintToApp($"Ошибка: добавление клиента {surname} с уже существующей фамилией в базе.");
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
            SearchElement searchElementCoach = new SearchElement(surnameCoach, phoneNumberCoach);
            Schedule.Training training = Schedule.GetTrainingCoach(searchElementCoach, dateTime);
            SearchElement searchElementClient = new SearchElement(surnameClient, phoneNumberClient);
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
                                if (localTraining.ClientKeys.Contains(searchElementClient) && localTraining.DateTime > smallerBorderTime && largeBorderTime > localTraining.DateTime) {
                                    throw new WrongValueException($"Ошибка: клиент {surnameClient} уже записан на другое занятие в это время.");
                                }
                            }
                        }
                        training.ClientKeys.Add(searchElementClient);
                        training.CountOfClients++;
                        return true;
                    }
                    else {
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
            SearchElement searchElementCoach = new SearchElement(surnameCoach, phoneNumberCoach);
            Schedule.Training training = Schedule.GetTrainingCoach(searchElementCoach, dateTime);
            SearchElement searchElementClient = new SearchElement(surnameClient, phoneNumberClient);
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
        public bool AddNewTraining(int gymKey, SearchElement coachKey, HashSet<SearchElement> clientKeys, DateTime dateTime) {
            try {
                // если указанный зал существует
                if (GetGymByNumber(gymKey) != null) {
                    // если указанный тренер существует
                    if (GetCoachBySearchElement(coachKey) != null) {
                        // если все клиенты в базе существуют
                        foreach (SearchElement client in clientKeys) {
                            if (GetClientBySearchElement(client) == null) {
                                throw new WrongValueException($"Ошибка: клиент {client} не найден в базе.");
                            }
                        }
                        return Schedule.AddTraining(gymKey, coachKey, clientKeys, dateTime);
                    }
                    throw new WrongValueException($"Ошибка: тренер {coachKey} в базе не найден.");
                }
                throw new WrongValueException($"Ошибка: зал номер {gymKey} в базе не найден.");                
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }
        }

        public bool AddNewTraining(int gymKey, SearchElement coachKey, DateTime dateTime) {
            try {
                if (GetGymByNumber(gymKey) != null) {
                    if (GetCoachBySearchElement(coachKey) != null) {
                        return Schedule.AddTraining(gymKey, coachKey, dateTime);
                    }
                    throw new WrongValueException($"Ошибка: тренер {coachKey} в базе не найден.");
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

        // вывод полной информации о сущностях с актуальным \ архивным расписанием 
        public string GetInfoCoach (SearchElement searchElement) {
            Coach temp = GetCoachBySearchElement(searchElement);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null ? "\nНайдено:" +
                   "\n--------------------\n" +
                   temp.ToString() +
                   localSchedule.stringScheduleCoach() : $"Ошибка: тренера {searchElement} не найдено в базе.";
        }

        public string GetInfoGym (int numberOfGym) {
            Gym temp = GetGymByNumber(numberOfGym);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null ? "\nНайдено:" +
                   "\n--------------------\n" +
                   temp.ToString() + 
                   localSchedule.ToString() : $"Ошибка: зала номер {numberOfGym} не найдено в базе.";
        }

        public string GetInfoClient (SearchElement searchElement) {
            Client temp = GetClientBySearchElement(searchElement);
            Schedule localSchedule = new Schedule(Schedule.GetInfo(temp));
            return temp != null ? "\nНайдено:" +
                   "\n--------------------\n" +
                   temp.ToString() +
                   localSchedule.stringScheduleClient() : $"Ошибка: клиента {searchElement.Surname} не найдено в базе.";
        }

        // получение экземпляра класса
        private Client GetClientBySearchElement(SearchElement searchElement) {
            // получает null, если не найден
            return Clients.GetValueOrDefault(searchElement);
        }
        private Coach GetCoachBySearchElement(SearchElement searchElement) {
            return Coaches.GetValueOrDefault(searchElement);
        }
        private Gym GetGymByNumber(int number) {
            return Gyms.GetValueOrDefault(number);
        }

        // удаление сущностей
        public bool DeleteCoach(SearchElement searchElement) {
           try { 
                if (!Coaches.Remove(searchElement)) {
                    throw new WrongValueException($"Ошибка: тренера {searchElement.Surname} не найдено в базе.");
                } 
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            return true;
        }

        public bool DeleteClient(SearchElement searchElement) {
            try { 
                if (!Clients.Remove(searchElement)) {
                    throw new WrongValueException($"Ошибка: клиента {searchElement.Surname} не найдено в базе.");
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
        public string FindTrainingCoach(SearchElement searchElement, DateTime dateTime) {
            Schedule.Training training = Schedule.GetTrainingCoach(searchElement, dateTime);
            if (training != null) {
                return training.stringForCoach();                    
            }
            return "Таких записей нет.";
        }
        public string FindTrainingClient(SearchElement searchElement, DateTime dateTime) {
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
    }
}