using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class cartItems
    {
        dbServices ds = new dbServices();
        public async Task<responseData> AddToCart(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"])
                };

                var checkQuery = @"SELECT * FROM pc_student.TEDrones_Carts WHERE UserId=@UserId AND DroneId = @DroneId;";
                var dbCheckData = ds.ExecuteSQLName(checkQuery, checkParams);
                if (dbCheckData[0].Count() != 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone with this Id already exists in cart!";
                }
                else
                {
                    MySqlParameter[] insertParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                        new MySqlParameter("@Quantity", rData.addInfo["Quantity"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                    };
                    var insertQuery = @"INSERT INTO pc_student.TEDrones_Carts (UserId, DroneId, Quantity, Price, TotalPrice) 
                                        VALUES (@UserId, @DroneId, @Quantity, @Price, @TotalPrice);";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(insertQuery, insertParams);

                    if (rowsAffected > 0)
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone added to cart successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Failed to add drone to cart!";
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

        public async Task<responseData> UpdateInCart(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                MySqlParameter[] checkParams = new MySqlParameter[]
                {
                    new MySqlParameter("@CartId", rData.addInfo["CartId"]),
                    new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Carts WHERE CartId=@CartId AND UserId=@UserId AND DroneId=@DroneId";
                var dbData = ds.ExecuteSQLName(query, checkParams);
                if (dbData[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found in cart!";
                }
                else
                {
                    MySqlParameter[] updateParams = new MySqlParameter[]
                   {
                        new MySqlParameter("@CartId", rData.addInfo["CartId"]),
                        new MySqlParameter("@UserId", rData.addInfo["UserId"]),
                        new MySqlParameter("@DroneId", rData.addInfo["DroneId"]),
                        new MySqlParameter("@Quantity", rData.addInfo["Quantity"]),
                        new MySqlParameter("@Price", rData.addInfo["Price"]),
                        new MySqlParameter("@TotalPrice", rData.addInfo["TotalPrice"]),
                   };
                    var updatequery = @"UPDATE pc_student.TEDrones_Carts
                                        SET UserId = @UserId, DroneId = @DroneId, Quantity = @Quantity, Price = @Price, TotalPrice = @TotalPrice
                                        WHERE CartId = @CartId;";
                    var updatedata = ds.ExecuteInsertAndGetLastId(updatequery, updateParams);
                    if (updatedata != 0)
                    {
                        resData.rData["rCode"] = 3;
                        resData.rData["rMessage"] = "Some error occured, couldn't update cart products!";
                    }
                    else
                    {
                        resData.eventID = rData.eventID;
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Cart products updated successfully.";
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

        public async Task<responseData> RemoveFromCart(requestData rData)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            try
            {
                // int cartId = Convert.ToInt32(rData.addInfo["CartId"]);
                // int droneId = Convert.ToInt32(rData.addInfo["DroneId"]);
                string CartId = rData.addInfo["CartId"].ToString();
                string UserId = rData.addInfo["UserId"].ToString();
                string DroneId = rData.addInfo["DroneId"].ToString();

                MySqlParameter[] para = new MySqlParameter[]
                {
                    new MySqlParameter("@CartId", CartId),
                    new MySqlParameter("@UserId", UserId),
                    new MySqlParameter("@DroneId", DroneId)
                };

                var query = @"SELECT * FROM pc_student.TEDrones_Carts WHERE CartId = @CartId AND UserId=@UserId AND DroneId = @DroneId;";
                var dbData = ds.ExecuteSQLName(query, para);
                if (dbData.Count == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found in the cart!";
                }
                else
                {
                    para = new MySqlParameter[]
                    {
                        new MySqlParameter("@CartId", CartId),
                        new MySqlParameter("@UserId", UserId),
                        new MySqlParameter("@DroneId", DroneId)
                    };

                    var deleteSql = @"DELETE FROM pc_student.TEDrones_Carts WHERE CartId = @CartId AND UserId=@UserId AND DroneId = @DroneId;";
                    int rowsAffected = ds.ExecuteInsertAndGetLastId(deleteSql, para);
                    if (rowsAffected > 0)
                    {
                        resData.rData["rCode"] = 0;
                        resData.rData["rMessage"] = "Drone removed from cart successfully.";
                    }
                    else
                    {
                        resData.rData["rCode"] = 2;
                        resData.rData["rMessage"] = "Failed to remove drone from cart!";
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

        public async Task<responseData> GetACartItem(requestData req)
        {
            responseData resData = new responseData();
            MySqlParameter[] items = new MySqlParameter[]
            {
                new MySqlParameter("@CartId", req.addInfo["CartId"])
            };
            try
            {
                var result = new ArrayList();
                var query = "SELECT * FROM pc_student.TEDrones_CartItems WHERE CartId=@CartId;";
                var data = ds.ExecuteSQLName(query, items);

                if (data == null)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Error in payload";
                }
                else
                {
                    for (var i = 0; i < data.Count(); i++)
                    {
                        foreach (var row in data[i])
                        {
                            Dictionary<string, object> myDict = new Dictionary<string, object>();

                            foreach (var field in row.Keys)
                            {
                                // if (field == "image" && row[field] is byte[])
                                // {
                                // Convert byte array to Base64 string
                                //     myDict[field] = Convert.ToBase64String((byte[])row[field]);
                                // }
                                // else
                                // {
                                myDict[field] = row[field].ToString();
                                // }
                            }
                            result.Add(myDict);
                            // Insert the data into the orders table
                            var insertQuery = @"INSERT INTO pc_student.TEDrones_CartItems (CartItemId, CartId, DroneId, Quantity, Price, TotalPrice) VALUES (@CartItemId, @CartId, @DroneId, @Quantity, @TotalPrice, @TotalPrice);";

                            MySqlParameter[] insertParams = new MySqlParameter[]
                            {
                                new MySqlParameter("@CartItemId", myDict["id"]),
                                new MySqlParameter("@CartId", myDict["CartId"]),
                                new MySqlParameter("@DroneId", myDict["DroneId"].ToString()),
                                new MySqlParameter("@Quantity", myDict["Quantity"]),
                                new MySqlParameter("@Price", myDict["Price"]),
                                new MySqlParameter("@TotalPrice", myDict["TotalPrice"]),
                            };

                            ds.ExecuteSQLName(insertQuery, insertParams);
                        }
                    }
                    resData.rData["rData"] = result;
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "displayed data";
                }
            }
            catch (Exception e)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = e.Message;
            }
            return resData;
        }

        public async Task<responseData> GetAllCartItems(requestData req)
        {
            responseData resData = new responseData();
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            try
            {
                var query = @"SELECT * FROM pc_student.TEDrones_Carts ORDER BY CartId ASC";
                var dbData = ds.executeSQL(query, null);
                if (dbData == null)
                {
                    resData.rData["rMessage"] = "Some error occurred, can't get all carts!";
                    resData.rStatus = 1;
                    return resData;
                }

                List<object> cartsList = new List<object>();
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
                                var cart = new
                                {
                                    CartId = rowData.ElementAtOrDefault(0),
                                    UserId = rowData.ElementAtOrDefault(1),
                                    DroneId = rowData.ElementAtOrDefault(2),
                                    Quantity = rowData.ElementAtOrDefault(3),
                                    Price = rowData.ElementAtOrDefault(4),
                                    TotalPrice = rowData.ElementAtOrDefault(5)
                                };
                                cartsList.Add(cart);
                            }
                        }
                    }
                }
                resData.rData["rCode"] = 0;
                resData.rData["rMessage"] = "All carts found successfully";
                resData.rData["carts"] = cartsList;
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