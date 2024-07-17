using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class contactUs
    {
        dbServices ds = new dbServices();
        public async Task<responseData> ContactUs(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.rData["rMessage"] = "Feedback sent successfully";

            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@UserName", req.addInfo["UserName"].ToString()),
                    new MySqlParameter("@Email", req.addInfo["Email"].ToString()),
                    new MySqlParameter("@Phone", req.addInfo["Phone"].ToString()),
                    new MySqlParameter("@Topic", req.addInfo["Topic"].ToString()),
                    new MySqlParameter("@Message", req.addInfo["Message"].ToString()),
                };

                var checkSql = $"SELECT * FROM pc_student.TEDrones_Users WHERE Email=@Email;";
                var checkResult = ds.executeSQL(checkSql, para);

                if (checkResult == null || checkResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Email not found in our records. Please register first!";
                }
                else
                {
                    var insertSql = $"INSERT INTO pc_student.TEDrones_Contacts (UserName, Email, Phone, Topic, Message) VALUES(@UserName, @Email, @Phone, @Topic, @Message);";
                    var insertId = ds.ExecuteInsertAndGetLastId(insertSql, para);

                    if (insertId != 0)
                    {
                        resData.eventID = req.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Thank you for your response";
                    }
                    else
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "Failed to submit feedback";
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
    }
}