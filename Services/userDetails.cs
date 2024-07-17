// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using MySql.Data.MySqlClient;

// namespace MyCommonStructure.Services
// {
//     public class userDetails
//     {
//         dbServices ds = new dbServices();

//         public async Task<responseData> UpdateUserProfile(requestData req)
//         {
//             responseData resData = new responseData();
//             resData.rData["rCode"] = 0;
//             resData.rData["rMessage"] = "User profile updated successfully";

//             try
//             {
//                 // Parameters to check if the user exists
//                 MySqlParameter[] checkParams = new MySqlParameter[] {
//                     new MySqlParameter("@date_of_birth", req.addInfo["date_of_birth"].ToString()),
//                     new MySqlParameter("@gender", req.addInfo["gender"].ToString()),
//                     new MySqlParameter("@profile_image", req.addInfo["profile_image"].ToString()),
//                     new MySqlParameter("@email", req.addInfo["email"].ToString()),
//                     new MySqlParameter("@password", req.addInfo["password"].ToString())
//                 };

//                 // Query to check if the user exists
//                 var checkSql = $"SELECT id FROM pc_student.et_register WHERE email=@email;";
//                 var checkResult = ds.executeSQL(checkSql, checkParams);

//                 if (checkResult[0].Count == 0)
//                 {
//                     resData.rData["rCode"] = 2;
//                     resData.rData["rMessage"] = "User not found";
//                 }
//                 else
//                 {
//                     // Get the user ID from the check result and convert to int
//                     int userId = Convert.ToInt32(checkResult[0][0]["id"].ToString());

//                     // Parameters to update the user profile
//                     MySqlParameter[] updateParams = new MySqlParameter[] {
//                         new MySqlParameter("@date_of_birth", req.addInfo["date_of_birth"].ToString()),
//                         new MySqlParameter("@gender", req.addInfo["gender"].ToString()),
//                         new MySqlParameter("@profile_image", req.addInfo["profile_image"].ToString()),
//                         new MySqlParameter("@user_id", userId)
//                     };

//                     // Update user profile data
//                     var updateSql = $"UPDATE pc_student.et_register SET date_of_birth=@date_of_birth, gender=@gender, profile_image=@profile_image WHERE id=@user_id;";
//                     ds.executeSQL(updateSql, updateParams);

//                     resData.eventID = req.eventID;
//                     resData.rData["rMessage"] = "User profile updated";
//                 }
//             }
//             catch (Exception ex)
//             {
//                 resData.rData["rCode"] = 1;
//                 resData.rData["rMessage"] = $"Error: {ex.Message}";
//             }

//             return resData;
//         }
//     }
// }
