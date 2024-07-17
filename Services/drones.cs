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
        public async Task<responseData> GetAllDrones(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.All_Drones ORDER BY DroneId ASC";
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
                                    ImageThumbnailUrl = rowData.ElementAtOrDefault(5)
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
