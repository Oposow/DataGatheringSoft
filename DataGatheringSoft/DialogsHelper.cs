using System;
using System.Windows.Forms;

namespace DataGatheringSoft
{
    public static class DialogsHelper
    {
        public static string GetDirectory()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }

            return String.Empty;
        }
    }
}
