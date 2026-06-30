namespace BookStore.Models.DTOs;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int Added { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}