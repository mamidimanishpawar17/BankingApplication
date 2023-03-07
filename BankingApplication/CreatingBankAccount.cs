using BankingApplication;
using System;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1
{
    public class CreatingBankAccount: UpdateAccountWithNumber
    {
        public void CreateBankAccount()
        {
            string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = BankApplication; Integrated Security = True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO BankAccountCreate (AccountNumber, FirstName, MiddleName, LastName, FatherName, MotherName, Ifsc, Email, Address, DateOfBirth, IsActive, AccountType, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES (@AccountNumber, @FirstName, @MiddleName, @LastName, @FatherName, @MotherName, @Ifsc, @Email, @Address, @DateOfBirth, @IsActive, @AccountType, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    Console.Write("Enter account number: ");
                    string accountNumber = Console.ReadLine();
                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    Console.Write("Enter first name: ");
                    string firstName = Console.ReadLine();
                    command.Parameters.AddWithValue("@FirstName", firstName);

                    Console.Write("Enter middle name (optional): ");
                    string middleName = Console.ReadLine();
                    command.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(middleName) ? DBNull.Value : (object)middleName);

                    Console.Write("Enter last name: ");
                    string lastName = Console.ReadLine();
                    command.Parameters.AddWithValue("@LastName", lastName);

                    Console.Write("Enter father's name: ");
                    string fatherName = Console.ReadLine();
                    command.Parameters.AddWithValue("@FatherName", fatherName);

                    Console.Write("Enter mother's name: ");
                    string motherName = Console.ReadLine();
                    command.Parameters.AddWithValue("@MotherName", motherName);

                    Console.Write("Enter IFSC code: ");
                    string ifsc = Console.ReadLine();
                    command.Parameters.AddWithValue("@Ifsc", ifsc);

                    Console.Write("Enter email: ");
                    string email = Console.ReadLine();
                    command.Parameters.AddWithValue("@Email", email);

                    Console.Write("Enter address: ");
                    string address = Console.ReadLine();
                    command.Parameters.AddWithValue("@Address", address);

                    Console.Write("Enter date of birth (yyyy-MM-dd): ");
                    string dobString = Console.ReadLine();
                    DateTime dob;
                    while (!DateTime.TryParse(dobString, out dob))
                    {
                        Console.Write("Invalid date format. Please enter date of birth (yyyy-MM-dd): ");
                        dobString = Console.ReadLine();
                    }
                    command.Parameters.AddWithValue("@DateOfBirth", dob);

                    Console.Write("Enter account active state (1 for active, 0 for inactive): ");
                    bool isActive;
                    while (!bool.TryParse(Console.ReadLine(), out isActive))
                    {
                        Console.Write("Invalid input. Please enter 1 for active or 0 for inactive: ");
                    }
                    command.Parameters.AddWithValue("@IsActive", isActive);

                    Console.Write("Enter account type:Current/Savings ");
                    string accountType = Console.ReadLine();
                    command.Parameters.AddWithValue("@AccountType", accountType);

                    command.Parameters.AddWithValue("@CreatedBy", "Admin");
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ModifiedBy", "Admin");
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Bank account created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create bank account.");
                    }
                }

                connection.Close();
            }

            Console.ReadLine();

        }
    }
}
