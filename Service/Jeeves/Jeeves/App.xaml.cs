using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Jeeves
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
           this.InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            new MainWindow().Show();
            base.OnStartup(e);
        }
    }
}