using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Win32;
using NAudio.Wave;

namespace ReproductorAudio
{
    public partial class MainWindow : Window
    {
        private IWavePlayer wavePlayer;
        private AudioFileReader audioFileReader;
        private DispatcherTimer timer;
        private bool isDraggingSlider = false;
        private string currentYouTubeUrl = "";
        private string currentFilePath = "";
        private string currentVideoTitle = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFileReader != null && !isDraggingSlider)
            {
                sliderProgress.Maximum = audioFileReader.TotalTime.TotalSeconds;
                sliderProgress.Value = audioFileReader.CurrentTime.TotalSeconds;

                txtCurrentTime.Text = audioFileReader.CurrentTime.ToString(@"mm\:ss");
                txtTotalTime.Text = audioFileReader.TotalTime.ToString(@"mm\:ss");
            }
        }

        private async void BtnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.m4a;*.aac;*.wma;*.flac;*.webm;*.opus|All Files|*.*";
            openFileDialog.Title = "Selecciona un archivo de audio";

            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                currentYouTubeUrl = "";
                btnDownload.Visibility = Visibility.Collapsed;

                await LoadAudioFile(currentFilePath);
                txtCurrentSong.Text = Path.GetFileNameWithoutExtension(currentFilePath);
            }
        }

        private async void BtnLoadYouTube_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new YouTubeInputDialog();
            if (dialog.ShowDialog() == true)
            {
                string url = dialog.YouTubeUrl;
                if (string.IsNullOrWhiteSpace(url))
                {
                    MessageBox.Show("Por favor ingresa una URL válida", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                currentYouTubeUrl = url;
                txtStatus.Text = "🔄 Obteniendo información del video...";
                btnPlay.IsEnabled = false;
                btnDownload.IsEnabled = false;

                try
                {
                    // Verificar si yt-dlp existe
                    string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
                    if (!File.Exists(ytDlpPath))
                    {
                        MessageBox.Show("No se encontró yt-dlp.exe en la carpeta del programa.\n\nPor favor descarga yt-dlp.exe desde:\nhttps://github.com/yt-dlp/yt-dlp/releases\n\ny colócalo en la carpeta del programa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "❌ Falta yt-dlp.exe";
                        return;
                    }

                    // Obtener título del video
                    string title = await GetVideoTitle(url, ytDlpPath);
                    if (string.IsNullOrEmpty(title))
                    {
                        title = "Audio de YouTube";
                    }
                    currentVideoTitle = title;
                    txtCurrentSong.Text = title;

                    // Descargar audio
                    txtStatus.Text = "⬇️ Descargando audio desde YouTube...";
                    string tempPath = Path.Combine(Path.GetTempPath(), $"temp_audio_{Guid.NewGuid()}.m4a");

                    bool success = await DownloadYouTubeAudio(url, tempPath, ytDlpPath);

                    if (success && File.Exists(tempPath))
                    {
                        currentFilePath = tempPath;
                        await LoadAudioFile(tempPath);

                        btnDownload.Visibility = Visibility.Visible;
                        txtStatus.Text = "✅ Listo para reproducir";
                    }
                    else
                    {
                        MessageBox.Show("No se pudo descargar el audio del video.\n\nVerifica:\n• La URL sea válida\n• Tengas conexión a internet\n• El video no tenga restricciones", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "❌ Error al descargar";
                        txtCurrentSong.Text = "No hay ninguna canción cargada";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar desde YouTube:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtStatus.Text = "❌ Error al cargar";
                    txtCurrentSong.Text = "No hay ninguna canción cargada";
                }
                finally
                {
                    btnPlay.IsEnabled = true;
                    btnDownload.IsEnabled = true;
                }
            }
        }

        private async Task<string> GetVideoTitle(string url, string ytDlpPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ytDlpPath,
                        Arguments = $"--get-title \"{url}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return output.Trim();
            }
            catch
            {
                return "";
            }
        }

        private async Task<bool> DownloadYouTubeAudio(string url, string outputPath, string ytDlpPath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ytDlpPath,
                        Arguments = $"-f bestaudio -x --audio-format m4a -o \"{outputPath}\" \"{url}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                return process.ExitCode == 0 && File.Exists(outputPath);
            }
            catch
            {
                return false;
            }
        }

        private async Task LoadAudioFile(string filePath)
        {
            try
            {
                StopAudio();

                await Task.Run(() =>
                {
                    audioFileReader = new AudioFileReader(filePath);
                    wavePlayer = new WaveOutEvent();
                    wavePlayer.Init(audioFileReader);
                });

                wavePlayer.Volume = (float)(sliderVolume.Value / 100.0);
                txtStatus.Text = "✅ Archivo cargado";
                sliderProgress.Maximum = audioFileReader.TotalTime.TotalSeconds;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo:\n\n{ex.Message}\n\nAsegúrate de que el archivo sea un formato de audio válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "❌ Error al cargar";
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (wavePlayer == null)
            {
                MessageBox.Show("Por favor carga un archivo primero", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Pause();
                btnPlay.Content = "▶️";
                txtStatus.Text = "⏸️ Pausado";
                timer.Stop();
            }
            else
            {
                wavePlayer.Play();
                btnPlay.Content = "⏸️";
                txtStatus.Text = "▶️ Reproduciendo";
                timer.Start();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                audioFileReader.Position = 0;
                btnPlay.Content = "▶️";
                txtStatus.Text = "⏹️ Detenido";
                timer.Stop();
                sliderProgress.Value = 0;
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (audioFileReader != null)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(Math.Max(0, audioFileReader.CurrentTime.TotalSeconds - 10));
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (audioFileReader != null)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(Math.Min(audioFileReader.TotalTime.TotalSeconds, audioFileReader.CurrentTime.TotalSeconds + 10));
            }
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentYouTubeUrl))
            {
                MessageBox.Show("No hay ninguna canción de YouTube cargada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Audio M4A|*.m4a|Audio MP3|*.mp3|Todos los archivos|*.*";
            saveFileDialog.Title = "Guardar canción";
            saveFileDialog.FileName = SanitizeFileName(currentVideoTitle);

            if (saveFileDialog.ShowDialog() == true)
            {
                string oldStatus = txtStatus.Text;
                txtStatus.Text = "💾 Descargando para guardar...";
                btnDownload.IsEnabled = false;

                try
                {
                    string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                    string format = extension == ".mp3" ? "mp3" : "m4a";

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = ytDlpPath,
                            Arguments = $"-f bestaudio -x --audio-format {format} -o \"{saveFileDialog.FileName}\" \"{currentYouTubeUrl}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0 && File.Exists(saveFileDialog.FileName))
                    {
                        MessageBox.Show("✅ Canción descargada exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtStatus.Text = "✅ Descarga completada";
                    }
                    else
                    {
                        MessageBox.Show("No se pudo completar la descarga", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = oldStatus;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al descargar:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtStatus.Text = oldStatus;
                }
                finally
                {
                    btnDownload.IsEnabled = true;
                }
            }
        }

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (wavePlayer != null)
            {
                wavePlayer.Volume = (float)(sliderVolume.Value / 100.0);
            }

            if (txtVolume != null)
            {
                txtVolume.Text = $"{(int)sliderVolume.Value}%";
            }
        }

        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDraggingSlider && audioFileReader != null)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(sliderProgress.Value);
            }
        }

        private void StopAudio()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                wavePlayer.Dispose();
                wavePlayer = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }

            timer.Stop();
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Source == sliderProgress)
            {
                isDraggingSlider = true;
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            isDraggingSlider = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            StopAudio();
            base.OnClosed(e);
        }
    }

    // Diálogo para ingresar URL de YouTube
    public partial class YouTubeInputDialog : Window
    {
        public string YouTubeUrl { get; private set; }

        public YouTubeInputDialog()
        {
            Title = "Cargar desde YouTube";
            Width = 500;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            Background = new SolidColorBrush(Color.FromRgb(26, 26, 46));

            var grid = new Grid();
            grid.Margin = new Thickness(20);

            var stackPanel = new StackPanel();

            var label = new TextBlock
            {
                Text = "Ingresa la URL del video de YouTube:",
                FontSize = 14,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var textBox = new TextBox
            {
                Name = "txtUrl",
                FontSize = 14,
                Padding = new Thickness(10),
                Height = 35
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var btnOk = new Button
            {
                Content = "Aceptar",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(233, 69, 96)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.Bold
            };

            var btnCancel = new Button
            {
                Content = "Cancelar",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(22, 33, 62)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0)
            };

            btnOk.Click += (s, e) =>
            {
                YouTubeUrl = textBox.Text;
                DialogResult = true;
                Close();
            };

            btnCancel.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };

            buttonPanel.Children.Add(btnOk);
            buttonPanel.Children.Add(btnCancel);

            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);

            grid.Children.Add(stackPanel);
            Content = grid;
        }
    }
}