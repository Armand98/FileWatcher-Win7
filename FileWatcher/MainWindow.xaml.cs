using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Collections.Generic;

namespace FileWatcher
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OracleConnection connection = null;
        Watcher watcher = null;
        int i;

        public MainWindow()
        {
            this.setConnection();
            InitializeComponent();
            watcher = new Watcher();
            i = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startupSettings();
            updateMonitorDataGrid();
        }

        #region My Functions

        private void setConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            connection = new OracleConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch(Exception exp)
            {
                throw;
            }
        }

        private void updateMonitorDataGrid()
        {
            string statementForColumns = getSqlCommandForColumns();
            string whereStatement = getSqlWhereStatementForOperationAttribute();
            string dateStatement = getSqlWhereStatementForDateAttribute();
            string finalStatement = "SELECT " + statementForColumns + " FROM MONITORING ";
            bool skip = false;

            if (statementForColumns.Equals("*"))
            {
                finalStatement += "WHERE 1 = 0";
                skip = true;
            }
            else if (OperationColumn.IsChecked.Value == false && !statementForColumns.Equals("*"))
            {
                disableDataOperationAttributeFilters();
            }
            else
            {
                enableDataOperationAttributeFilters();
            }

            if(!skip)
            {
                if (!whereStatement.Equals(""))
                    finalStatement += "WHERE" + whereStatement + " AND CHANGE_DATE" + dateStatement;
                else
                    finalStatement += "WHERE CHANGE_DATE" + dateStatement;
            }

            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = finalStatement;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            MonitoringDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void updateScansDataGrid()
        {
            string statementForColumns = getSqlCommandForColumns();
            string whereStatement = getSqlWhereStatementForOperationAttribute();
            string dateStatement = getSqlWhereStatementForDateAttribute();
            string finalStatement = "SELECT " + statementForColumns + " FROM SINGLE_SCANS ";

            if (statementForColumns.Equals("*"))
            {
                finalStatement += "WHERE 1 = 0";
            }
            else if (!whereStatement.Equals(""))
                finalStatement += "WHERE" + whereStatement + " AND CREATION_DATE" + dateStatement + " OR LAST_ACCESS" + dateStatement + " OR LAST_WRITE" + dateStatement;
            else
                finalStatement += "WHERE CREATION_DATE" + dateStatement + " OR LAST_ACCESS" + dateStatement + " OR LAST_WRITE" + dateStatement;

            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = finalStatement;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            ScansDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private string getSqlWhereStatementForOperationAttribute()
        {
            string created = "",
                   renamed = "",
                   deleted = "",
                   changed = "";

            if (DatabaseCreatedCheckbox.IsChecked.Value) created = "'Created',";
            if (DatabaseRenamedCheckbox.IsChecked.Value) renamed = "'Renamed',";
            if (DatabaseDeletedCheckbox.IsChecked.Value) deleted = "'Deleted',";
            if (DatabaseChangedCheckbox.IsChecked.Value) changed = "'Changed'";

            string statement = $"{created}{renamed}{deleted}{changed}";

            if (statement != "")
                statement = " OPERATION IN (" + statement.TrimEnd(',') + ")";

            return statement;
        }

        private string getSqlWhereStatementForDateAttribute()
        {
            string fromDate = FromDatePickerBox.SelectedDate.Value.ToShortDateString().Replace('-','/');
            string toDate = ToDatePickerBox.SelectedDate.Value.AddDays(1).ToShortDateString().Replace('-', '/');

            string statement = $" BETWEEN '{fromDate}' AND '{toDate}'";

            return statement;
        }

        private string getSqlCommandForColumns()
        {
            string oldFileName = "",
                   fileName = "",
                   oldFilePath = "",
                   filePath = "",
                   operation = "",
                   changeDate = "",
                   fileExtension = "",
                   lastAccess = "",
                   lastWrite = "",
                   creationDate = "";

            if (OldFileNameColumn.IsChecked.Value) oldFileName = "OLD_FILE_NAME AS \"Old File Name\",";
            if (FileNameColumn.IsChecked.Value) fileName = "FILE_NAME AS \"File Name\",";
            if (OldFilePathColumn.IsChecked.Value) oldFilePath = "OLD_FILE_PATH AS \"Old File Path\",";
            if (FilePathColumn.IsChecked.Value) filePath = "FILE_PATH AS \"File Path\",";
            if (OperationColumn.IsChecked.Value) operation = "OPERATION AS \"Operation\",";
            if (ChangeDateColumn.IsChecked.Value) changeDate = "CHANGE_DATE AS \"Change Date\",";
            if (FileExtensionColumn.IsChecked.Value) fileExtension = "FILE_EXTENSION AS \"File Extension\",";
            if (LastAccessColumn.IsChecked.Value) lastAccess = "TO_CHAR(LAST_ACCESS,'yyyy/mm/dd hh24:mi:ss') AS \"Last Access\",";
            if (LastWriteColumn.IsChecked.Value) lastWrite = "TO_CHAR(LAST_WRITE,'yyyy/mm/dd hh24:mi:ss') AS \"Last Write\",";
            if (CreationDateColumn.IsChecked.Value) creationDate = "TO_CHAR(CREATION_DATE,'yyyy/mm/dd hh24:mi:ss') AS \"Creation Date\",";

            string statement = $"{oldFileName}{fileName}{oldFilePath}{filePath}" +
                                    $"{operation}{changeDate}{fileExtension}{lastAccess}" +
                                    $"{lastWrite}{creationDate}";

            if (statement == "") statement = "*";
            else statement = "ID, " + statement.TrimEnd(',');

            return statement;
        }

        private void startupSettings()
        {
            FromDatePickerBox.SelectedDate = DateTime.Parse("2019-01-01");
            ToDatePickerBox.SelectedDate = DateTime.Today;

            FileTypeComboBox.Items.Add("*.*");
            FileTypeComboBox.Items.Add("*.txt");
            FileTypeComboBox.Items.Add("*.pdf");
            FileTypeComboBox.Items.Add("*.exe");
            FileTypeComboBox.Items.Add("*.dll");
            FileTypeComboBox.SelectedIndex = 0;

            Created.IsChecked = true;
            Deleted.IsChecked = true;
            Renamed.IsChecked = true;
            Changed.IsChecked = true;

            SaveToDatabaseCheckBox.IsChecked = true;
            Stop.IsEnabled = false;
            setColumnFiltersForMonitoringLogs();
        }

        private void setUniversalColumnFiltersForLogsChecked()
        {
            FileNameColumn.IsChecked = true;
            FilePathColumn.IsChecked = true;
        }

        private void setColumnFiltersForMonitoringLogsChecked()
        {
            OldFilePathColumn.IsChecked = true;
            OldFileNameColumn.IsChecked = true;
            OperationColumn.IsChecked = true;
            OperationColumn.IsChecked = true;
            ChangeDateColumn.IsChecked = true;
        }

        private void setColumnFiltersForScanLogsChecked()
        {
            FileExtensionColumn.IsChecked = true;
            LastAccessColumn.IsChecked = true;
            LastWriteColumn.IsChecked = true;
            CreationDateColumn.IsChecked = true;
        }

        private void setColumnFiltersForMonitoringLogs()
        {
            FileExtensionColumn.IsChecked = false;
            FileExtensionColumn.IsEnabled = false;
            LastAccessColumn.IsChecked = false;
            LastAccessColumn.IsEnabled = false;
            LastWriteColumn.IsChecked = false;
            LastWriteColumn.IsEnabled = false;
            CreationDateColumn.IsChecked = false;
            CreationDateColumn.IsEnabled = false;
            OldFilePathColumn.IsEnabled = true;
            OldFileNameColumn.IsEnabled = true;
            OperationColumn.IsEnabled = true;
            OperationColumn.IsEnabled = true;
            ChangeDateColumn.IsEnabled = true;

            enableDataOperationAttributeFilters();
        }

        private void setColumnFiltersForScanLogs()
        {
            FileExtensionColumn.IsEnabled = true;
            LastAccessColumn.IsEnabled = true;
            LastWriteColumn.IsEnabled = true;
            CreationDateColumn.IsEnabled = true;
            OldFilePathColumn.IsChecked = false;
            OldFilePathColumn.IsEnabled = false;
            OldFileNameColumn.IsChecked = false;
            OldFileNameColumn.IsEnabled = false;
            OperationColumn.IsChecked = false;
            OperationColumn.IsEnabled = false;
            ChangeDateColumn.IsChecked = false;
            ChangeDateColumn.IsEnabled = false;

            disableDataOperationAttributeFilters();
        }

        private void enableDataOperationAttributeFilters()
        {
            DatabaseCreatedCheckbox.IsEnabled = true;
            DatabaseRenamedCheckbox.IsEnabled = true;
            DatabaseDeletedCheckbox.IsEnabled = true;
            DatabaseChangedCheckbox.IsEnabled = true;
        }

        private void disableDataOperationAttributeFilters()
        {
            DatabaseCreatedCheckbox.IsChecked = false;
            DatabaseCreatedCheckbox.IsEnabled = false;
            DatabaseRenamedCheckbox.IsChecked = false;
            DatabaseRenamedCheckbox.IsEnabled = false;
            DatabaseDeletedCheckbox.IsChecked = false;
            DatabaseDeletedCheckbox.IsEnabled = false;
            DatabaseChangedCheckbox.IsChecked = false;
            DatabaseChangedCheckbox.IsEnabled = false;
        }

        #endregion

        #region UI Events

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string strPath = PathTextBox.Text;

            if (strPath == "")
            {
                PathTextBox.Text = @"C:\";
                return;
            }

            string strFileType = FileTypeComboBox.SelectedValue.ToString();
            bool includeSubdirectories = (bool)IncludeSubDirs.IsChecked;
            bool created = (bool)Created.IsChecked;
            bool deleted = (bool)Deleted.IsChecked;
            bool renamed = (bool)Renamed.IsChecked;
            bool changed = (bool)Changed.IsChecked;

            watcher.RunMonitorSystem(strPath, strFileType, includeSubdirectories, created, deleted, renamed, changed, connection);
            BrushConverter converter = new BrushConverter();
            Status.Content = "Running...";
            Status.Foreground = (Brush)converter.ConvertFromString("Green");
            Start.IsEnabled = false;
            Stop.IsEnabled = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            watcher.StopMonitorSystem();
            BrushConverter converter = new BrushConverter();
            Status.Content = "STOP";
            Status.Foreground = (Brush)converter.ConvertFromString("Red");
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "Select Folder";
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void SaveToDatabaseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReportDestinationPathTextBox.IsEnabled = false;
        }

        private void SaveToDatabaseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReportDestinationPathTextBox.IsEnabled = true;
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            string strSourcePath = ReportSourcePathTextBox.Text;

            if (SaveToDatabaseCheckBox.IsChecked.Value)
            {
                if (strSourcePath == "")
                {
                    ReportSourcePathTextBox.Text = @"C:\";
                    return;
                }

                FileScanner Fs = new FileScanner();

                Fs.scanSubdirectories(strSourcePath, connection);
                Fs.scanFiles(strSourcePath, connection);

                updateScansDataGrid();
                System.Windows.MessageBox.Show($"Log saved to database", "Scan completed!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string strDestinationPath = ReportDestinationPathTextBox.Text;
                if (strSourcePath == "")
                {
                    ReportSourcePathTextBox.Text = @"C:\";
                    return;
                }

                if (strDestinationPath == "")
                {
                    ReportDestinationPathTextBox.Text = @"C:\";
                    return;
                }

                FileScanner Fs = new FileScanner();
                Fs.scanSubdirectories(strSourcePath, strDestinationPath);
                Fs.scanFiles(strSourcePath, strDestinationPath);

                System.Windows.MessageBox.Show($"Log saved to {strDestinationPath}", "Scan completed!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow newWindow = new HelpWindow();
            newWindow.ShowDialog();
        }

        private void BrowseButton2_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "Select Folder";
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReportSourcePathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void BrowseButton3_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "Select Folder";
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReportDestinationPathTextBox.Text = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// If i (mod2) equals 0, it means that user has chosen ScanLogs Tab
        /// If i (mod2) equals 1, it means that user has chosen MonitoringLogs Tab
        /// Every Tab change increments the i integer value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (i % 2 == 0)
            {
                setColumnFiltersForScanLogs();
                setColumnFiltersForScanLogsChecked();
                setUniversalColumnFiltersForLogsChecked();
                updateScansDataGrid();
                CreateReportButton.IsEnabled = true;
            }
            else
            {
                setColumnFiltersForMonitoringLogs();
                setColumnFiltersForMonitoringLogsChecked();
                setUniversalColumnFiltersForLogsChecked();
                CreateReportButton.IsEnabled = false;
            }
            i++;
        }

        #region SQL SELECT REFRESH

        private void OldFileNameColumn_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void FileNameColumn_Click(object sender, RoutedEventArgs e)
        {
            if (i % 2 == 1)
                updateScansDataGrid();
            else
                updateMonitorDataGrid();

        }

        private void FileExtensionColumn_Click(object sender, RoutedEventArgs e)
        {
            updateScansDataGrid();
        }

        private void FilePathColumn_Click(object sender, RoutedEventArgs e)
        {
            if (i % 2 == 1)
                updateScansDataGrid();
            else
                updateMonitorDataGrid();
        }

        private void OldFilePathColumn_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void CreationDateColumn_Click(object sender, RoutedEventArgs e)
        {
            updateScansDataGrid();
        }

        private void ChangeDateColumn_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void OperationColumn_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void LastWriteColumn_Click(object sender, RoutedEventArgs e)
        {
            updateScansDataGrid();
        }

        private void LastAccessColumn_Click(object sender, RoutedEventArgs e)
        {
            updateScansDataGrid();
        }

        private void DatabaseCreatedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void DatabaseRenamedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void DatabaseDeletedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        private void DatabaseChangedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            updateMonitorDataGrid();
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            connection.Close();
        }

        #endregion

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (i % 2 == 1)
                updateScansDataGrid();
            else
                updateMonitorDataGrid();
        }
    }
}
