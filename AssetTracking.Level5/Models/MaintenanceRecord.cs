namespace AssetTracking.Level5.Models;

// Records a completed maintenance visit and the next planned date.
public class MaintenanceRecord
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public DateTime LastMaintenanceDate { get; set; }

    public DateTime NextMaintenanceDate { get; set; }

    public string Notes { get; set; } = "";
}