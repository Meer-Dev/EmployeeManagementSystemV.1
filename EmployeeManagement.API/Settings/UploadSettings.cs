// EmployeeManagement.API/Settings/UploadSettings.cs
namespace EmployeeManagement.API.Settings;

public class UploadSettings
{
    public string TempPath { get; set; } = "uploads/temp";
    public int MaxFileSizeMb { get; set; } = 50;
    public string[] AllowedExtensions { get; set; } = [".xlsx"];
}