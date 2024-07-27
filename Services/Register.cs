using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;

namespace MyCommonStructure.Services
{
    public class register
    {
        dbServices ds = new dbServices();

        public async Task<responseData> Registration(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Role", req.addInfo["Role"].ToString()),
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@Address", req.addInfo["Address"].ToString()),
                    new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE Role=@Role AND Email=@Email AND Phone=@Phone;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Already registered, Try Login in!!";
                }
                else
                {
                    var insertSql = @"INSERT INTO pc_student.TEDrones_Users (Role, UserName, Email, Phone, Address, UserPassword) 
                                      VALUES(@Role, @UserName, @Email, @Phone, @Address, @UserPassword);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);
                    if (insertId != 0)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        await SendOTPToEmail(req.addInfo["Email"].ToString(), req.addInfo["UserName"].ToString());
                        resData.rData["rMessage"] = "Registered successfully!";
                        

                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occurred while registration!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
        private async Task SendOTPToEmail(string email, string userName)
        // private async Task SendOTPToEmail(string email, string emailOtp)
        {

            Console.WriteLine($"Sending OTP {email}");
            try
            {
                var fromAddress = new MailAddress("jeevank028@gmail.com");
                var toAddress = new MailAddress(email);
                const string fromPassword = "dznk ezxs tfbc wfxb";
                const string subject = "";
                // String userName = "TravelMates";


                // string body = $"Your OTP code is {emailOtp}. This code is valid for 5 minutes.";
                string body = $@"
                Dear {userName},

                Thank you for using TravelMates.

                Your One-Time Password (OTP) is . This code is valid for 5 minutes. Please enter it to complete your verification process.

                If you did not request this code, please ignore this message.

                Thank you for your attention.

                Best regards,  
                The TravelMates Team
                ";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    await smtp.SendMailAsync(message);
                }

                Console.WriteLine("OTP email sent successfully to " + email);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("SMTP Exception: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + smtpEx.InnerException.Message);
                }
                throw;  // Re-throw the exception after logging it
            }
            catch (SocketException socketEx)
            {
                Console.WriteLine("Socket Exception: " + socketEx.Message);
                throw;  // Re-throw the exception after logging it
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                throw;  // Re-throw the exception after logging it
            }
        }

        public async Task<responseData> GetUserByEmail(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.rStatus = 200;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "User Details Retrieved Successfully";
            try
            {
                string input = req.addInfo["Email"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", input)
                };

                var sql = "SELECT * FROM pc_student.TEDrones_Users WHERE Email=@Email;";
                var data = ds.ExecuteSQLName(sql, myParams);

                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "User not found";
                }
                else
                {
                    resData.rData["UserId"] = data[0][0]["UserId"];
                    resData.rData["UserName"] = data[0][0]["UserName"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["Phone"] = data[0][0]["Phone"];
                    resData.rData["Address"] = data[0][0]["Address"];
                    resData.rData["UserPassword"] = data[0][0]["UserPassword"];
                    resData.rData["ProfilePic"] = data[0][0]["ProfilePic"];
                    resData.rData["Role"] = data[0][0]["Role"];
                    resData.rData["RegistrationDate"] = data[0][0]["RegistrationDate"];
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
            }
            return resData;
        }
    }
}