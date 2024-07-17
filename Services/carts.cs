using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class carts
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
            resData.rData["rCode"] = 0;
            resData.eventID = req.eventID;
            resData.rData["rMessage"] = "Drone found successfully!";
            try
            {
                string CartId = req.addInfo["CartId"].ToString();
                string UserId = req.addInfo["UserId"].ToString();
                string DroneId = req.addInfo["DroneId"].ToString();

                MySqlParameter[] myParams = new MySqlParameter[]
                {
                    new MySqlParameter("@CartId", req.addInfo["CartId"]),
                    new MySqlParameter("@UserId", req.addInfo["UserId"]),
                    new MySqlParameter("@DroneId", req.addInfo["DroneId"])
                };

                string getsql = $"SELECT * FROM pc_student.TEDrones_Carts " +
                             "WHERE CartId = @CartId AND UserId = @UserId AND DroneId = @DroneId;";
                var cartdata = ds.ExecuteSQLName(getsql, myParams);
                if (cartdata == null || cartdata.Count == 0 || cartdata[0].Count() == 0)
                {
                    resData.rData["rCode"] = 2;
                    resData.rData["rMessage"] = "Drone not found!";
                }
                else
                {
                    var cartData = cartdata[0][0];
                    resData.rData["CartId"] = cartData["CartId"];
                    resData.rData["UserId"] = cartData["UserId"];
                    resData.rData["DroneId"] = cartData["DroneId"];
                    resData.rData["Quantity"] = cartData["Quantity"];
                    resData.rData["Price"] = cartData["Price"];
                    resData.rData["TotalPrice"] = cartData["TotalPrice"];
                }
            }
            catch (Exception ex)
            {
                resData.rStatus = 402;
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = ex + "Enter correct Cartid or DroneId or UserId!";
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