﻿using System;
using System.Collections.Generic;
using FitnessCenterConsole.Entities;

namespace FitnessCenterConsole.DAL {
    interface IDatabase {
        public bool AddNewCoach(string surname, string name, string middleName, int experience,
            Education education, DateTime birthday, string phoneNumber);
        public bool AddNewClient(string surname, string name, string middleName, 
            DateTime birthday, string phoneNumber);
        public bool AddNewGym(int numberOfGym, TypeOfGym typeOfGym);
        public bool AddClientToTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime);
        public bool DeleteClientFromTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime);
        public bool AddNewTraining(int gymKey, string surnameCoach, string phoneNumberCoach, 
            HashSet<Tuple<string, string>> clientKeys, DateTime dateTime);
        public bool AddNewTraining(int gymKey, string surnameCoach, 
            string phoneNumberCoach, DateTime dateTime);
        public bool DeleteTraining(int gymKey, DateTime dateTime);
        public string GetInfoCoach(string surname, string phoneNumber);
        public string GetInfoGym(int numberOfGym);
        public string GetInfoClient(string surname, string phoneNumber);
        public bool DeleteCoach(string surname, string phoneNumber);
        public bool DeleteClient(string surname, string phoneNumber);
        public bool DeleteGym(int numberOfGym);
        public string FindTrainingCoach(string surname, string phoneNumber, DateTime dateTime);
        public string FindTrainingClient(string surname, string phoneNumber, DateTime dateTime);
        public string FindTrainingGym(int numberOfGym, DateTime dateTime);
    }
}