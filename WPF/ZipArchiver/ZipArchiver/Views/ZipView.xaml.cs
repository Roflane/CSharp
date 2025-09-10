using System.Windows;
using System.Windows.Controls;
using ZipArchiver.Core;
using ZipArchiver.Managers;
using ZipArchiver.ViewModels;

namespace ZipArchiver.Views;

public partial class ZipView : UserControl {
    public ZipView() {
        InitializeComponent();
    }
    
    private void ButtonArchive_OnClick(object sender, RoutedEventArgs e) {
        if (DataContext is ZipViewModel viewModel) {
            try {
                var progressWindow = new ProgressBarView();
                ArchiveManager.AddArchive();
                progressWindow.Show();

                var progress = new Progress<double>(value => {
                    progressWindow.UpdateProgress(value);
                });

                Thread th = new Thread(() => {
                    try {
                        progressWindow._viewModel.Count = ArchiveManager.ActiveArchives;
                        ZipArchiverCore.ArchiveAsync(
                            viewModel.ZipSourcePath,
                            viewModel.ZipDestinationPath,
                            progress);

                        Application.Current.Dispatcher.Invoke(() => {
                            ArchiveManager.RemoveArchive();
                            progressWindow.Close();
                        });
                    }
                    catch (Exception ex) {
                        Application.Current.Dispatcher.Invoke(() => {
                            ArchiveManager.RemoveArchive();
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            progressWindow.Close();
                        });
                    }
                });

                th.IsBackground = true;
                th.Start();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}