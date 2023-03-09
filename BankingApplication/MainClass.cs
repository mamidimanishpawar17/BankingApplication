using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication
{
    class MainClass : AddAccountDetails
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Banking Application!");
            int i = 0;
            while (i<4)
            {
                Console.WriteLine("\nPlease select an option:");
                Console.WriteLine("1. Create a new bank account");
                Console.WriteLine("2. Update an existing bank account");
                Console.WriteLine("3. Add account details");
                Console.WriteLine("4. Generate Account details by Account Number");

                int option = Convert.ToInt16(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        MainClass main = new MainClass();
                        main.CreateBankAccount();
                        //main.UserAccountDetails();
                        break;

                    case 2:
                        UpdateAccountWithNumber updateAccount = new UpdateAccountWithNumber();
                        Console.Write("Enter the account number you want to update: ");
                        long accountNumber = Convert.ToInt64(Console.ReadLine());
                        updateAccount.UserAccountDetails((int)accountNumber);
                        break;
                    case 3:
                        AddAccountDetails addAccountDetails = new AddAccountDetails();
                        Console.Write("Update the Account details of the required user");
                        addAccountDetails.UserAccountDetails();
                        break;
                    case 4:
                        AddAccountDetails getdetails= new AddAccountDetails();
                        long accountdetals = Convert.ToInt64(Console.ReadLine());
                        getdetails.GetAccountDetails((int)accountdetals);
                        break;

                    default:
                        Console.WriteLine("Invalid option selected. Please try again.");
                        break;
                }
                i++;
            }
        }
    }

}
