using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SuccessCafePOS
{
    public class Backup
    {
        void backupdb(string filepath, string srcfilename, string destfilename)
        {
            var srcfile = Path.Combine(filepath, srcfilename);
            var destfile = Path.Combine(filepath, destfilename);
            //  if (File.Exists(destfile)) File.Delete(destfile);
            File.Copy(srcfile, destfile, true);
        }

        void restoredb(string filepath, string srcfilename, string destfilename, bool iscopy = false)
        {
            var srcfile = Path.Combine(filepath, srcfilename);
            var destfile = Path.Combine(filepath, destfilename);

            if (iscopy)
                backupdb(filepath, srcfilename, destfilename);
            else
                File.Move(srcfile, destfile);
        }

        public void backup()
        {
            string pickdatabasefrom = Environment.CurrentDirectory;
            var dbname = "SuccessCafePOS.db";


            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "SuccessCafePOS" + DateTime.Now.ToString("ddMMyyyy") + ".db";


            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(Path.GetDirectoryName(sfd.FileName)))
            {
                var backupdatabaseto = Path.GetDirectoryName(sfd.FileName) + "\\" + (Path.GetFileNameWithoutExtension(sfd.FileName) + ".db");
                backupdb(pickdatabasefrom, dbname, backupdatabaseto);

                MessageBox.Show("Backup complete successfully.");
            }
        }

        public void restore()
        {
            string restoredatabasefrom = string.Empty;
            var dbname = "SuccessCafePOS.db";
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var olddb = Path.GetDirectoryName(dialog.FileName) + "\\" + (Path.GetFileNameWithoutExtension(dialog.FileName) + ".db");
                var newdb = Path.Combine(Path.GetDirectoryName(dialog.FileName), dbname);


                File.Copy(olddb, newdb, true);
                restoredatabasefrom = Path.GetDirectoryName(dialog.FileName);
                var restoredatabaseto = Environment.CurrentDirectory + "\\" + dbname;
                restoredb(restoredatabasefrom, dbname, restoredatabaseto, true);
                MessageBox.Show("Restore complete successfully.");
                File.Delete(newdb);
            }
        }
    }
}
