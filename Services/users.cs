using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.Ocsp;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class users
    {
        dbServices ds = new dbServices();
        public async Task<responseData> GetAllUsers(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.TEDrones_Users WHERE Role = 'User' ORDER BY UserId ASC;";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Users not found!!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> usersList = new List<object>();
                foreach (var rowSet in dbData)
                {
                    if (rowSet != null)
                    {
                        foreach (var row in rowSet)
                        {
                            if (row != null)
                            {
                                List<string> rowData = new List<string>();

                                foreach (var column in row)
                                {
                                    if (column != null)
                                    {
                                        rowData.Add(column.ToString());
                                    }
                                }
                                var user = new
                                {
                                    UserId = rowData.ElementAtOrDefault(0),
                                    UserName = rowData.ElementAtOrDefault(1),
                                    Email = rowData.ElementAtOrDefault(2),
                                    Phone = rowData.ElementAtOrDefault(3),
                                    UserPassword = rowData.ElementAtOrDefault(4),
                                    Address = rowData.ElementAtOrDefault(5),
                                    RegistrationDate = rowData.ElementAtOrDefault(6),
                                    LastLogin = rowData.ElementAtOrDefault(7),
                                    Role = rowData.ElementAtOrDefault(8),
                                    ProfilePic = rowData.ElementAtOrDefault(9),
                                    IsEmailVerified = rowData.ElementAtOrDefault(10),
                                    IsPhoneVerified = rowData.ElementAtOrDefault(11),
                                };
                                usersList.Add(user);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "Users found successfully";
                resData.rData["users"] = usersList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }

        public async Task<responseData> GetUserById(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "User details found successfully";
            try
            {
                string UserId = req.addInfo["UserId"].ToString();
                string Email = req.addInfo["Email"].ToString();
                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@Email", Email),
                };

                var getusersql = $"SELECT * FROM pc_student.TEDrones_Users WHERE UserId=@UserId OR Email=@Email;";
                var data = ds.ExecuteSQLName(getusersql, myParams);
                if (data == null || data[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to get user details!!";
                }
                else
                {
                    resData.rData["UserId"] = data[0][0]["UserId"];
                    resData.rData["UserName"] = data[0][0]["UserName"];
                    resData.rData["Email"] = data[0][0]["Email"];
                    resData.rData["Phone"] = data[0][0]["Phone"];
                    resData.rData["UserPassword"] = data[0][0]["UserPassword"];
                    resData.rData["Address"] = data[0][0]["Address"];
                    resData.rData["RegistrationDate"] = data[0][0]["RegistrationDate"];
                    resData.rData["Role"] = data[0][0]["Role"];
                    resData.rData["ProfilePic"] = data[0][0]["ProfilePic"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occured: {ex.Message}";
            }
            return resData;
        }
        public async Task<responseData> DeleteUserById(requestData req)
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

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", input),
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId OR UserName = @UserName;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile not found, No records deleted!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.TEDrones_Users WHERE {columnName} = @UserId OR UserName = @UserName;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
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

        public async Task<responseData> EditUserPic(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile not found, can not update the picture!";
                }
                else
                {
                    string updateSql = $"UPDATE pc_student.TEDrones_Users SET ProfilePic = @ProfilePic WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                    if (rowsAffected == null)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Invalid credentials, Wrong Id or Password!";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile Pic updated successfully";
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


        public async Task<responseData> DeleteUserPic(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    // new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE Email = @Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Profile not found, No records deleted!";
                }
                else
                {
                    var updateSql = $"UPDATE pc_student.TEDrones_Users SET ProfilePic = NULL WHERE Email = @Email;";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                    if (rowsAffected == null)

                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to remove Profile pic";
                    }
                    else
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile removed Sucessfully";
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