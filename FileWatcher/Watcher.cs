using System;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace FileWatcher
{
    /// <summary>
    /// Starts and stops monitor system.
    /// Saves information about file and directory changes
    /// </summary>
    public class Watcher
    {
        FileSystemWatcher fsw;
        static OracleConnection connection = null;

        public Watcher()
        {
            fsw = new FileSystemWatcher();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void RunMonitorSystem(string path, string fileType, bool includeSubdirectories, bool created, bool deleted, bool renamed, bool changed, OracleConnection connection)
        {
            Watcher.connection = connection;

            // Create a new FileSystemWatcher and set its properties.
            fsw.Path = path;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            fsw.NotifyFilter = NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.FileName
                             | NotifyFilters.DirectoryName;

            // Watch specific type of files.
            fsw.Filter = fileType;

            // Include Subdirectories
            if (includeSubdirectories) fsw.IncludeSubdirectories = true;
            else fsw.IncludeSubdirectories = false;

            // Add event handlers.
            if (created) fsw.Created += OnChanged;
            if (deleted) fsw.Deleted += OnChanged;
            if (renamed) fsw.Renamed += OnRenamed;
            if (changed) fsw.Changed += OnChanged;

            // Begin watching.
            fsw.EnableRaisingEvents = true;
        }

        public void StopMonitorSystem()
        {
            // Stop watching.
            fsw.EnableRaisingEvents = false;

            // Subtract event handlers.
            fsw.Created -= OnChanged;
            fsw.Deleted -= OnChanged;
            fsw.Renamed -= OnRenamed;
            fsw.Changed -= OnChanged;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            string sql = "INSERT INTO MONITORING(file_name, file_path, operation, change_date) " +
                         $" VALUES('{e.Name}', '{e.FullPath}', '{e.ChangeType}', SYSDATE)";
            
            insert(sql);

            /* Save report to a file
            // Specify what is done when a file is changed, created, or deleted.
            using (StreamWriter File = new StreamWriter("raport.txt", true))
            {
                File.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            }
            */
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            string sql = "INSERT INTO MONITORING(old_file_name, file_name, old_file_path, file_path, operation, change_date) " +
                         $"VALUES('{e.OldName}', '{e.Name}', '{e.OldFullPath}', '{e.FullPath}', '{e.ChangeType}', SYSDATE)";

            insert(sql);

            /* Save report to a file
             // Specify what is done when a file is renamed.
            using (StreamWriter file = new StreamWriter("raport.txt", true))
            {
                file.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            }
            */
        }

        private static void insert(string sql_statement)
        {
            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql_statement;
            cmd.CommandType = CommandType.Text;

            try
            {
                int n = cmd.ExecuteNonQuery();

                if (n > 0)
                {
                    cmd.CommandText = "COMMIT";
                    cmd.ExecuteNonQuery();
                    
                    
                }
            }
            catch (Exception expe)
            {
                throw;
            }
        }
    }
}
