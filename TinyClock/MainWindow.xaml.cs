/*
  TinyKlok
  Copyright (C) 2013 Marko Devcic
   
  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation <http://www.gnu.org/licenses/>.
  
  This application comes without WARRANTY WHATSOEVER!
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace TinyKlok
{

    public partial class MainWindow : Window
    {
        #region Fields

        bool swVisible = false;
        System.Timers.Timer timerClock;
        System.Timers.Timer timerStopWatch;
        Stopwatch stopwatch; 

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            timerClock = new System.Timers.Timer(500);
            timerClock.AutoReset = true;
            timerClock.Elapsed += (s, e) => { UpdateTime(); };
            timerClock.Start();
            timerStopWatch = new System.Timers.Timer(100);
            timerStopWatch.Elapsed += (s, e) => { UpdateStopwatch(); };
            timerStopWatch.AutoReset = true;
            stopwatch = new Stopwatch();
            this.Loaded += (s, e) => { LoadSettings(); };
            (App.Current as App).SessionEnding += (s, e) => { SaveSettings(); };
            this.Closing += (s, e) => { SaveSettings(); };
        }
        
        #endregion

        #region Class Methods

        private void UpdateTime()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() => { lblClock.Content = string.Format("{0:HH:mm}", DateTime.Now); }));
        }

        private void UpdateStopwatch()
        {
            Dispatcher.BeginInvoke((ThreadStart)(() => { lblStopWatch.Content = string.Format("{0:D2}:{1:D2}:{2:D2}", stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds); }));
        }

        private void HideStopWatch()
        {
            FadeOut();
            swVisible = false;
        }

        private void ChangeTheme(Uri uri)
        {
            Application.Current.Resources.Source = uri;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Width = this.ActualWidth;
            Properties.Settings.Default.Height = this.ActualHeight;
            Properties.Settings.Default.Opacity = rootBackground.Opacity;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            this.Left = Properties.Settings.Default.Left;
            this.Top = Properties.Settings.Default.Top;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;
            rootBackground.Opacity = Properties.Settings.Default.Opacity;
            if (Properties.Settings.Default.Theme) ChangeTheme(new Uri("/Themes/Dark.xaml", UriKind.Relative));
            else ChangeTheme(new Uri("/Themes/Light.xaml", UriKind.Relative));
        }

        private void ShowStopWatch()
        {
            if (this.ActualHeight >= this.ActualWidth) FadeIn();
            else SlideDown();
            swVisible = true;
        }

        #endregion

        #region Animations
        private void FadeIn()
        {
            vbStopwatch.Visibility = System.Windows.Visibility.Visible;
            if (stopwatch.Elapsed == TimeSpan.Zero) lblStopWatch.Content = "click";
            lblStopWatch.Opacity = 0;
            DoubleAnimation fade = new DoubleAnimation(0.0d, 1.0d, new Duration(TimeSpan.FromSeconds(1.3d)));
            fade.SetValue(Storyboard.TargetProperty, lblStopWatch);
            Storyboard story = new Storyboard();
            Storyboard.SetTarget(fade, lblStopWatch);
            Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));
            story.Children.Add(fade);
            story.Begin(lblStopWatch);
        }

        private void FadeOut()
        {
            DoubleAnimation fade = new DoubleAnimation(1.0d, 0.0d, new Duration(TimeSpan.FromSeconds(1.3d)));
            fade.SetValue(Storyboard.TargetProperty, lblStopWatch);
            Storyboard story = new Storyboard();
            Storyboard.SetTarget(fade, lblStopWatch);
            Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));
            story.Children.Add(fade);
            story.Completed += (s, e) => { if (this.ActualHeight >= this.ActualWidth / 1.6666) SlideUp(); };
            story.Begin(lblStopWatch);
        }

        private void SlideUp()
        {
            DoubleAnimation slide = new DoubleAnimation(this.ActualHeight, this.ActualWidth / 1.6666d, new Duration(TimeSpan.FromSeconds(0.7)));
            slide.SetValue(Storyboard.TargetProperty, this);
            Storyboard story = new Storyboard();
            Storyboard.SetTarget(slide, this);
            Storyboard.SetTargetProperty(slide, new PropertyPath("Height"));
            story.Children.Add(slide);
            story.Begin(this);
        }

        private void SlideDown()
        {
            DoubleAnimation slide = new DoubleAnimation(this.ActualHeight, this.ActualWidth, new Duration(TimeSpan.FromSeconds(0.7)));
            slide.SetValue(Storyboard.TargetProperty, this);
            Storyboard story = new Storyboard();
            Storyboard.SetTarget(slide, this);
            Storyboard.SetTargetProperty(slide, new PropertyPath("Height"));
            story.Children.Add(slide);
            story.Completed += (s, e) =>
            {
                FadeIn();
            };

            story.Begin(this);
        } 

        #endregion

        #region Event Handlers

        private void lblStopWatch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                timerStopWatch.Enabled = false;
                stopwatch.Stop();
                lblStopWatch.Content = "resume";

            }
            else
            {
                timerStopWatch.Enabled = true;
                stopwatch.Start();

            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!swVisible) ShowStopWatch();
            else { HideStopWatch(); }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            popupSettings.IsOpen = true;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            popupSettings.IsOpen = false;
        }

        private void lblTheme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.Theme)
            {
                ChangeTheme(new Uri("/Themes/Light.xaml", UriKind.Relative));
                lblTheme.Content = "Light";
                Properties.Settings.Default.Save();
            }
            else
            {
                ChangeTheme(new Uri("/Themes/Dark.xaml", UriKind.Relative));
                lblTheme.Content = "Dark";
                Properties.Settings.Default.Save();
            }

        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            (App.Current as App).Shutdown();
        }

        #endregion

    }
}
