using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessCenterConsole.Entities {
    public class SearchElement {
        private string _surname;
        private string _phoneNumber;

        public SearchElement(string surname, string phoneNumber) {
            Surname = surname;
            PhoneNumber = phoneNumber;
        }

        public override string ToString() {
            return Surname;
        }
        public string Surname { get => _surname; set => _surname = value; }
        public string PhoneNumber { get => _phoneNumber; set => _phoneNumber = value; }
    }
}
