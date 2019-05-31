using System;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace FileWatcher
{
    /// <summary>
    /// Scan files in a directory and get information about their attributes
    /// </summary>
    class FileScanner
    {
        #region Result to file

        public void scanSubdirectories(string sourcePath, string destinationPath)
        {
            //  Loop through all the immediate subdirectories of C.
            foreach (string entry in Directory.GetDirectories(sourcePath))
            {
                DisplayFileSystemInfoAttributes(new DirectoryInfo(entry), destinationPath);
            }
        }

        public void scanFiles(string sourcePath, string destinationPath)
        {
            //  Loop through all the files in C.
            foreach (string entry in Directory.GetFiles(sourcePath))
            {
                DisplayFileSystemInfoAttributes(new FileInfo(entry), destinationPath);
            }
        }

        static void DisplayFileSystemInfoAttributes(FileSystemInfo fsi, string destinationPath)
        {
            //  Assume that this entry is a file.
            string entryType = "File";

            // Determine if entry is really a directory
            if ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                entryType = "Directory";
            }

            if (!destinationPath[destinationPath.Length - 1].Equals(@"\"))
            {
                destinationPath += @"\";
            }

            string date = DateTime.Today.ToString("dd/MM/yyyy");
            string time = DateTime.Now.ToString("HH:mm:ss");
            time = time.Replace(':', '-');
            string reportName = $"{destinationPath}scan_{date}_{time}.txt";

            //  Save this entry's type, name, and creation date.
            using (StreamWriter file = new StreamWriter(reportName, true))
            {
                file.WriteLine("{0} entry {1} was created on {2:D}", entryType, fsi.FullName, fsi.CreationTime);
            }
        }

        #endregion

        #region Result to database

        public void scanFiles(string sourcePath, OracleConnection connection)
        {
            //  Loop through all the files in C.
            foreach (string entry in Directory.GetFiles(sourcePath))
            {
                DisplayFileSystemInfoAttributes(new FileInfo(entry), connection);
            }
        }

        public void scanSubdirectories(string sourcePath, OracleConnection connection)
        {
            //  Loop through all the immediate subdirectories of C.
            foreach (string entry in Directory.GetDirectories(sourcePath))
            {
                DisplayFileSystemInfoAttributes(new DirectoryInfo(entry), connection);
            }
        }

        static void DisplayFileSystemInfoAttributes(FileSystemInfo fsi, OracleConnection connection)
        {
            insert(fsi, connection);
        }

        #endregion

        private static void insert(FileSystemInfo fsi, OracleConnection connection)
        {
            OracleCommand cmd = connection.CreateCommand();
            string statement = "INSERT INTO SINGLE_SCANS(FILE_NAME, FILE_EXTENSION, FILE_PATH, LAST_ACCESS, LAST_WRITE, CREATION_DATE)" +
                              $" VALUES('{fsi.Name}', '{fsi.Extension}', '{fsi.FullName}', TO_DATE('{fsi.LastAccessTime}', 'yyyy/mm/dd hh24:mi:ss'), " +
                              $"TO_DATE('{fsi.LastWriteTime}', 'yyyy/mm/dd hh24:mi:ss'), TO_DATE('{fsi.CreationTime}', 'yyyy/mm/dd hh24:mi:ss'))";
            cmd.CommandText = statement;
            cmd.CommandType = CommandType.Text;
            
            try
            {
                int n = cmd.ExecuteNonQuery();
                
                if(n > 0)
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
