using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class editProfile
    {
        dbServices ds = new dbServices();
        public async Task<responseData> EditProfile(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", req.addInfo["UserId"].ToString()),
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@Address", req.addInfo["Address"].ToString()),
                    new MySqlParameter("@ProfilePic", req.addInfo["ProfilePic"].ToString()),
                    // new MySqlParameter("@UserPassword", req.addInfo["UserPassword"].ToString()),
                };

                var updateSql = @"UPDATE pc_student.TEDrones_Users 
                                SET UserName = @UserName, Email = @Email, Phone = @Phone, Address = @Address, ProfilePic = @ProfilePic 
                                WHERE UserId = @UserId";
                var rowsAffected = ds.ExecuteInsertAndGetLastId(updateSql, para);
                if (rowsAffected != 0)
                {
                    resData.eventID = req.eventID;
                    resData.rData["rCode"] = 0;
                }
                else
                {
                    var selectSql = @"SELECT * FROM pc_student.TEDrones_Users WHERE UserId = @UserId";
                    var existingDataList = ds.ExecuteSQLName(selectSql, para);
                    if (existingDataList != null && existingDataList.Count > 0)
                    {

                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Profile updated successfully";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "No changes were made";
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