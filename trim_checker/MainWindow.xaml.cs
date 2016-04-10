using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using TrimRes;

namespace trim_checker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum FsutilCheckType
        {
            CheckTrim,
            EnableTrim,
            DisableTrim
        };

        private TrimCheckerResourceDictionary trimCheckerResDic;


        public MainWindow()
        {
            this.trimCheckerResDic = new TrimCheckerResourceDictionary();
            this.trimCheckerResDic.InitializeComponent();
            InitializeComponent();
        }

        /// <summary>
        /// Create the process structure based on the operation 
        /// we want to perform.
        /// </summary>
        /// <param name="typeCheck"></param>
        /// <returns></returns>
        private Process CreateFsutilProc(FsutilCheckType typeCheck)
        {
            var fsproc = new Process
            {
                StartInfo =
                {
                    FileName = "fsutil.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            switch (typeCheck)
            {
                case FsutilCheckType.CheckTrim:
                {    
                    fsproc.StartInfo.Arguments = "behavior query disabledeletenotify";
                    break;   
                }
                case FsutilCheckType.EnableTrim:
                {
                    fsproc.StartInfo.Arguments = "behavior set disabledeletenotify 0";
                    break;
                }
                case FsutilCheckType.DisableTrim:
                {
                    fsproc.StartInfo.Arguments = "behavior set disabledeletenotify 1";
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCheck), typeCheck, null);
            }

            return fsproc;
        }

        /// <summary>
        /// Check if trim is enabled or not.
        /// </summary>
        /// <returns></returns>
        private bool CheckTrimFunctionality()
        {
            var fsproc = CreateFsutilProc(FsutilCheckType.CheckTrim);
            fsproc.Start();
            var output = fsproc.StandardOutput.ReadToEnd();
            fsproc.WaitForExit();

            if (!output.Contains("DisableDeleteNotify"))
            {
                throw new InvalidProgramException("Error occurred while calling fsutil.exe, clue: " + output);
            }

            return output.Contains("DisableDeleteNotify = 0");
        }

        /// <summary>
        /// Tries to enable the trim functionality on this system.
        /// </summary>
        /// <returns></returns>
        private bool EnableTrimFunctionality()
        {
            var fsproc = CreateFsutilProc(FsutilCheckType.EnableTrim);
            fsproc.Start();
            var output = fsproc.StandardOutput.ReadToEnd();
            fsproc.WaitForExit();

            if (!output.Contains("DisableDeleteNotify"))
            {
                throw new InvalidProgramException("Error occurred while calling fsutil.exe, clue: " + output);
            }

            return output.Contains("DisableDeleteNotify = 0");
        }

        /// <summary>
        /// Tries to disable the trim functionality on this system.
        /// </summary>
        /// <returns></returns>
        private bool DisableTrimFunctionality()
        {
            var fsproc = CreateFsutilProc(FsutilCheckType.DisableTrim);
            fsproc.Start();
            var output = fsproc.StandardOutput.ReadToEnd();
            fsproc.WaitForExit();

            if (!output.Contains("DisableDeleteNotify"))
            {
                throw new InvalidProgramException("Error occurred while calling fsutil.exe, clue: " + output);
            }

            return output.Contains("DisableDeleteNotify = 1");
        }

        /// <summary>
        /// Button event that spawns the check trim function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_check_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckTrimFunctionality())
                {
                    result_text.Content = "Trim is already enabled.";
                    result_image.Source = (ImageSource)trimCheckerResDic["OK_iconDrawingImage"];
                }
                else
                {
                    result_text.Content = "Trim is not enabled, try enabling it!";
                    result_image.Source = (ImageSource)trimCheckerResDic["Not_OK_iconDrawingImage"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception caught", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Button event that spawns the enable trim function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_enable_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnableTrimFunctionality())
                {
                    result_text.Content = "Trim has been enabled successfully!";
                    result_image.Source = (ImageSource) trimCheckerResDic["OK_iconDrawingImage"];
                }
                else
                {
                    result_text.Content = "Could not enable trim";
                    result_image.Source = (ImageSource) trimCheckerResDic["Not_OK_iconDrawingImage"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception caught", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Button event that spawns the disable trim function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_disable_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DisableTrimFunctionality())
                {
                    result_text.Content = "Trim has been disabled successfully!";
                    result_image.Source = (ImageSource)trimCheckerResDic["OK_iconDrawingImage"];
                }
                else
                {
                    result_text.Content = "Could not disable trim";
                    result_image.Source = (ImageSource)trimCheckerResDic["Not_OK_iconDrawingImage"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception caught", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
