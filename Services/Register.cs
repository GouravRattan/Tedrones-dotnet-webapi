using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;

namespace MyCommonStructure.Services
{
    public class Register
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
                        resData.rStatus = 200;
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

        public async Task<responseData> GetUserRegistrationByEmail(requestData req)
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