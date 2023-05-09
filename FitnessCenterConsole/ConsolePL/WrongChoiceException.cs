using System;

namespace FitnessCenterConsole.ConsolePL {
    class WrongChoiceException : Exception {
        internal WrongChoiceException(string message)
        : base(message) { }
}
}
