using System;

namespace FitnessCenterConsole.Common {
    class WrongValueException : Exception {
        internal WrongValueException(string message)
        : base(message) { }
    }
}