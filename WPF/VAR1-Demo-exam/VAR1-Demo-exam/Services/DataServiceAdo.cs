using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace VAR1_Demo_exam.Services
{
    public class DataServiceAdo<T> where T : class, new()
    {
        private readonly string _connectionString;

        public DataServiceAdo()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["VAR1Ado"]?.ConnectionString
                ?? "Server=USER;Database=VAR1;Integrated Security=True;TrustServerCertificate=True;";
        }

        private static string TableName => typeof(T).Name;

        private static List<PropertyInfo> ScalarProperties => typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .Where(IsScalarType)
            .ToList();

        private static bool IsScalarType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return type.IsPrimitive
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(Guid)
                   || type == typeof(byte[]);
        }

        private static PropertyInfo GetPrimaryKey()
        {
            var props = ScalarProperties;
            var explicitName = props.FirstOrDefault(p => p.Name.Equals($"ID_{TableName}", StringComparison.OrdinalIgnoreCase));
            if (explicitName != null)
            {
                return explicitName;
            }

            var byIdSuffix = props.FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) || p.Name.StartsWith("ID_", StringComparison.OrdinalIgnoreCase));
            return byIdSuffix ?? props.FirstOrDefault();
        }

        public List<T> GetAll()
        {
            try
            {
                var result = new List<T>();
                var props = ScalarProperties;

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand($"SELECT * FROM [{TableName}]", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new T();
                            foreach (var prop in props)
                            {
                                if (!ColumnExists(reader, prop.Name))
                                {
                                    continue;
                                }

                                var value = reader[prop.Name];
                                if (value == DBNull.Value)
                                {
                                    prop.SetValue(item, null);
                                    continue;
                                }

                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                prop.SetValue(item, Convert.ChangeType(value, targetType));
                            }

                            result.Add(item);
                        }
                    }
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        public void Add(T item)
        {
            try
            {
                var pk = GetPrimaryKey();
                var props = ScalarProperties.Where(p => p != pk).ToList();
                var columns = string.Join(", ", props.Select(p => $"[{p.Name}]"));
                var parameters = string.Join(", ", props.Select(p => $"@{p.Name}"));
                var sql = $"INSERT INTO [{TableName}] ({columns}) VALUES ({parameters})";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var prop in props)
                    {
                        command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(item) ?? DBNull.Value);
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        public void Update(T item)
        {
            try
            {
                var pk = GetPrimaryKey();
                if (pk == null)
                {
                    throw new InvalidOperationException("Первичный ключ не найден.");
                }

                var props = ScalarProperties.Where(p => p != pk).ToList();
                var setPart = string.Join(", ", props.Select(p => $"[{p.Name}] = @{p.Name}"));
                var sql = $"UPDATE [{TableName}] SET {setPart} WHERE [{pk.Name}] = @pk";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var prop in props)
                    {
                        command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(item) ?? DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@pk", pk.GetValue(item) ?? DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        public void Delete(T item)
        {
            try
            {
                var pk = GetPrimaryKey();
                if (pk == null)
                {
                    throw new InvalidOperationException("Первичный ключ не найден.");
                }

                var sql = $"DELETE FROM [{TableName}] WHERE [{pk.Name}] = @pk";
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@pk", pk.GetValue(item) ?? DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        private static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
