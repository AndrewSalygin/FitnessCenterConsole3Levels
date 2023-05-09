using System;
using System.Collections.Generic;
using FitnessCenterConsole.Entities;

namespace FitnessCenterConsole.DAL {
    interface IDatabase {
        public bool AddNewCoach(string surname, string name, string middleName, int experience,
            Education education, int age, DateTime birthday, string phoneNumber);
        public bool AddNewClient(string surname, string name, string middleName, 
            DateTime birthday, string phoneNumber);
        public bool AddNewGym(int numberOfGym, TypeOfGym typeOfGym);
        public bool AddClientToTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime);
        public bool DeleteClientFromTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime);
        public bool AddNewTraining(int gymKey, SearchElement coachKey, 
            HashSet<SearchElement> clientKeys, DateTime dateTime);
        public bool AddNewTraining(int gymKey, SearchElement coachKey, DateTime dateTime);
        public bool DeleteTraining(int gymKey, DateTime dateTime);
        public string GetInfoCoach(SearchElement searchElement);
        public string GetInfoGym(int numberOfGym);
        public string GetInfoClient(SearchElement searchElement);
        public bool DeleteCoach(SearchElement searchElement);
        public bool DeleteClient(SearchElement searchElement);
        public bool DeleteGym(int numberOfGym);
        public string FindTrainingCoach(SearchElement searchElement, DateTime dateTime);
        public string FindTrainingClient(SearchElement searchElement, DateTime dateTime);
        public string FindTrainingGym(int numberOfGym, DateTime dateTime);
    }
}
