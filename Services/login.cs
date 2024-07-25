using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class login
    {
        dbServices ds = new dbServices();
        decryptService cm = new decryptService();
        private readonly Dictionary<string, string> jwt_config = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _service_config = new Dictionary<string, string>();
        IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public login()
        {
            jwt_config["Key"] = appsettings["jwt_config:Key"].ToString();
            jwt_config["Issuer"] = appsettings["jwt_config:Issuer"].ToString();
            jwt_config["Audience"] = appsettings["jwt_config:Audience"].ToString();
            jwt_config["Subject"] = appsettings["jwt_config:Subject"].ToString();
            jwt_config["ExpiryDuration_app"] = appsettings["jwt_config:ExpiryDuration_app"].ToString();
            jwt_config["ExpiryDuration_web"] = appsettings["jwt_config:ExpiryDuration_web"].ToString();
        }
        public async Task<responseData> Login(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                string input = req.addInfo["UserId"].ToString();
                bool isEmail = IsValidEmail(input);
                bool isMobileNumber = IsValidMobileNumber(input);
                string columnName;
                if (isEmail)
                {
                    columnName = "Email";
                }
                else if (isMobileNumber)
                {
                    columnName = "Phone";
                }
                else
                {
                    columnName = "";
                }

                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@UserId", input),
                new MySqlParameter("@Role", req.addInfo["Role"].ToString()),
                new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString())
                };

                var sq = $"SELECT * FROM pc_student.TEDrones_Users WHERE Role=@Role AND {columnName} = @UserId AND UserPassword = @UserPassword;";
                var data = ds.ExecuteSQLName(sq, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rStatus = 404;
                    resData.rData["rMessage"] = "Invalid Credentials";
                }
                else
                {
                    // Create a new session token
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_config["Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var Sectoken = new JwtSecurityToken(jwt_config["Issuer"],
                      jwt_config["Issuer"],
                      null,
                      expires: DateTime.Now.AddMinutes(120),
                      signingCredentials: credentials);
                    var Token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                    MySqlParameter[] sessionParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@Token", Token),
                        new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    };

                    string sessionQuery = @"INSERT INTO pc_student.TEDrones_Sessions (UserId, Token) VALUES (@UserId, @Token)";
                    ds.ExecuteSQLName(sessionQuery, sessionParams);

                    resData.eventID = req.eventID;
                    resData.rStatus = 200;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Login Successfully, Welcome!";
                    resData.rData["UserId"] = input;
                    resData.rData["Token"] = Token;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                resData.rStatus = 199;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex.Message.ToString();
            }
            return resData;
        }
        public async Task<responseData> Logout(requestData rData)
        {
            responseData resData = new responseData();
            try
            {
                if (!rData.addInfo.ContainsKey("Token"))
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Token is required for logout";
                    return resData;
                }

                MySqlParameter[] myParam = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@Token", rData.addInfo["Token"]),
                };

                string query = @"DELETE FROM pc_student.TEDrones_Sessions WHERE UserId=@UserId AND Token = @Token";
                var dbData = ds.ExecuteSQLName(query, myParam);
                if (dbData.Count() == null)
                {
                    resData.rData["rCode"] = 3;
                    resData.rData["rMessage"] = "Failed to logout!!";
                }
                else
                {
                    resData.eventID = rData.eventID;
                    resData.rStatus = 200;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Logout Successfully";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "Error: " + ex.Message;
            }
            return resData;
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsValidMobileNumber(string phoneNumber)
        {
            string pattern = @"^[0-9]{7,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}