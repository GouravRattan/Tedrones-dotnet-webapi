using MySql.Data.MySqlClient;



public class dbServices
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //MySqlConnection conn = null; // this will store the connection which will be persistent 
    MySqlConnection connPrimary = null; // this will store the connection which will be persistent 
    MySqlConnection connReadOnly = null;

    public dbServices() // constructor
    {
        //_appsettings=appsettings;
        connectDBPrimary();
        connectDBReadOnly();
    }


    private void connectDBPrimary()
    {

        try
        {
            connPrimary = new MySqlConnection(appsettings["db:connStrPrimary"]);
            connPrimary.Open();
        }
        catch (Exception ex)
        {
            //throw new ErrorEventArgs(ex); // check as this will throw exception error
            Console.WriteLine(ex);
        }
    }
    private void connectDBReadOnly()
    {

        try
        {
            connReadOnly = new MySqlConnection(appsettings["db:connStrReadOnly"]);
            connReadOnly.Open();
        }
        catch (Exception ex)
        {
            //throw new ErrorEventArgs(ex); // check as this will throw exception error
            Console.WriteLine(ex);
        }
    }




    public List<List<Object[]>> executeSQL(string sq, MySqlParameter[] prms) // this will return the database response the last partameter is to allow selection of connectio id
    {
        MySqlTransaction trans = null;
        //ArrayList allTables=new ArrayList();
        List<List<Object[]>> allTables = new List<List<Object[]>>();

        try
        {
            if (connPrimary == null || connPrimary.State == 0)
                connectDBPrimary();

            trans = connPrimary.BeginTransaction();

            var cmd = connPrimary.CreateCommand();
            cmd.CommandText = sq;
            if (prms != null)
                cmd.Parameters.AddRange(prms);


            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                do
                {
                    List<Object[]> tblRows = new List<Object[]>();
                    while (dr.Read())
                    {
                        object[] values = new object[dr.FieldCount]; // create an array with sixe of field count
                        dr.GetValues(values); // save all values here
                        tblRows.Add(values); // add this to the list array
                    }
                    allTables.Add(tblRows);
                } while (dr.NextResult());
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            trans.Rollback(); // check these functions
            return null; // if error return null
        }
        Console.Write("Database Operation Completed Successfully");
        trans.Commit(); // check thee functions
        connPrimary.Close(); //here is close the connection
        return allTables; // if success return allTables
    }


    // for updation query 


    public int ExecuteUpdateSQL(string sql, MySqlParameter[] parameters)
    {
        MySqlTransaction transaction = null;
        int rowsAffected = 0;

        try
        {
            if (connPrimary == null || connPrimary.State == 0)
                connectDBPrimary();

            transaction = connPrimary.BeginTransaction();

            var cmd = connPrimary.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            rowsAffected = cmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            transaction.Rollback();
            return -1; // Return -1 to indicate error
        }
        finally
        {
            connPrimary.Close(); // Close the connection
        }

        Console.Write("Database Operation Completed Successfully");
        return rowsAffected;
    }
    //update end



    public List<Dictionary<string, object>[]> ExecuteSQLName(string query, MySqlParameter[] parameters)
    {
        MySqlTransaction transaction = null;
        List<Dictionary<string, object>[]> allTables = new List<Dictionary<string, object>[]>();

        try
        {
            if (connPrimary == null || connPrimary.State == 0)
                connectDBPrimary();

            transaction = connPrimary.BeginTransaction();

            using (MySqlCommand cmd = new MySqlCommand(query, connPrimary))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    do
                    {
                        List<Dictionary<string, object>> tblRows = new List<Dictionary<string, object>>();

                        while (reader.Read())
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object columnValue = reader.GetValue(i);
                                values[columnName] = columnValue;
                            }

                            tblRows.Add(values);
                        }

                        allTables.Add(tblRows.ToArray());
                    } while (reader.NextResult());
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (transaction != null)
                transaction.Rollback();
            return null;
        }


        Console.WriteLine("Database Operation Completed Successfully");
        return allTables;
    }

    public int ExecuteInsertAndGetLastId(string sq, MySqlParameter[] prms)
    {
        MySqlTransaction trans = null;
        int lastInsertedId = -1;

        try
        {
            if (connPrimary == null || connPrimary.State == 0)
                connectDBPrimary();

            trans = connPrimary.BeginTransaction();

            var cmd = connPrimary.CreateCommand();
            cmd.CommandText = sq;
            if (prms != null)
                cmd.Parameters.AddRange(prms);

            // Execute the INSERT query
            cmd.ExecuteNonQuery();

            // Get the last inserted ID
            cmd.CommandText = "SELECT LAST_INSERT_ID();";
            lastInsertedId = Convert.ToInt32(cmd.ExecuteScalar());

        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            trans.Rollback();
        }

        trans.Commit();
        connPrimary.Close();

        return lastInsertedId;
    }


    public List<List<Object[]>> executeSQLpcmdb(string sq, MySqlParameter[] prms) // this will return the database response the last partameter is to allow selection of connectio id
    {

        MySqlTransaction trans = null;
        List<List<Object[]>> allTables = new List<List<Object[]>>();

        try
        {
            if (connReadOnly == null)
                connectDBReadOnly();

            trans = connReadOnly.BeginTransaction();

            var cmd = connReadOnly.CreateCommand();
            cmd.CommandText = sq;
            if (prms != null)
                cmd.Parameters.AddRange(prms);

            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                do
                {
                    List<Object[]> tblRows = new List<Object[]>();
                    while (dr.Read())
                    {
                        object[] values = new object[dr.FieldCount]; // create an array with sixe of field count
                        dr.GetValues(values); // save all values here
                        tblRows.Add(values); // add this to the list array
                    }
                    allTables.Add(tblRows);
                } while (dr.NextResult());
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
            trans.Rollback(); // check these functions
            return null; // if error return null
        }
        Console.Write("Database Operation Completed Successfully");
        trans.Commit(); // check thee functions
        connReadOnly.Close(); //here is close the connection
        return allTables; // if success return allTables
    }

    public async Task<List<Dictionary<string, object>>> ExecuteSQLAsync(string query, MySqlParameter[] parameters)
    {
        List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

        using (var connection = new MySqlConnection("server=210.210.210.50;user=test_user;password=test*123;port=2020;database=pc_student"))
        {
            await connection.OpenAsync();

            using (var command = new MySqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }
        }
        return results;
    }



}