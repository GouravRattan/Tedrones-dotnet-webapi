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
    public class deleteProfile
    {
        dbServices ds = new dbServices();

        public async Task<responseData> DeleteProfileByUser(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString())
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE UserId=@UserId AND UserPassword = @UserPassword;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile not found, No records deleted!";
                }
                else
                {
                    var deleteSql = @"DELETE FROM pc_student.TEDrones_Users WHERE UserId = @UserId AND UserPassword = @UserPassword;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected != null)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile deleted successfully";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Invalid credentials, Wrong Id or Password!";
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
        public async Task<responseData> DeleteProfileByAdmin(requestData req)
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
                    columnName = "UserId";
                }
                string UserId = input;
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId;";
                var checkResult = ds.executeSQL(checkSql, para);
                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile not found, No records deleted!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == null)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Invalid credentials, Wrong Id or Password!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile deleted successfully";
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
