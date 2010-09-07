using RefreshCache.Packager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RefreshCache.Packager.Builder
{
    public partial class mainForm : Form
    {
        /// <summary>
        /// Initialize all the user interface elements on the Files tab.
        /// </summary>
        private void InitFilesTab()
        {
            //
            // Setup all the tab actions and events.
            //
            dgFiles.VirtualMode = true;
            dgFiles.CellValueNeeded += new DataGridViewCellValueEventHandler(dgFiles_CellValueNeeded);
            dgFiles.CellValuePushed += new DataGridViewCellValueEventHandler(dgFiles_CellValuePushed);
            dgFiles.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgFiles_UserDeletingRow);
            tbFiles_MigrationSource.Validated += new EventHandler(tbFiles_MigrationSource_Validated);
        }


        /// <summary>
        /// Update the user interface elements on the Files tab to apply
        /// the changes of a new package being loaded into memory.
        /// </summary>
        private void UpdateFilesTab()
        {
            dgFiles.RowCount = 1;
            dgFiles.RowCount = package.Files.Count + 1;

            tbFiles_MigrationSource.Text = package.MigrationSource;
        }


        #region User Interface Events and Actions

        /// <summary>
        /// A value from the Files datagrid is needed. Find the file
        /// on that row and retrieve the value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgFiles_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            File file;


            if (e.RowIndex == package.Files.Count)
            {
                return;
            }
            file = package.Files[e.RowIndex];

            if (e.ColumnIndex == 0)
                e.Value = file.Path;
            else if (e.ColumnIndex == 1)
                e.Value = file.Source;
        }

        
        /// <summary>
        /// User has entered a value in a cell that needs to be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgFiles_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            File file;


            if (e.RowIndex == package.Files.Count)
            {
                package.Files.Add(new File());
            }
            file = package.Files[e.RowIndex];

            if (e.ColumnIndex == 0)
                file.Path = e.Value.ToString();
            else if (e.ColumnIndex == 1)
                file.Source = e.Value.ToString();
        }


        /// <summary>
        /// User has requested that a row be deleted. It will be removed from
        /// the datagrid after this function returns.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgFiles_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            package.Files.RemoveAt(e.Row.Index);
        }


        /// <summary>
        /// The user has changed the Migration Source value on the Files
        /// tab, it has been validated and can be stored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbFiles_MigrationSource_Validated(object sender, EventArgs e)
        {
            package.MigrationSource = tbFiles_MigrationSource.Text;
        }

        #endregion

    }
}
