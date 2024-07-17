// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MySql.Data.MySqlClient;
// using Org.BouncyCastle.Ocsp;

// namespace MyCommonStructure.Services
// {
//     public class getProductData
//     {
//         dbServices ds = new dbServices();

//         public async Task<responseData> GetProductData(requestData req)
//         {
//             responseData resData = new responseData();
//             resData.eventID = req.eventID;

//             try
//             {
//                 string product_id = req.addInfo["product_id"].ToString();

//             MySqlParameter[] myParams = new MySqlParameter[] {
//             new MySqlParameter("@product_id", product_id)
//         };
//                 // Query to fetch product data from the database
//                 var query = $"SELECT * FROM pc_student.et_products WHERE product_id = @product_id;";

//                 // Execute the query using the dbServices instance
//                 var data =  ds.ExecuteSQLName(query, myParams);

//                 // Set response data
//                 if (data == null || data[0].Count() == 0)
//                 {
//                     resData.rData["rCode"] = 1;
//                     resData.rData["rMessage"] = "Product Not Found...";
//                 }
//                 else
//                 {

//                     var product = data[0][0];
//                     resData.rData["product_id"] = product["product_id"];
//                     resData.rData["product_name"] = product["product_name"];
//                     resData.rData["description"] = product["description"];
//                     resData.rData["price"] = product["price"];
//                     resData.rData["image"] = product["image"];
//                     resData.rData["rating"] = product["rating"];


//                     resData.rData["rCode"] = 0;
//                     resData.rData["rMessage"] = "Product found";

//                 }

//             }
//             catch (Exception ex)
//             {
//                 resData.rData["rCode"] = 1;
//                 resData.rData["rMessage"] = ex.Message;

//             }
//             return resData;
//         }

//     public async Task<responseData> GetRecipesData(requestData req)
//     {
//         responseData resData = new responseData();
//         resData.eventID = req.eventID;

//         try
//         {
//             string product_id = req.addInfo["product_id"].ToString();

//             // Query to fetch product details along with sections
//             string query = @"
//                 SELECT 
//                     p.product_id,
//                     p.product_name,
//                     p.description,
//                     p.price,
//                     p.image,
//                     p.rating,
//                     s.section_id,
//                     s.title AS section_title,
//                     s.content AS section_content,
//                     s.icon_url AS section_icon
//                 FROM 
//                     pc_student.et_recipes p
//                 LEFT JOIN 
//                     pc_student.et_sections s ON p.product_id = s.product_id
//                 WHERE 
//                     p.product_id = @product_id;";

// Parameters for the query



//             MySqlParameter[] myParams = {
//                 new MySqlParameter("@product_id", product_id)
//             };

//             // Execute the query using dbServices instance (assuming ds.ExecuteSQLName handles execution)
//             List<Dictionary<string, object>> data = ds.ExecuteSQLName(query, myParams);

//             // Process the retrieved data
//             if (data == null || data.Count == 0)
//             {
//                 resData.rData["rCode"] = 1;
//                 resData.rData["rMessage"] = "Product Not Found...";
//             }
//             else
//             {
//                 // Initialize response data
//                 resData.rData["rCode"] = 0;
//                 resData.rData["rMessage"] = "Product found";

//                 // Create product details structure
//                 Dictionary<string, object> productDetails = new Dictionary<string, object>();

//                 // Fill product details
//                 productDetails["product_id"] = data[0]["product_id"];
//                 productDetails["product_name"] = data[0]["product_name"];
//                 productDetails["description"] = data[0]["description"];
//                 productDetails["price"] = data[0]["price"];
//                 productDetails["image"] = data[0]["image"];
//                 productDetails["rating"] = data[0]["rating"];

//                 // Create sections list
//                 List<Dictionary<string, object>> sections = new List<Dictionary<string, object>>();

//                 // Iterate through data to extract sections
//                 foreach (var row in data)
//                 {
//                     if (row["section_id"] != DBNull.Value) // Ensure section data exists
//                     {
//                         Dictionary<string, object> section = new Dictionary<string, object>();
//                         section["section_id"] = row["section_id"];
//                         section["section_title"] = row["section_title"];
//                         section["section_content"] = row["section_content"];
//                         section["section_icon"] = row["section_icon"];
//                         sections.Add(section);
//                     }
//                 }

//                 // Add sections to product details
//                 productDetails["sections"] = sections;

//                 // Add product details to response data
//                 resData.rData["product_details"] = productDetails;
//             }
//         }
//         catch (Exception ex)
//         {
//             resData.rData["rCode"] = 1;
//             resData.rData["rMessage"] = ex.Message;
//         }

//         return resData;
//     }
// }
// }
// public async Task<responseData> GetAllProductData(requestData req)
// {
//     responseData resData = new responseData();
//     resData.eventID = req.eventID;

//     try
//     {
//         string product_id = req.addInfo["product_id"].ToString();

//     MySqlParameter[] myParams = new MySqlParameter[] {
//     new MySqlParameter("@product_id", product_id)
// };
//         // Query to fetch product data from the database
//         var query = $"SELECT * FROM pc_student.et_products WHERE product_id = @product_id;";

//         // Execute the query using the dbServices instance
//         var data =  ds.ExecuteSQLName(query, myParams);

//         // Set response data
//         if (data == null || data[0].Count() == 0)
//         {
//             resData.rData["rCode"] = 1;
//             resData.rData["rMessage"] = "Product Not Found...";
//         }
//         else
//         {
//     var products = new List<Dictionary<string, string>>();
//     foreach (var product in data)
//     {
//         var productData = new Dictionary<string, string>
//         {
//             { "product_id", product["product_id"].ToString() },
//             { "product_name", product["product_name"].ToString() },
//             { "description", product["description"].ToString() },
//             { "price", product["price"].ToString() },
//             { "image", product["image"].ToString() },
//             { "rating", product["rating"].ToString() }
//         };

//         products.Add(productData);
//     }

//     resData.rData["products"] = products;
//     resData.rData["rCode"] = 0;
//     resData.rData["rMessage"] = "Products found";
// }

//     }
//     catch (Exception ex)
//     {
//         resData.rData["rCode"] = 1;
//         resData.rData["rMessage"] = ex.Message;

//     }
//     return resData;
// }