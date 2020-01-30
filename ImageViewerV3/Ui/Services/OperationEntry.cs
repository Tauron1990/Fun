namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationEntry
    {
        private string Name { get; }

        public int Id { get; }

        public OperationEntry(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString() 
            => $"{Id}: {Name}";
    }
}