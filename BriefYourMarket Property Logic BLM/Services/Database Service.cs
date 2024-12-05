using BriefYourMarketPropertyLogicBLM.Converters;
using BriefYourMarketPropertyLogicBLM.Models;
using System.Data.SqlClient;

namespace BriefYourMarketPropertyLogicBLM.Services
{
    internal class DatabaseService
    {
        private LoggerService Logger;

        public DatabaseService(LoggerService _logger)
        {
            Logger = _logger;
        }

        public (int, string, string, string) GetInstanceConfig(string instance)
        {
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching instance details for {instance}");

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = @"";
            int instanceId = 0;
            string databaseServer = "";
            string database = "";
            string branchIds = "";

            connection = new SqlConnection(AppSettingsModel.ConnectionString);
            connection.Open();
            command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("@Host", instance));

            try
            {
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    instanceId = dataReader.GetInt32(0);
                    databaseServer = dataReader.GetString(1);
                    database = dataReader.GetString(2);
                    branchIds = dataReader.GetString(3);
                }

                dataReader.Close();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to fetch instance details for {instance}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            connection.Close();

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Instance Id: {instanceId}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Database Server: {databaseServer}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Database: {database}");
            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Branch Ids: {branchIds}");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched instance details for {instance}");

            return (instanceId, databaseServer, database, branchIds);
        }

        public void GetPropertyData(InstanceModel instance, BranchModel branch)
        {
            string branchName = branch.Id.Replace($"BYMPL{instance.Id}", "");

            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetching data for {branchName}");

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string connectionString = AppSettingsModel.ConnectionString.Replace("BYM-ASQLConfig", instance.DatabaseServer).Replace("BYMConfiguration", instance.Database);
            string sql = @"";

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@Branch", branchName));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    string internalId = dataReader.GetString(0);

                    PropertyDataModel property = new()
                    {
                        Id = int.Parse(internalId.Substring(0, internalId.IndexOf("_")))
                    };

                    DataModel propertyData = new()
                    {
                        Field = dataReader.GetName(0),
                        Value = internalId.Replace($"{property.Id}_", "")
                    };

                    property.Data.Add(propertyData);

                    for (int i = 1; i < dataReader.FieldCount; i++)
                    {
                        propertyData = new()
                        {
                            Field = dataReader.GetName(i),
                            Value = dataReader.GetValue(i).ToString()
                        };

                        property.Data.Add(propertyData);
                    }

                    propertyData = new()
                    {
                        Field = "BranchId",
                        Value = branch.Id.Replace(" ", "")
                    };

                    property.Data.Add(propertyData);

                    property.Images = GetPropertyImages(property.Id, connectionString, branchName);
                    branch.Properties.Add(property);
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to fetch data for {branchName}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            Logger.LogMessage(StandardValues.LoggerValues.Debug, $"{branch.Properties.Count} properties returned for {branchName}");
            Logger.LogMessage(StandardValues.LoggerValues.Info, $"Fetched data for {branchName}");
        }

        public List<ImageDataModel> GetPropertyImages(int propertyId, string connectionString, string branchName)
        {
            List<ImageDataModel> propertyImages = new();

            SqlConnection connection;
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = @"";

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@Id", propertyId));
                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        if (propertyImages.Count > i)
                        {
                            propertyImages[i].Value.Add(dataReader.GetString(i));
                        }

                        else
                        {
                            ImageDataModel propertyImage = new()
                            {
                                Field = dataReader.GetName(i)
                            };

                            propertyImage.Value.Add(dataReader.GetString(i));
                            propertyImages.Add(propertyImage);
                        }
                    }
                }

                dataReader.Close();
                connection.Close();
            }

            catch (Exception ex)
            {
                Logger.LogMessage(StandardValues.LoggerValues.Warning, $"Failed to fetch data for {branchName}");
                Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return propertyImages;
        }
    }
}
