using FitnessCenterConsole.BLL;
using FitnessCenterConsole.ConsolePL;
using FitnessCenterConsole.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessCenterConsole.Common
{
    public static class DependencyResolver
    {
        static private IDatabase _database;
        static private IProgramLogic _programLogic;

        static public IDatabase Database {
            set => _database = value;
            get => _database ?? (_database = new Database());
        }
        static public IProgramLogic ProgramLogic {
            set => _programLogic = value;
            get => _programLogic ?? (_programLogic = new ProgramLogic(Database));
        }

        static public IProgramLogic setParameters(string file) {
            Database = Database.setDatabase(file);
            ProgramLogic = new ProgramLogic(Database);
            return ProgramLogic;
        }
    }
}