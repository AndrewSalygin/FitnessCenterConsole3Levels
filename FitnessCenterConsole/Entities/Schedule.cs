﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FitnessCenterConsole.Common;

namespace FitnessCenterConsole.Entities {
    public class Schedule {
        public class Training : IComparable<Training> {
            private DateTime _dateTime;
            private int _gymKey;
            private SearchElement _coachKey;
            private HashSet<SearchElement> _clientKeys;
            private int _countOfClients;

            [JsonConstructor]
            public Training(int gymKey, DateTime dateTime, SearchElement coachKey = null, HashSet<SearchElement> clientKeys = null) {
                DateTime = dateTime;
                GymKey = gymKey;
                if (coachKey == null) {
                    CoachKey = new SearchElement("отсутствует", "отсутствует");
                } else {
                    CoachKey = coachKey;
                }
                
                if (clientKeys != null) {
                    if (clientKeys.Count <= 0)
                        throw new WrongValueException("Ошибка: количество клиентов должно быть больше 0.");
                    else
                        CountOfClients = clientKeys.Count();
                    ClientKeys = clientKeys;
                } else {
                    ClientKeys = new HashSet<SearchElement>();
                }
            }

            public override string ToString() {
                string result = "\n--------------------\n" +
                                $"Дата: {DateTime.ToShortDateString()} \n" +
                                $"Время: {DateTime.ToLongTimeString()} \n" +
                                $"Тренер: {CoachKey}\n" +
                                $"Номер зала: {GymKey} \n" +
                                $"Количество клиентов: {CountOfClients} \n\n" +
                                $"Клиенты: \n";
                foreach (SearchElement client in ClientKeys) {
                    result += $"{client} \n";
                }
                result += "--------------------\n\n";
                return result;
            }

            public string stringForClient() {
                string result = "\n--------------------\n" +
                                $"Дата: {DateTime.ToShortDateString()} \n" +
                                $"Время: {DateTime.ToLongTimeString()} \n" +
                                $"Номер зала: {GymKey} \n" +
                                $"Тренер: {CoachKey} \n";
                result += "--------------------\n\n";
                return result;
            }

            public string stringForCoach() {
                string result = "\n--------------------\n" +
                                $"Дата: {DateTime.ToShortDateString()} \n" +
                                $"Время: {DateTime.ToLongTimeString()} \n" +
                                $"Номер зала: {GymKey} \n" +
                                $"Количество клиентов: {CountOfClients} \n\n" +
                                $"Клиенты: \n";
                foreach (SearchElement client in ClientKeys) {
                    result += $"{client} \n";
                }
                result += "--------------------\n\n";
                return result;
            }

            public int CompareTo(Training training) {
                return DateTime.CompareTo(training.DateTime);
            }

            public DateTime DateTime { get => _dateTime; set => _dateTime = value; }
            public int GymKey { get => _gymKey; set => _gymKey = value; }
            public SearchElement CoachKey { get => _coachKey; set => _coachKey = value; }
            public HashSet<SearchElement> ClientKeys { get => _clientKeys; set => _clientKeys = value; }
            public int CountOfClients { get => _countOfClients; set => _countOfClients = value; }
        }

        private SortedSet<Training> _trainings;

        public SortedSet<Training> Trainings { get => _trainings; set => _trainings = value; }

        internal Schedule() {
            Trainings = new SortedSet<Training>();
        }

        [JsonConstructor]
        public Schedule(SortedSet<Training> trainings) {

            Trainings = trainings;
        }

        public override string ToString() {
            string result = "\nРасписание занятий:\n\n";

            int count = 1;
            foreach (Training elem in Trainings) {
                result += $"Запись номер {count}: \n" + elem.ToString();
                count++;
            }
            return result;
        }

        public string stringScheduleClient() {
            string result = "\nРасписание занятий:\n\n";

            int count = 1;
            foreach (Training elem in Trainings) {
                result += $"Запись номер {count}: \n" + elem.stringForClient();
                count++;
            }
            return result;
        }

        public string stringScheduleCoach() {
            string result = "\nРасписание занятий:\n\n";

            int count = 1;
            foreach (Training elem in Trainings) {
                result += $"Запись номер {count}: \n" + elem.stringForCoach();
                count++;
            }
            return result;
        }

        // вывод исключений
        internal static void PrintToApp(string text) {
            Console.WriteLine(text);
        }
        
        internal bool AddTraining(int gymKey, SearchElement coachKey, HashSet<SearchElement> clientKeys, DateTime dateTime) {
            Training temp;
            DateTime largeBorderTime = dateTime.AddHours(1);
            DateTime smallerBorderTime;
            try {
                smallerBorderTime = dateTime.AddHours(-1);
            } catch (ArgumentOutOfRangeException ex) {
                PrintToApp("Ошибка: указано невозможное время.");
                return false;
            }
            
            // проверка условий для новой тренировки
            try {
                foreach (Training training in Trainings) {
                    if (smallerBorderTime < training.DateTime && training.DateTime < largeBorderTime) {
                        if (training.CoachKey == coachKey) {
                            throw new WrongValueException($"Ошибка: тренер {coachKey} уже проводит тренеровку в данное время.");
                        }
                        if (gymKey == training.GymKey) {
                            throw new WrongValueException($"Ошибка: зал номер {gymKey} уже занят в данное время.");
                        }
                        foreach (SearchElement client in clientKeys) {
                            if (training.ClientKeys.Contains(client)) {
                                throw new WrongValueException($"Ошибка: клиент {client} уже записан на другую тренировку в это время.");
                            }
                        }
                    }
                }
                temp = new Training(gymKey, dateTime, coachKey, clientKeys);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            Trainings.Add(temp);
            return true;
        }

        internal bool AddTraining(int gymKey, SearchElement coachKey, DateTime dateTime) {
            Training temp;
            DateTime largeBorderTime = dateTime.AddHours(1);
            DateTime smallerBorderTime;
            try {
                smallerBorderTime = dateTime.AddHours(-1);
            } catch (ArgumentOutOfRangeException ex) {
                PrintToApp("Ошибка: указано невозможное время.");
                return false;
            }

            // проверка условий для новой тренировки
            try {
                foreach (Training training in Trainings) {
                    if (smallerBorderTime < training.DateTime && training.DateTime < largeBorderTime) {
                        if (training.CoachKey == coachKey) {
                            throw new WrongValueException($"Ошибка: тренер {coachKey} уже проводит тренеровку в данное время.");
                        }
                        if (gymKey == training.GymKey) {
                            throw new WrongValueException($"Ошибка: зал номер {gymKey} уже занят в данное время.");
                        }
                    }
                }
                temp = new Training(gymKey, dateTime, coachKey);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            Trainings.Add(temp);
            return true;
        }

        internal bool AddTraining(int gymKey, DateTime dateTime) {
            Training temp;
            DateTime largeBorderTime = dateTime.AddHours(1);
            DateTime smallerBorderTime;
            try {
                smallerBorderTime = dateTime.AddHours(-1);
            } catch (ArgumentOutOfRangeException ex) {
                PrintToApp("Ошибка: указано невозможное время.");
                return false;
            }

            // проверка условий для новой тренировки
            try {
                foreach (Training training in Trainings) {
                    if (smallerBorderTime < training.DateTime && training.DateTime < largeBorderTime) {
                        if (gymKey == training.GymKey) {
                            throw new WrongValueException($"Ошибка: зал номер {gymKey} уже занят в данное время.");
                        }
                    }
                }
                temp = new Training(gymKey, dateTime);
            } catch (WrongValueException ex) {
                PrintToApp(ex.Message);
                return false;
            }

            Trainings.Add(temp);
            return true;
        }

        internal bool DeleteTraining(int gymKey, DateTime dateTime) {
            Training temp = GetTrainingGym(gymKey, dateTime);
            if (temp != null) {
                Trainings.Remove(temp);
            } else {
                throw new WrongValueException($"Ошибка: тренировки в зале номер {gymKey} в это время не найдено в базе.");
            }
            return true;
        }

        internal Training GetTrainingCoach(SearchElement searchElement, DateTime dateTime) {
            return Trainings.FirstOrDefault(x => x.CoachKey.Equals(searchElement) &&
                                            x.DateTime <= dateTime &&
                                            x.DateTime.AddHours(1) >= dateTime);
        }

        internal Training GetTrainingClient(SearchElement searchElement, DateTime dateTime) {
            foreach (Training training in Trainings) {
                foreach (SearchElement client in training.ClientKeys) {
                    if (searchElement == client &&
                        dateTime >= training.DateTime &&
                        dateTime <= training.DateTime.AddHours(1)) {
                        return training;
                    }
                }
            }
            return null;
        }

        internal Training GetTrainingGym(int number, DateTime dateTime) {
            return Trainings.FirstOrDefault(x => x.GymKey.Equals(number) &&
                                            x.DateTime <= dateTime &&
                                            x.DateTime.AddHours(1) >= dateTime);
        }


        // реализация просмотра интересущих нас занятий для каждого типа пользователя

        // для клиента
        internal SortedSet<Training> GetInfo(Client client) {
            if (client != null) {
                SortedSet<Training> trainings = new SortedSet<Training>();
                foreach (Training training in Trainings) {
                    if (training.ClientKeys.Contains(new SearchElement(client.Surname, client.PhoneNumber))) {
                        trainings.Add(training);
                    }
                }
                return trainings;
            }
            return null;
        }

        // для администратора
        internal SortedSet<Training> GetInfo(Gym gym) {
            if (gym != null) {
                SortedSet<Training> trainings = new SortedSet<Training>();
                foreach (Training training in Trainings) {
                    if (training.GymKey == gym.NumberOfGym) {
                        trainings.Add(training);
                    }
                }
                return trainings;
            }
            return null;
        }

        // для тренера
        internal SortedSet<Training> GetInfo(Coach coach) {
            if (coach != null) {
                SortedSet<Training> trainings = new SortedSet<Training>();
                foreach (Training training in Trainings) {
                    if (training.CoachKey == new SearchElement(coach.Surname, coach.PhoneNumber)) {
                        trainings.Add(training);
                    }
                }
                return trainings;
            }
            return null;
        }
    }
}
