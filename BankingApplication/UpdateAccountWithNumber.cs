using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace BankingApplication
{
    public class UpdateAccountWithNumber
    {
        //string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = BankApplication; Integrated Security = True";

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;


        public UpdateAccountWithNumber(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public void UserAccountDetails(int accountnumber)
        {
            Console.WriteLine("Enter the following details:");
            Console.Write("Account Number: ");
            long accountNumber = Convert.ToInt64(Console.ReadLine());

            Console.Write("Account Balance: ");
            decimal accountBalance = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Minimum Account Balance: ");
            decimal minimumAccountBalance = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Is Active? (True/False): ");
            bool isActive = Convert.ToBoolean(Console.ReadLine());

            Console.Write("Modified By: ");
            string modifiedBy = Console.ReadLine();

            Console.Write("Modified Date (DD/MM/YYYY): ");
            DateTime? modifiedDate = null;
            string modifiedDateString = Console.ReadLine();
            if (!string.IsNullOrEmpty(modifiedDateString))
            {
                modifiedDate = Convert.ToDateTime(modifiedDateString);
            }

            if (minimumAccountBalance < 10000)
            {
                Console.WriteLine($"Minimum account balance is less than 10000. Please add {10000 - minimumAccountBalance} to the account within 1 minute.");
                System.Threading.Thread.Sleep(60000); // Wait for 1 minute

                // Check if minimum balance has been added
                decimal updatedMinimumBalance = GetMinimumBalance(accountNumber);
                if (updatedMinimumBalance < 10000)
                {
                    Console.WriteLine($"Failed to add minimum balance within 1 minute. Account will be inactive.");
                    isActive = false;
                }
                else
                {
                    minimumAccountBalance = updatedMinimumBalance;
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO AccountInformation (AccountNumber, AccountBalance, MinimumAccountBalance, IsActive, ModifiedBy, ModifiedDate) VALUES (@AccountNumber, @AccountBalance, @MinimumAccountBalance, @IsActive, @ModifiedBy, @ModifiedDate)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    command.Parameters.AddWithValue("@AccountBalance", accountBalance);
                    command.Parameters.AddWithValue("@MinimumAccountBalance", minimumAccountBalance);
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                    command.Parameters.AddWithValue("@ModifiedDate", modifiedDate);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Account information created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create account information.");
                    }
                }

                connection.Close();
            }

            Console.ReadLine();
        }
        private decimal GetMinimumBalance(long accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT MinimumAccountBalance FROM AccountInformation WHERE AccountNumber = @AccountNumber";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        decimal minimumBalance = Convert.ToDecimal(reader["MinimumAccountBalance"]);
                        return minimumBalance;
                    }
                    else
                    {
                        Console.WriteLine("Account not found.");
                        return 0;
                    }
                }
            }
        }
    }
}
