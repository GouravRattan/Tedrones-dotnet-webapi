using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class deleteProfile
    {
        dbServices ds = new dbServices();

        public async Task<responseData> DeleteProfile(requestData req)
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
    }
}
