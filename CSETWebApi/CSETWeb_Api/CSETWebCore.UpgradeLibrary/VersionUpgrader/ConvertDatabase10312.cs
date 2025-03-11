//////////////////////////////// 
// 
//   Copyright 2025 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System;
using Microsoft.Data.SqlClient;
using System.IO;


namespace UpgradeLibrary.Upgrade
{
    internal class ConvertDatabase10312 : ConvertSqlDatabase
    {
        public ConvertDatabase10312(string path) : base(path)
        {
            myVersion = new Version("10.3.1.2");
        }


        /// <summary>
        /// Runs the database update script
        /// </summary>
        /// <param name="conn"></param>
        public override void Execute(SqlConnection conn)
        {
            try
            {
                RunFile(Path.Combine(this.applicationPath, @"VersionUpgrader\SQL\1031_to_1032_data.sql"), conn);

                this.UpgradeToVersionLocalDB(conn, myVersion);
            }
            catch (Exception e)
            {
                throw new DatabaseUpgradeException("Error in upgrading database version 10.3.1.0 to 10.3.1.2: " + e.Message);
            }
        }
    }
}