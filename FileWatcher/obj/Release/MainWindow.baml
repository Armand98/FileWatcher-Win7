   M S B A M L   `   `   `  ���� F  BFileWatcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:FileWatcherFileWatcher      FileWatcher.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:FileWatcher     -     � Title$  File Watcher��$	��700��$
��1200��.����  �� Padding$ 10q�._��� .+��  � ColumnDefinitions �� $��* ��� $��5* ��� $��* �.+��� $��0��$��Path: ��$��Right=�$��Center=�$��15(��� -    ��Name$ PathTextBox��$��1��$	��C:\��$��Stretch=�$��Center=�$��15(��� -    ��Name$ BrowseButton��$��2��$��Browse��$��Center=�$��Center=�$��15(�$	��100���� $��Stretch=�$��Center=�
����� �$��1��$��5 15 5 0q�� $��0 5 0 0q� �� $	��50* ��� $��* ��� $	��50* ��� $��* ��� $	��50* �.+��� $��0��$��Center=�.+��� $��Filters��$��15(�$��Center=�
����  �$
��Bold"��  �� $��* ��� $��* �.+��� $��0��$��File Type: ��$
��Left=�$��Center=�$��15(��� -    ��Name$ FileTypeComboBox��$��1��$��Right=�$��Center=�$��15(�$	��100���� -    ��Name$ IncludeSubDirs��$��Include Subdirectories��$��Stretch=�$��Center=�$��15(�
���  ���  �� $��* ��� $��* �.+��� -   $ Created��$��0��$��Created��$
��Left=�$��Center=�$��15(��� -   $ Renamed��$��1��$��Renamed��$
��Left=�$��Center=�$��15(��  �� $��* ��� $��* �.+��� -   $ Deleted��$��0��$��Deleted��$
��Left=�$��Center=�$��15(��� -	   $ Changed��$��1��$��Changed��$
��Left=�$��Center=�$��15(��� $��Stretch=�$��1��$��2q�
���   �$��1���� $��2��.+��� $��Start/Stop recording��$��Center=�$��15(�
����  �$
��Bold"��� -
   $ 
HelpButton��$��Info?/Help?��$��15(�$	��300���� -    ��Name$ Status��$
��STOP��$��Center=�$��20(�
���  ���  �� $��* ��� $��* �.+��� -   $ Start��$��0��$��Start��$��15(�$	��120���� -   $
 Stop��$��1��$
��Stop��$��15(�$	��120���� $��Stretch=�$��1��$��2q�
���   �$��3���� $��4��$��0 0 30 0q�.+��� $��Single scan options��$��Center=�
����  �$
��Bold"�$��15(��  �� $��3* ��� $��5* ��� $��2* �.+��� $��0��$��Source:��$��Right=�$��15(��� -   $ ReportSourcePathTextBox��$��1��$	��C:\��$��Stretch=�$��Center=�$��15(��� -   $ BrowseButton2��$��2��$��Browse��$��Center=�$��Center=�$��15(��  �� $��3* ��� $��5* ��� $��2* �.+��� $��0��$��Destination:��$��Right=�$��15(��� -   $" ReportDestinationPathTextBox��$��1��$	��C:\��$��Stretch=�$��Center=�$��15(��� -   $ BrowseButton3��$��2��$��Browse��$��Center=�$��Center=�$��15(��  �� $��* ��� $��* �.+��� -   $ SaveToDatabaseCheckBox��$��0��$��Save To Database��$��Center=�$��Center=�$��15(��� -   $ CreateReportButton��$��1��$��Create a report��$��Right=�$��Center=�$��15(��� $��Stretch=�$��Center=�
����� �$��1��$��5 5 5 0q��� $��Data filters:��$��15(�$��Black"�$��10 0 0 0q��  �� $	��50* ��� $��* ��� $	��50* ��� $��* ��� $
��100* �.+��� $��0��$��Center=�$��Center=�.+��� $��Operation type��$��Center=�$��15(�$
��Bold"�� $��5q� �� $��* ��� $��* �.+��� -   $ DatabaseCreatedCheckbox��$��0��$��Created��$��0 0 5 0q�$
��Left=�$��Center=�$��15(��� -   $ DatabaseRenamedCheckbox��$��1��$��Renamed��$��5 0 0 0q�$��Right=�$��Center=�$��15(�� $��5q� �� $��* ��� $��* �.+��� -   $ DatabaseDeletedCheckbox��$��0��$��Deleted��$��0 0 5 0q�$
��Left=�$��Center=�$��15(��� -   $ DatabaseChangedCheckbox��$��1��$��Changed��$��5 0 0 0q�$��Right=�$��Center=�$��15(��� $��1��$��Stretch=�$��1��$��2q�
���   ��� $��2��$��Center=�$��Center=�.+��� $,��&Select time duration between two dates��$��Center=�$��15(�$
��Bold"�� $��5q� �� $��* ��� $��* �.+��� $��0��$��From:��$��Right=�$��Center=�(  "System.Windows.Controls.DatePicker  -     Name$ FromDatePickerBox��$��1��$	��120��� $��5q� �� $��* ��� $��* �.+��� $��0��$	��To:��$��Right=�$��Center=�  -   $ ToDatePickerBox��$��1��$	��120���� -   $ RefreshButton��$��Select Date��$��15(��� $��3��$��Stretch=�$��1��$��2q�
���   ��� $��4��$��Center=�$��Center=�.+��� $��Database columns��$��Center=�$��15(�$
��Bold"��  �� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��5q�.+��� -   $ OldFileNameColumn��$��OldFileName��$
��Left=�$��Center=�$��15(��� -   $ FileNameColumn��$��FileName��$
��Left=�$��Center=�$��15(��� $��1��$��5q�.+��� -   $ FilePathColumn��$��FilePath��$
��Left=�$��Center=�$��15(��� -   $ OldFilePathColumn��$��OldFilePath��$
��Left=�$��Center=�$��15(��� $��2��$��5q�.+��� -   $ CreationDateColumn��$��CreationDate��$
��Left=�$��Center=�$��15(��� -    $ ChangeDateColumn��$��
ChangeDate��$
��Left=�$��Center=�$��15(��� $��3��$��5q�.+��� -!   $ OperationColumn��$��	Operation��$
��Left=�$��Center=�$��15(��� -"   $ FileExtensionColumn��$��FileExtension��$
��Left=�$��Center=�$��15(��� $��4��$��5q�.+��� -#   $ LastWriteColumn��$��	LastWrite��$
��Left=�$��Center=�$��15(��� -$   $ LastAccessColumn��$��
LastAccess��$
��Left=�$��Center=�$��15(��� -%   $��10q�.@��� -&   	 ��Name$	 MonitorLogsTabItem��$	��120��$��30��$��Monitoring Logs��.��&   System.Windows.Controls.DataGrid  -'   
  Name$
 MonitoringDataGrid��$��0��$��0��$��Center=�$	��Top=�$��	0 10 0 10q���. $
��Auto=�$
��Auto=�   
IsReadOnly . $	��250���� -(   $	 ScansTabItem��$	��120��$��30��$��Scans��.��  -)   $
 ScansDataGrid��$��0��$��0��$��Center=�$	��Top=�$��	0 10 0 10q���. $
��Auto=�$
��Auto=� . $	��250��