using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BankingApplication
{

    public class AddAccountDetails : CreatingBankAccount
    {
        const string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = BankApplication; Integrated Security = True";

        public void UserAccountDetails()
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

            Console.Write("Modified Date (MM/DD/YYYY): ");
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

            using (SqlConnection connection = new SqlConnection(connectionString))
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
            //mail to the user
            SendEmail(accountNumber, accountBalance, minimumAccountBalance, isActive, modifiedBy, modifiedDate);

            Console.ReadLine();
            
        }
        private decimal GetMinimumBalance(long accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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




        public void GetAccountDetails(long accountNumber)
        {
            Console.Write("Enter account number: ");
            accountNumber = long.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAccountByNumber", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@AccountNumber", SqlDbType.BigInt).Value = accountNumber;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"AccountNumber: {reader["AccountNumber"]}");
                            Console.WriteLine($"MinimumAccountBalance : {reader["MinimumAccountBalance"]}");
                            Console.WriteLine($"Account Balance: {reader["AccountBalance"]}");
                        }
                    }
                }

                Console.ReadLine();

            }
        }



        private void SendEmail(long accountNumber, decimal accountBalance, decimal minimumAccountBalance, bool isActive, string modifiedBy, DateTime? modifiedDate)
        {
            Console.Write("Enter your email address: ");
            string emailAddress = Console.ReadLine();
            string from = "mamidiman@gmail.com";
            string subject = "Account Details";
            string body = GetAccountDetailsHtml(accountNumber, accountBalance, minimumAccountBalance, isActive, modifiedBy, modifiedDate);

            using (MailMessage mail = new MailMessage(from, emailAddress))
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential("mamidiman@gmail.com", "lenceulvappkrkze");
                    //smtp.Send(mail);
                    try
                    {
                        smtp.Send(mail);
                        Console.WriteLine("Account details sent to email.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending email: " + ex.Message);
                    }
                }
            }

            Console.WriteLine("Account details sent to email.");
        }

        private string GetAccountDetailsHtml(long accountNumber, decimal accountBalance, decimal minimumAccountBalance, bool isActive, string modifiedBy, DateTime? modifiedDate)
        {
            string html = "<table><tbody>" +
                $"<tr><td>Account Number:</td><td>{accountNumber}</td></tr>" +
                $"<tr><td>Account Balance:</td><td>{accountBalance}</td></tr>" +
                $"<tr><td>Minimum Account Balance:</td><td>{minimumAccountBalance}</td></tr>" +
                $"<tr><td>Is Active:</td><td>{isActive}</td></tr>" +
                $"<tr><td>Modified By:</td><td>{modifiedBy}</td></tr>" +
                $"<tr><td>Modified Date:</td><td>{modifiedDate}</td></tr>" +
                "</tbody></table>";

            return html;
        }
    }
}

