namespace Domain.Attributes
{
    public class ComputedColumnsAttribute : Attribute
    {
        string[] _columns = new string[0];

        public ComputedColumnsAttribute(params string[] columns)
        {
            _columns = columns;
        }

        public virtual string[] Columns
        {
            get { return _columns; }
        }
    }
}
