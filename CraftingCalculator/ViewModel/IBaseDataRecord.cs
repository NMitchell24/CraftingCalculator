namespace CraftingCalculator.ViewModel
{
    public interface IBaseDataRecord
    {
        int Id { get; set; }
        string? Name { get; set; }
        string? Description { get; set; }
        string Tooltip { get; set; }
        DataType Type { get; set; }

        IBaseDataRecord Clone();
        IBaseDataRecord CopyForSave();
    }
}
