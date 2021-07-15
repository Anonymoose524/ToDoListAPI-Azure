using Microsoft.Azure.Cosmos.Table;
public class TodoItem : TableEntity
{
    public long IdNum { get; set; }
    public string Name { get; set; }
}