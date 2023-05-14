using System;
using System.Collections;
using System.Collections.Generic;
using FitnessCenterConsole.DAL;
using FitnessCenterConsole.Entities;


namespace FitnessCenterConsole.BLL
{
    public class ProgramLogic : IProgramLogic
    {
        private IDatabase _database;

        public ProgramLogic(IDatabase database) {
            _database = database;
        }

        public IDatabase Database { get => _database; set => _database = value; }
        
        public bool AddNewCoach(string surname, string name, string middleName, int experience,
            Education education, DateTime birthday, string phoneNumber) {
            return Database.AddNewCoach(surname, name, middleName, experience, education, birthday, phoneNumber);
        }

        public bool AddNewClient(string surname, string name, string middleName, DateTime birthday, string phoneNumber) {
            return Database.AddNewClient(surname, name, middleName, birthday, phoneNumber);
        }

        public bool AddNewGym(int numberOfGym, TypeOfGym typeOfGym) {
            return Database.AddNewGym(numberOfGym, typeOfGym);
        }

        public bool AddClientToTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            return Database.AddClientToTraining(surnameClient, phoneNumberClient, surnameCoach,
                phoneNumberCoach, dateTime);
        }

        public bool DeleteClientFromTraining(string surnameClient, string phoneNumberClient,
            string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            return Database.DeleteClientFromTraining(surnameClient, phoneNumberClient, surnameCoach,
                phoneNumberCoach, dateTime);
        }

        public bool AddNewTraining(int gymKey, string surnameCoach, string phoneNumberCoach, HashSet<string> clientKeys,
            DateTime dateTime) {
            return Database.AddNewTraining(gymKey, surnameCoach, phoneNumberCoach, clientKeys, dateTime);
        }
        
        public bool AddNewTraining(int gymKey, string surnameCoach, string phoneNumberCoach, DateTime dateTime) {
            return Database.AddNewTraining(gymKey, surnameCoach, phoneNumberCoach, dateTime);
        }
        
        public bool AddNewTraining(int gymKey, DateTime dateTime) {
            return Database.AddNewTraining(gymKey, dateTime);
        }

        public bool DeleteTraining(int gymKey, DateTime dateTime) {
            return Database.DeleteTraining(gymKey, dateTime);
        }

        public string GetInfoCoach(string surname, string phoneNumber) {
            return Database.GetInfoCoach(surname, phoneNumber);
        }

        public string GetInfoGym (int numberOfGym) {
            return Database.GetInfoGym(numberOfGym);
        }

        public string GetInfoClient(string surname, string phoneNumber) {
            return Database.GetInfoClient(surname, phoneNumber);
        }

        public bool DeleteCoach(string surname, string phoneNumber) {
            return Database.DeleteCoach(surname, phoneNumber);
        }

        public bool DeleteClient(string surname, string phoneNumber) {
            return Database.DeleteClient(surname, phoneNumber);
        }

        public bool DeleteGym(int numberOfGym) {
            return Database.DeleteGym(numberOfGym);
        }

        public string FindTrainingCoach(string surname, string phoneNumber, DateTime dateTime) {
            return Database.FindTrainingCoach(surname, phoneNumber, dateTime);
        }

        public string FindTrainingClient(string surname, string phoneNumber, DateTime dateTime) {
            return Database.FindTrainingClient(surname, phoneNumber, dateTime);
        }

        public string FindTrainingGym(int numberOfGym, DateTime dateTime) {
            return Database.FindTrainingGym(numberOfGym, dateTime);
        }
        public void clearSchedule() {
            Database.clearSchedule();
        }

        public string FindCoachesBySurname(string surname) {
            return Database.FindCoachesBySurname(surname);
        }
        public string FindClientsBySurname(string surname) {
            return Database.FindClientsBySurname(surname);
        }
        public void saveDatabase(string file) {
            Database.saveDatabase(file);
        }
        public void setDatabase(string file) {
            Database.setDatabase(file);
        }
    }
}
