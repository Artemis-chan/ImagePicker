using System.Windows.Forms;
using System.Reflection;
public static class Extensions
{
    public static void SetDoubleBuffer(this Control control, bool set)
    {
        control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(control, set, null);
    }
}