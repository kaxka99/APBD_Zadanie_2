using LegacyApp.Core.Interfaces;
using LegacyApp.Core.Validators.Users;
using LegacyApp.Core.DAL;
using LegacyApp.Core.DAL.Repositories;
using LegacyApp.Models;
using System;


namespace LegacyApp.Core.DAL.Services
{

    

    public class UserService
    {
        private IInputValidator _inputValidator;
        public UserService()
        {
            _inputValidator = new InputValidator();
        }
        
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            //SRP
            if (!_inputValidator.ValidateName(firstName, lastName)) 
            {
                return false;
            }
            if (!_inputValidator.ValidateEmail(email))
            {
                return false;
            }
            if (!_inputValidator.ValidateAge(dateOfBirth))
            {
                return false;
            }



            //DIP
            var clientRepository = new ClientRepository();
            var client = clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };

            //DIP OCP
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else if (client.Type == "ImportantClient")
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    user.CreditLimit = creditLimit;
                }
            }
            else
            {
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                }
            }

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }
    }
}
