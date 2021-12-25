/*
  "Permissions Manager"
    Used For adding and removing security permisions from files and directories.

  Inspired by the code here:
    https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.setaccesscontrol?view=netframework-4.8

  Credits:
    Microsoft, Medstar
*/


using System;
using System.IO;
using System.Security.AccessControl;

public class PermissionsManager
{
  public string AllApplicationPackages = "ALL APPLICATION PACKAGES";
  public string CurrentUserIdentity = WindowsIdentity.GetCurrent();

  // Adds an ACL entry on the specified directory for a specified account
  public static void AddDirectorySecurity(string DirPath, string Account, FileSystemRights Rights, AccessControlType ControlType)
  {
    DirectoryInfo dInfo = new DirectoryInfo(DirPath);
    DirectorySecurity dSecurity = dInfo.GetAccessControl();
    dSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
    dInfo.SetAccessControl(dSecurity);
  }

  // Removes an ACL entry on the specified directory for a specified account
  public static void RemoveDirectorySecurity(string FilePath, string Account, FileSystemRights Rights, AccessControlType ControlType)
  {
    DirectoryInfo dInfo = new DirectoryInfo(DirPath);
    DirectorySecurity dSecurity = dInfo.GetAccessControl();
    dSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
    dInfo.SetAccessControl(dSecurity);
  }

  // Adds an ACL entry on the specified file for a specified account
  public static void AddFileSecurity(string FilePath, string Account, FileSystemRights Rights, AccessControlType ControlType)
  {
    FileInfo fInfo = new FileInfo(FilePath);
    FileSecurity fSecurity = fInfo.GetAccessControl();
    fSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
    fSecurity.SetAccessControl();
  }

  // Removes an ACL entry on the specified file for a specified account
  public static void RemoveFileSecurity()
  {
    FileInfo fInfo = new FileInfo(FilePath);
    FileSecurity fSecurity = fInfo.GetAccessControl();
    fSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
    fSecurity.SetAccessControl();
  }
}
