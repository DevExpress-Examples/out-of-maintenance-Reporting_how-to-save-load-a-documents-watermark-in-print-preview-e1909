#region Usings
using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Drawing;
#endregion

namespace SaveRestoreWatermark {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region Internal Fields
        private enum WatermarkStorage { Registry, XML, Stream };
        private string registryPath = @"HKEY_CURRENT_USER\Software\MyCompany\MyTool\";
        private string xmlFile = "test.xml";
        private MemoryStream stream = new MemoryStream();
        #endregion

        #region Prepare Example
        private void Form1_Load(object sender, EventArgs e) {
            this.productsTableAdapter.Fill(this.nwindDataSet.Products);

            radioGroup1.SelectedIndex = 0;

            Watermark wm = printableComponentLink1.PrintingSystem.Watermark;
            wm.Text = "Change the Watermark,\r\nthen close and re-open\r\nthe form.";
            wm.ShowBehind = false;
            wm.TextDirection = DirectionMode.Horizontal;

            SaveWatermark(printableComponentLink1, WatermarkStorage.Registry);
            SaveWatermark(printableComponentLink1, WatermarkStorage.Stream);
            SaveWatermark(printableComponentLink1, WatermarkStorage.XML);
        }
        #endregion

        #region Get Watermark Storage
        private WatermarkStorage GetStorage() {
            WatermarkStorage storage = WatermarkStorage.Registry;

            switch (radioGroup1.SelectedIndex) {
                case 0: {
                        storage = WatermarkStorage.Registry;
                        break;
                    }
                case 1: {
                        storage = WatermarkStorage.XML;
                        break;
                    }
                case 2: {
                        storage = WatermarkStorage.Stream;
                        break;
                    }
            }

            return storage;
        }
        #endregion

        private void btnShowPreview_Click(object sender, EventArgs e) {
            RestoreWatermark(printableComponentLink1, GetStorage());

            printableComponentLink1.CreateDocument();

            printableComponentLink1.PrintingSystem.PreviewFormEx.FormClosed +=
                new FormClosedEventHandler(PreviewFormEx_FormClosed);

            printableComponentLink1.ShowPreview();
        }

        void PreviewFormEx_FormClosed(object sender, FormClosedEventArgs e) {
            SaveWatermark(printableComponentLink1, GetStorage());
        }

        private void RestoreWatermark(PrintableComponentLink pcl, WatermarkStorage storage) {
            switch (storage) {
                case WatermarkStorage.Registry: {
                        pcl.PrintingSystem.Watermark.RestoreFromRegistry(registryPath);
                        break;
                    }
                case WatermarkStorage.XML: {
                        if (File.Exists(xmlFile)) {
                            pcl.PrintingSystem.Watermark.RestoreFromXml(xmlFile);
                        }
                        break;
                    }
                case WatermarkStorage.Stream: {
                        pcl.PrintingSystem.Watermark.RestoreFromStream(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        break;
                    }
            }
        }

        private void SaveWatermark(PrintableComponentLink pcl, WatermarkStorage storage) {
            switch (storage) {
                case WatermarkStorage.Registry: {
                        pcl.PrintingSystem.Watermark.SaveToRegistry(registryPath);
                        break;
                    }
                case WatermarkStorage.XML: {
                        pcl.PrintingSystem.Watermark.SaveToXml(xmlFile);
                        break;
                    }
                case WatermarkStorage.Stream: {
                        pcl.PrintingSystem.Watermark.SaveToStream(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        break;
                    }
            }
        }

    }
}