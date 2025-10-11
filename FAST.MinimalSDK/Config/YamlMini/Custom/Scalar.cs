namespace FAST.Config.YamlMini.Grammar
{
    public partial class Scalar : DataItem
    {
        public Scalar()
        {
            this.Text = String.Empty;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
