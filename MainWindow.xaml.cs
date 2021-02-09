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
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace ZeneLejatszo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private MediaPlayer MediaPlayer = new MediaPlayer();
        List<string> ut = new List<string>();
        StreamWriter sw;
        public void MediaPlayerS()
        {
            InitializeComponent();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                MediaPlayer.Open(new Uri(openFileDialog.FileName));
                ListBoxItem item = new ListBoxItem();
                var name = openFileDialog.FileName;
                var safeName = openFileDialog.SafeFileName;
                
                ut.Add(Convert.ToString(name));

                Lista.Items.Add(safeName);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if ((MediaPlayer.Source != null) && (MediaPlayer.NaturalDuration.HasTimeSpan))
            {
                ZenPro.Minimum = 0;
                ZenPro.Maximum = Convert.ToDouble(MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                ZenPro.Value = Convert.ToDouble(MediaPlayer.Position.TotalSeconds);
                //Ez az ismétlés
                if (MediaPlayer.Position.TotalSeconds == MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds)
                {
                    MediaPlayer.Position = TimeSpan.FromSeconds(0);
                }
            }

        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
           if (Lista.SelectedItems.Count != 0)
           {
                MediaPlayer.Open(new Uri(ut[Lista.SelectedIndex]));
                MediaPlayer.Play();
            }

                MediaPlayer.Play();

                
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
                MediaPlayer.Pause();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Lista.SelectedItems.Count != 0)
                {
                    MediaPlayer.Open(new Uri(ut[Lista.SelectedIndex - 1]));
                    ZenPro.Minimum = 0;
                    ZenPro.Maximum = Convert.ToDouble(MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                    ZenPro.Value = Convert.ToDouble(MediaPlayer.Position.TotalSeconds);
                }
                else
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Play();
                }
                MediaPlayer.Play();
            }
            catch(Exception)
            {
                MessageBox.Show("nyomja meg megegyszer léc kösz");
                return;

            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Lista.SelectedItems.Count != 0)
                {
                    if (Lista.SelectedIndex == Lista.Items.Count)
                    {
                        MediaPlayer.Open(new Uri(ut[0]));
                        ZenPro.Minimum = 0;
                        ZenPro.Maximum = Convert.ToDouble(MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                        ZenPro.Value = Convert.ToDouble(MediaPlayer.Position.TotalSeconds);
                    }
                    else
                        MediaPlayer.Open(new Uri(ut[Lista.SelectedIndex + 1]));

                }
                MediaPlayer.Play();
            }
            catch(Exception)
            {
                MessageBox.Show("nem jou");
                return;
               
            }

        }

        private void Hoz_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerS();
            //Lista.Items.Add();
        }

        private void ZenPro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Jelen.Content = TimeSpan.FromSeconds(ZenPro.Value).ToString(@"hh\:mm\:ss");
            MediaPlayer.Position = TimeSpan.FromSeconds(ZenPro.Value);

        }
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = Hangero.Value;
        }
        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           /* MediaPlayer.Stop();
            MediaPlayer.Open(new Uri(Convert.ToString(ut[Lista.SelectedIndex])));
            MediaPlayer.Play();*/
        }

        private void elt_Click(object sender, RoutedEventArgs e)
        {
            if (Lista.SelectedItems.Count != 0)
            {
                while (Lista.SelectedIndex != -1)
                {
                    Lista.Items.RemoveAt(Lista.SelectedIndex);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sfd = new SaveFileDialog();
                sfd.CreatePrompt = false;
                sfd.OverwritePrompt = true;
                sfd.Filter = "Zenelista (*.zl) | *.zl";
                sfd.ShowDialog();
                sw = new StreamWriter(sfd.FileName);
                for (int i = 0; i < ut.Count; i++)
                {
                    sw.WriteLine($"{ut[i]};{Lista.Items[i]}");

                }
                sw.Close();
            }
            catch(Exception)
            {
                return;
            }

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Zenelista (*.zl) | *.zl";
            ofd.ShowDialog();
            StreamReader sr = new StreamReader(ofd.FileName);
            ut.Clear();
            Lista.Items.Clear();
            while (!sr.EndOfStream)
            {
                var zenek = sr.ReadLine().Split(';');
                ut.Add(zenek[0]);
                Lista.Items.Add(zenek[1]);
            }
            sr.Close();
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (MessageBox.Show("Biztos ki akarsz lépni?", "", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                Environment.Exit(0);
        }
    }
}
