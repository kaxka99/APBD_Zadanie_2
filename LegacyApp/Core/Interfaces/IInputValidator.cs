using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Core.Interfaces
{
    public interface IInputValidator
    {
        bool ValidateName(string firstName, string lastName);
        bool ValidateEmail(string email);

        bool ValidateAge(DateTime dateOfBirth);
    }
}
