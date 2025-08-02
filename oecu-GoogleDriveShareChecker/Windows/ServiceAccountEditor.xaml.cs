using oecu_GoogleDriveShareChecker.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace oecu_GoogleDriveShareChecker.Windows
{
    /// <summary>
    /// ServiceAccountEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class ServiceAccountEditor : Window
    {
        public ServiceAccountEditorViewModel model = new ServiceAccountEditorViewModel();
        public ServiceAccountEditor()
        {
            InitializeComponent();

            model.EditorWindow = this;
            DataContext = model;
        }

        private void TbJsonText_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void TbJsonText_Drop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && File.Exists(files[0]))
            {
                try
                {
                    if (sender is TextBox)
                    {
                        TextBox tbx = (TextBox)sender;

                        string content = File.ReadAllText(files[0]);
                        tbx.Text = content;
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show("ファイル読み込みエラー: " + ex.Message);
                }
            }
        }
    }    
}
