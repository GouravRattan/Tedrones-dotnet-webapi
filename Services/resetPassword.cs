using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class resetPassword
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ResetPassword(requestData req)
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
                string NewPassword = req.addInfo["NewPassword"].ToString();
                string ConfirmPassword = req.addInfo["ConfirmPassword"].ToString();

                var para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@NewPassword", NewPassword)
                };

                var selectSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId;";
                var data = ds.ExecuteSQLName(selectSql, para);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile number not found, Please enter a valid details";
                }
                else
                {
                    if (NewPassword == ConfirmPassword)
                    {
                        var updateSql = $"UPDATE pc_student.TEDrones_Users SET UserPassword = @NewPassword WHERE {columnName} = @UserId;";
                        var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                        if (rowsAffected == 0)
                        {
                            selectSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId AND UserPassword=@NewPassword;";
                            data = ds.ExecuteSQLName(selectSql, para);
                            if (data[0].Count() == 0)
                            {
                                resData.rData["rCode"] = 4;
                                resData.rData["rMessage"] = "Password already exist, new password must be different!";
                            }
                            else
                            {
                                resData.eventID = req.eventID;
                                resData.rData["rCode"] = 0;
                                resData.rData["rMessage"] = "Password reset successfully";
                            }
                        }
                        else
                        {
                            resData.rData["rCode"] = 3;
                            resData.rData["rMessage"] = "Some error occured, could'nt reset password!";
                        }
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "New password and confirm password must match!";
                    }
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
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