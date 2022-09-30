﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace LeaveManagementSystem
{
    internal class DataAccess
    {
        public string ConnectionString { get; private set; }

        private SqlConnection sql_connection;

        public DataAccess(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;

            sql_connection = new SqlConnection(ConnectionString);
        }

        //Default constructor to be used by business logic layer classes 
        public DataAccess()
        {
            // update connection string
            this.ConnectionString = @"Data Source=DESKTOP-HK;Initial Catalog=Northwind;Integrated Security=True";
            sql_connection = new SqlConnection(ConnectionString);
        }

        public (string field, string value)[] GetFieldValues(object obj)
        {
            IList<PropertyInfo> properties = new List<PropertyInfo>(obj.GetType().GetProperties());

            var fieldValues = new List<(string field, string value)>();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);

                //Console.WriteLine($"Property: {property.Name} value: {value.ToString()}");
                fieldValues.Add((property.Name, value.ToString()));
            }
            return fieldValues.ToArray();
        }

        public object[] RetrieveObjects(Type type, string condition = "")
        {
            DataSet ds = new DataSet();
            var adaptor = RetrieveData(type.Name, condition);
            adaptor.Fill(ds);
            DataTable table = ds.Tables[0];

            object[] results = new object[table.Rows.Count];

            for (int i = 0; i < results.Length; i++)
            {
                var row = table.Rows[i];

                object[] args = new object[table.Columns.Count];

                for (int j = 0; j < args.Length; j++)
                {
                    var t = row[j].GetType();

                    if (t.Name == "DBNull")
                    {
                        args[j] = null;
                    }
                    else if (t.Name == "String")
                    {
                        args[j] = row[j].ToString();
                        //Console.WriteLine(row[j].ToString());
                    }
                    else if (t.Name == "Int32")
                    {
                        args[j] = row[j].ToString();//we use string for ids but in the database it is int
                        //Convert.ToInt32(row[j].ToString());
                    }
                    else
                    {
                        Console.WriteLine($"Undefined type with Name: {t.Name} returned from DB in RetrieveObjects");
                    }
                }

                //Console.WriteLine(row[0].ToString());

                results[i] = Activator.CreateInstance(type, args);
                //here we create a object by type and passing in values to its constructor
                //need to make sure that the order of the args matches the class constructor params
            }
            return results;
        }
        public object[] RetrieveObjects(string dbType, Type type, string condition = "")
        {
            DataSet ds = new DataSet();
            var adaptor = RetrieveData(dbType, condition);
            adaptor.Fill(ds);
            DataTable table = ds.Tables[0];

            object[] results = new object[table.Rows.Count];

            for (int i = 0; i < results.Length; i++)
            {
                var row = table.Rows[i];

                object[] args = new object[table.Columns.Count];

                for (int j = 0; j < args.Length; j++)
                {
                    var t = row[j].GetType();

                    if (t.Name == "DBNull")
                    {
                        args[j] = null;
                    }
                    else if (t.Name == "String")
                    {
                        args[j] = row[j].ToString();
                        //Console.WriteLine(row[j].ToString());
                    }
                    else if (t.Name == "Int32")
                    {
                        args[j] = row[j].ToString();//we use string for ids but in the database it is int
                        //Convert.ToInt32(row[j].ToString());
                    }
                    else
                    {
                        Console.WriteLine($"Undefined type with Name: {t.Name} returned from DB in RetrieveObjects");
                    }
                }

                //Console.WriteLine(row[0].ToString());

                results[i] = Activator.CreateInstance(type, args);
                //here we create a object by type and passing in values to its constructor
                //need to make sure that the order of the args matches the class constructor params
            }
            return results;
        }

        public void InsertObject(object obj)
        {
            Insert(obj.GetType().Name, GetFieldValues(obj));
        }

        public SqlDataAdapter RetrieveData(string table, string condition = "")
        {
            sql_connection.Open();

            string query;

            if (condition == "")
                query = $"SELECT * FROM {table}";
            else
                query = $"SELECT * FROM {table} WHERE {condition}";


            SqlDataAdapter adapter = null;

            try
            {
                adapter = new SqlDataAdapter(query, sql_connection);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error retrieving data: " + error.Message);
            }
            finally
            {
                sql_connection.Close();
            }

            if (adapter == null) { }//raise excepetion or something here

            return adapter;
        }

        public void Insert(string table, (string Field, string Value)[] FieldValues)
        {
            sql_connection.Open();

            Debug.Assert(FieldValues != null);

            string FieldsConcat = "", ValuesConcat = "";

            for (int i = 0; i < FieldValues.Length; i++)
            {
                ValuesConcat += ("'" + FieldValues[i].Value + "'");

                if (i == FieldValues.Length - 1)
                    FieldsConcat += FieldValues[i].Field;
                else
                {
                    FieldsConcat += (FieldValues[i].Field + ",");
                    ValuesConcat += ",";
                }
            }

            string query = $"INSERT INTO {table}({FieldsConcat}) VALUES({ValuesConcat})";//.ToLower();
            Console.WriteLine(query);

            SqlCommand command = new SqlCommand(query, sql_connection);
            try
            {
                command.ExecuteNonQuery();

                //Console.WriteLine("Values inserted");
                Debug.WriteLine("Insert successful");
            }
            catch (Exception error)
            {
                Console.WriteLine("Insert Error: " + error.Message);
            }
            finally
            {
                sql_connection.Close();
            }
        }

        public void Update(string table, (string Field, string Value)[] FieldValues, string condition = "")
        {
            sql_connection.Open();

            string builder = "";

            for (int i = 0; i < FieldValues.Length; i++)
            {
                if (i == FieldValues.Length - 1)
                    builder += $"{FieldValues[i].Field} = '{FieldValues[i].Value}'";
                else
                    builder += $"{FieldValues[i].Field} = '{FieldValues[i].Value}', ";
            }

            string query;

            if (condition == "")
                query = $"UPDATE {table} SET {builder}";
            else
                query = $"UPDATE {table} SET {builder} WHERE {condition}";

            Console.WriteLine(query);


            SqlCommand command = new SqlCommand(query, sql_connection);

            try
            {
                command.ExecuteNonQuery();
                //Console.WriteLine(query);
                //Console.WriteLine("Value updated");
                Debug.WriteLine("Update successful");
            }
            catch (Exception error)
            {
                Console.WriteLine("Update Error: " + error.Message);
            }
            finally
            {
                sql_connection.Close();
            }
        }

        public void Delete(string table, string condition)
        {
            sql_connection.Open();

            string query = $"DELETE FROM {table} WHERE {condition}";
            Console.WriteLine(query);

            SqlCommand command = new SqlCommand(query, sql_connection);

            try
            {
                command.ExecuteNonQuery();
                //Console.WriteLine(query);
                //Console.WriteLine("Value deleted");
                Debug.WriteLine("Delete successful");
            }
            catch (Exception error)
            {
                Console.WriteLine("Delete Error: " + error.Message);
            }
            finally
            {
                sql_connection.Close();
            }
        }

        public void sqlQuery(string query)
        {
            sql_connection.Open();
            SqlCommand command = new SqlCommand(query, sql_connection);
            try
            {
                command.ExecuteNonQuery();

                //Console.WriteLine("Values inserted");
                Debug.WriteLine("Successful");
            }
            catch (Exception error)
            {
                Console.WriteLine("Insert Error: " + error.Message);
            }
            finally
            {
                sql_connection.Close();
            }
        }
    }

}
