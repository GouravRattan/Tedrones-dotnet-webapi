using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class drones
    {
        dbServices ds = new dbServices();

        public async Task<responseData> AddDrone(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@Name", rData.addInfo["Name"]),
                };

                var checkQuery = @"SELECT * FROM pc_student.TEDrones_Drones WHERE Name = @Name;";
                var dbCheckData = ds.ExecuteSQLName(checkQuery, checkParams);
                if (dbCheckData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone with this Name already exists!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@Name", rData.addInfo["Name"]),
                        new MySqlParameter("@Description", rData.addInfo["Description"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@ImageUrl", rData.addInfo["ImageUrl"]),
                        new MySqlParameter("@ProductType", rData.addInfo["ProductType"]),
                    };
                    var insertQuery = @"INSERT INTO pc_student.TEDrones_Drones (Name, Description, Price, ImageUrl, ProductType) 
                                        VALUES (@Name, @Description, @Price, @ImageUrl, @ProductType);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);

                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone added successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add drone!";
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


        public async Task<responseData> EditDrone(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Drones WHERE DroneId=@DroneId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No drone found with the provided Id!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                        new MySqlParameter("@Name", rData.addInfo["Name"]),
                        new MySqlParameter("@Description", rData.addInfo["Description"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@ImageUrl", rData.addInfo["ImageUrl"]),
                        new MySqlParameter("@ProductType", rData.addInfo["ProductType"]),
                   };
                    var updatequery = @"UPDATE pc_student.TEDrones_Drones
                                        SET Name = @Name, Description = @Description, Price = @Price, ImageUrl = @ImageUrl, ProductType=@ProductType
                                        WHERE DroneId = @DroneId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, couldn't update details!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone details updated successfully.";
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

        public async Task<responseData> DeleteDrone(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"].ToString()),
                    new MySqlParameter("@Name", rData.addInfo["Name"].ToString())
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Drones WHERE DroneId=@DroneId OR Name=@Name;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "No Drone found!";
                }
                else
                {
                    var deleteSql = $"DELETE FROM pc_student.TEDrones_Drones WHERE DroneId=@DroneId OR Name = @Name";
                    var rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected == 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone deleted successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Couldn't delete drone!";
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

        public async Task<responseData> GetDrone(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Drone found successfully!";
            try
            {
                string DroneId = req.addInfo["DroneId"].ToString();
                string Name = req.addInfo["Name"].ToString();
                string ProductType = req.addInfo["ProductType"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@DroneId", req.addInfo["DroneId"]),
                    new MySqlParameter("@Name", req.addInfo["Name"]),
                    new MySqlParameter("@ProductType", req.addInfo["ProductType"])
                };

                string getsql = $"SELECT * FROM pc_student.TEDrones_Drones " +
                             "WHERE DroneId = @DroneId OR Name = @Name OR ProductType = @ProductType;";
                var Dronedata = ds.ExecuteSQLName(getsql, myParams);
                if (Dronedata == null || Dronedata.Count == 0 || Dronedata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found!";
                }
                else
                {
                    var DroneData = Dronedata[0][0];
                    resData.rData["DroneId"] = DroneData["DroneId"];
                    resData.rData["Name"] = DroneData["Name"];
                    resData.rData["Description"] = DroneData["Description"];
                    resData.rData["Price"] = DroneData["Price"];
                    resData.rData["ImageUrl"] = DroneData["ImageUrl"];
                    resData.rData["ProductType"] = DroneData["ProductType"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct Drone or Description name!";
            }
            return resData;
        }

        public async Task<responseData> GetAllDrones(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.TEDrones_Drones ORDER BY DroneId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all Drones!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> dronesList = new List<object>();
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
                                var drone = new
                                {
                                    DroneId = rowData.ElementAtOrDefault(0),
                                    Name = rowData.ElementAtOrDefault(1),
                                    Description = rowData.ElementAtOrDefault(2),
                                    Price = rowData.ElementAtOrDefault(3),
                                    ImageUrl = rowData.ElementAtOrDefault(4),
                                    ProductType = rowData.ElementAtOrDefault(5)
                                };
                                dronesList.Add(drone);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All Drones found successfully";
                resData.rData["Drones"] = dronesList;
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = $"Exception occurred: {ex.Message}";
            }
            return resData;
        }
    }
}
