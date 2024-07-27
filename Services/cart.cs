using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MyCommonStructure.Services
{
    public class cart
    {
        dbServices ds = new dbServices();
        public async Task<responseData> GetData(requestData req)
        {
            responseData resData = new responseData();
            MySqlParameter[] items = new MySqlParameter[]
            {
         new MySqlParameter("@id", req.addInfo["id"])
            };

            try
            {
                var result = new ArrayList();
                var query = "SELECT * FROM Food_Items WHERE id=@id;";
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
                            var insertQuery = @" INSERT INTO Orders (uid, category, image, title, details, price) VALUES (@uid, @category, @image, @title, @details, @price);";

                            MySqlParameter[] insertParams = new MySqlParameter[]
                            {
                        new MySqlParameter("@uid", myDict["id"]),
                        new MySqlParameter("@category", myDict["category"]),
                        new MySqlParameter("@image", myDict["image"].ToString()),
                        new MySqlParameter("@title", myDict["title"]),
                        new MySqlParameter("@details", myDict["details"]),
                        new MySqlParameter("@price", myDict["price"])
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

        public async Task<responseData> DeleteOrder(requestData req)
        {
            responseData resData = new responseData();
            MySqlParameter[] items = new MySqlParameter[]{
            new MySqlParameter("@id", req.addInfo["id"])
        };
            try
            {
                var result = new ArrayList();
                var query = "DELETE from Orders WHERE id=@id;";
                var data = ds.ExecuteSQLName(query, items);
                if (data != null)
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "data deleted!!";
                }

                else
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Error in payload";
                }
            }

            catch (Exception e)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = e.Message;
            }
            return resData;
        }

        public async Task<responseData> OrderHistory(requestData req)
        {
            responseData resData = new responseData();
            MySqlParameter[] items = new MySqlParameter[]{
          new MySqlParameter("@uid", req.addInfo["uid"])
        };

            try
            {
                var result = new ArrayList();
                var query = "select * from Orders WHERE uid=@uid;";
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
                                if (field == "image" && row[field] is byte[])
                                {
                                    // Convert byte array to Base64 string
                                    myDict[field] = Convert.ToBase64String((byte[])row[field]);
                                }
                                else
                                {
                                    myDict[field] = row[field].ToString();
                                }
                            }

                            result.Add(myDict);
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

        public async Task<responseData> DisplayCart(requestData req)
        {
            responseData resData = new responseData();
            MySqlParameter[] items = new MySqlParameter[]{
        //   new MySqlParameter("@uid", req.addInfo["uid"])
        };

            try
            {
                var result = new ArrayList();
                var query = "select * from Orders ";
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
                                if (field == "image" && row[field] is byte[])
                                {
                                    // Convert byte array to Base64 string
                                    myDict[field] = Convert.ToBase64String((byte[])row[field]);
                                }
                                else
                                {
                                    myDict[field] = row[field].ToString();
                                }
                            }

                            result.Add(myDict);
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
    }
}