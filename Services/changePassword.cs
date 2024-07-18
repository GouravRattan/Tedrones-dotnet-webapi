using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class changePassword
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ChangePassword(requestData req)
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
                string UserId = req.addInfo["UserId"].ToString();
                string UserPassword = req.addInfo["UserPassword"].ToString();
                string NewPassword = req.addInfo["NewPassword"].ToString();

                if (UserPassword == NewPassword)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "New password must be different from the current password";
                }
                else
                {
                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@UserPassword", UserPassword),
                        new MySqlParameter("@NewPassword", NewPassword),
                    };

                    var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId AND UserPassword = @UserPassword;";
                    var checkResult = ds.executeSQL(checkSql, parameters);
                    if (checkResult[0].Count() == 0)
                    {
                        resData.rData["rCode"] = 2;
                        resData.rStatus = 404;
                        resData.rData["rMessage"] = "Wrong credentials, enter valid details!";
                    }
                    else
                    {
                        string updateSql = $"UPDATE pc_student.TEDrones_Users SET UserPassword = @NewPassword WHERE {columnName} = @UserId";
                        var rowsAffected = ds.executeSQL(updateSql, parameters);
                        if (rowsAffected[0].Count() != 0)
                        {
                            resData.rData["rCode"] = 3;
                            resData.rData["rMessage"] = "Failed to change password!";
                        }
                        else
                        {
                            resData.rData["rCode"] = 0;
                            resData.eventID = req.eventID;
                            resData.rData["rMessage"] = "Password changed successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 404;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Error: {ex.Message}";
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