using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FitnessCenterConsole.Entities;
using FitnessCenterConsole.DAL;
using Newtonsoft.Json;
using System.ComponentModel;
using Nancy.Json.Converters;

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
            database.AddNewClient("Иванов", "Михаил", "Сергеевич", new DateTime(1997, 2, 23), "70000000000");
            database.AddNewClient("Борисова", "Елизавета", "Егоровна", new DateTime(1996, 3, 22), "74444444444");
            database.AddNewClient("Сергеев", "Алексей", "Маркович", new DateTime(1999, 6, 25), "74444444441");
            database.AddNewClient("Черных", "Александр", "Данилович", new DateTime(1995, 2, 6), "74444444442");
            database.AddNewClient("Орлова", "Мария", "Руслановна", new DateTime(1999, 2, 7), "74444444443");
            database.AddNewClient("Борисов", "Евгений", "Ильич", new DateTime(1999, 9, 27), "74444444445");
            database.AddNewClient("Борисов", "Евгений", "Ильич", new DateTime(1999, 9, 27), "74444444445");

            database.AddNewCoach("Кулаков", "Богдан", "Ярославович", 3, Education.Higher, new DateTime(2002, 6, 25), "74444444448");
            database.AddNewCoach("Воронцова", "Ксения", "Владимировна", 5, Education.Higher, new DateTime(1998, 3, 24), "74444444449");
            database.AddNewCoach("Быкова", "Евгения", "Георгиевна", 2, Education.College, new DateTime(1999, 9, 20), "74444444410");
            database.AddNewCoach("Колосов", "Сергей", "Георгиевич", 2, Education.College, new DateTime(1999, 8, 11), "74444444411");
        
            database.AddNewGym(10, TypeOfGym.Common);

            database.AddNewGym(11, TypeOfGym.Strength);
            database.AddNewGym(11, TypeOfGym.Gymnastics);

            database.AddNewGym(12, TypeOfGym.Сardio);

            database.DeleteClient("Орлова", "74444444443");
            database.DeleteClient("Иванов", "75555555555");
            database.DeleteCoach("Колосов", "74444444411");

            database.DeleteGym(10);

            DateTime time1 = DateTime.Now.AddDays(1);
            database.AddNewTraining(11, "Кулаков", "74444444448", time1);
            database.AddNewTraining(12, "Кулаков", "74444444448", time1.AddMinutes(20));
            
            database.AddClientToTraining("Черных", "74444444442", "Кулаков", "74444444448", time1);
            database.AddClientToTraining("Черныхxxxx", "74444444442", "Кулаков", "74444444448", time1);
            database.AddClientToTraining("Сергеев", "74444444441", "Кулаков", "74444444448", time1);
            database.AddClientToTraining("Борисова", "74444444444", "Кулаков", "74444444448", time1);
            

            HashSet<string> clients = new HashSet<string>();
            clients.Add(new string("Борисова_74444444444"));
            clients.Add(new string("Борисова222_74444444444"));
            clients.Add(new string("Черных_74444444442"));

            DateTime time2 = DateTime.Now.AddDays(1).AddMinutes(20);

            database.AddNewTraining(12, "Воронцова", "74444444449", clients, time2);
            clients.Remove("Борисова222_74444444444");
            database.AddNewTraining(12, "Воронцова", "74444444449", clients, time2);
            clients.Remove(new string("Борисова_74444444444"));            
            database.AddNewTraining(12, "Воронцова", "74444444449", clients, time2);
            database.AddNewTraining(12, "Воронцова", "74444444449", time2);

            Console.WriteLine(database.GetInfoCoach("Кулаков", "74444444448"));
            Console.WriteLine(database.GetInfoClient("Черных", "74444444442"));
            Console.WriteLine(database.FindTrainingClient("Черных", "74444444442", time1));
            database.DeleteTraining(12, time2);
            database.DeleteClientFromTraining("Черных", "74444444442", "Кулаков", "74444444448", time1);
            Console.WriteLine(database.FindTrainingClient("Черных", "74444444443", time1));

            // сохранение файла
            using (FileStream fs = new FileStream("database_new.json", FileMode.Create)) {
                System.Text.Json.JsonSerializer.SerializeAsync<Database>(fs, database).Wait();
            }

        }
    }
}