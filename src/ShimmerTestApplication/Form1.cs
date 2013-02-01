using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shimmer.Client;
using System.Reactive.Subjects;

namespace ShimmerTestApplication
{
    public partial class Form1 : Form
    {
        UpdateManager _updateManager;

        public Form1()
        {
            InitializeComponent();
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_updateManager != null) _updateManager.Dispose();
            base.OnClosing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_updateManager != null) _updateManager.Dispose();

            _updateManager = new UpdateManager(
                //"https://raw.github.com/belfryimages/shimmer-test-winforms-client/master/update-path/",
                @"..\..\..\..\update-path\",
                "ShimmerTestApplication",
                FrameworkVersion.Net40);

            _updateManager.CheckForUpdate().Subscribe(updateInfo =>
            {
                if (updateInfo == null)
                {
                    MessageBox.Show("No updates found");
                    return;
                }
                MessageBox.Show(string.Format("Found {0} releases to apply", updateInfo.ReleasesToApply.Count()));
                foreach (var r in updateInfo.ReleasesToApply)
                {
                    MessageBox.Show(string.Format(
                        "{0} {1}",
                        r.PackageName,
                        r.Version.ToString()));
                }
            });
        }
    }
}
