using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FitnessCenterConsole.Entities;
using FitnessCenterConsole.DAL;

namespace FitnessCenterConsole.ConsolePL {
    class Program {
        static void Main() {
            Console.WriteLine("1.Создать новую базу\n2.Открыть существующую базу");
            int choice = Int32.Parse(Console.ReadLine());
            Database database = null;

            try {
                switch (choice) {
                    case 1:
                        database = new Database();
                        break;
                    case 2:
                        Console.Write("Введите имя файла: ");
                        string file = Console.ReadLine();
                        try {
                            database = JsonFileReader.Read<Database>($"{file}.json");
                        } catch (FileNotFoundException ex) {
                            Console.WriteLine("Ошибка: Файл не найден.");
                            return;
                        }
                        break;
                    default:
                        throw new WrongChoiceException("Ошибка: Неверный ввод.");
                }
            } catch (WrongChoiceException ex) {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return;
            }

            database.AddNewClient("Иванов", "Михаил", "Сергеевич", new DateTime(1997, 2, 23), "75555555555");
        /*    database.AddNewClient("Борисова", "Елизавета", "Егоровна", 21, new DateTime(1996, 3, 22), "74444444444");
            database.AddNewClient("Сергеев", "Алексей", "Маркович", 25);
            database.AddNewClient("Черных", "Александр", "Данилович", 30);
            database.AddNewClient("Орлова", "Мария", "Руслановна", -27);
            database.AddNewClient("Борисов", "Евгений", "Ильич", 0);
            database.AddNewClient("Борисов", "Евгений", "Ильич", 20);
            database.AddNewClient("Борисов", "Евгений", "Ильич", 20);

            database.AddNewCoach("Кулаков", "Богдан", "Ярославович", 3, Education.Higher, 28);
            database.AddNewCoach("Воронцова", "Ксения", "Владимировна", 5, Education.Higher, 30);
            database.AddNewCoach("Быкова", "Евгения", "Георгиевна", 2, Education.College, 24);
            database.AddNewCoach("Колосов", "Сергей", "Георгиевич", 2, Education.College, 24);
        */
            database.AddNewGym(10, TypeOfGym.Common);

            database.AddNewGym(11, TypeOfGym.Strength);
            database.AddNewGym(11, TypeOfGym.Gymnastics);

            database.AddNewGym(12, TypeOfGym.Сardio);

            database.DeleteClient("Иванов");
            database.DeleteClient("Иванов");
            database.DeleteCoach("Колосов");

            database.DeleteGym(10);

            DateTime time1 = DateTime.Now.AddDays(1);
            database.AddNewTraining(11, "Кулаков", time1);
            database.AddNewTraining(12, "Кулаков", time1.AddMinutes(20));
            /*
            database.AddClientToTraining("Черных", "Кулаков", time1);
            database.AddClientToTraining("Черныхxxxx", "Кулаков", time1);
            database.AddClientToTraining("Сергеев", "Кулаков", time1);
            database.AddClientToTraining("Борисова", "Кулаков", time1);
            */

            HashSet<string> clients = new HashSet<string>();
            clients.Add("Борисова");
            clients.Add("Борисова222");

            DateTime time2 = DateTime.Now.AddDays(1).AddMinutes(20);

            database.AddNewTraining(12, "Воронцова", clients, time2);
            clients.Remove("Борисова222");
            database.AddNewTraining(12, "Воронцова", clients, time2);
            clients.Remove("Борисова");            
            database.AddNewTraining(12, "Воронцова", clients, time2);
            database.AddNewTraining(12, "Воронцова", time2);

            Console.WriteLine(database.GetInfoCoach("Кулаков", true));
            Console.WriteLine(database.FindTrainingClient("Черных", time1));
            database.DeleteTraining(12, time2);
            database.DeleteClientFromTraining("Черных", "Кулаков", time1);
            Console.WriteLine(database.FindTrainingClient("Черных", time1));

            // сохранение файла
            using (FileStream fs = new FileStream("database_new.json", FileMode.Create)) {
                    JsonSerializer.SerializeAsync<Database>(fs, database);
            }
        }
    }
}