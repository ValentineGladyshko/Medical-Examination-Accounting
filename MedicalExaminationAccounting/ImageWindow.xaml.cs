using MedicalExaminationAccounting.Model.Repositories;
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
using System.Windows.Shapes;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting
{
    public partial class ImageWindow : Window
    {
        public ExaminationData Data { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");
        public ImageWindow()
        {
            InitializeComponent();

            Data = new ExaminationData();

            DataContext = Data;

            SetHandlers();
        }

        public ImageWindow(int id)
        {
            InitializeComponent();

            Data = unitOfWork.ExaminationDatas.Get(id);

            DataContext = Data;

            SetHandlers();
        }

        public void SetHandlers()
        {
            MinusButton.Click += (object sender, RoutedEventArgs args) =>
            {
                int num = (int)Slider.Value;
                if (num <= 25)
                {
                    Slider.Value = 10;
                    return;
                }

                if (num <= 50)
                {
                    Slider.Value = 25;
                    return;
                }

                if (num <= 75)
                {
                    Slider.Value = 50;
                    return;
                }

                if (num <= 100)
                {
                    Slider.Value = 75;
                    return;
                }

                int f = num % 100;
                if (f == 0)
                {
                    Slider.Value = num - 100;
                }
                else
                {
                    Slider.Value = num - f;
                }
            };

            PlusButton.Click += (object sender, RoutedEventArgs args) =>
            {
                int num = (int)Slider.Value;
                if (num < 25)
                {
                    Slider.Value = 25;
                    return;
                }

                if (num < 50)
                {
                    Slider.Value = 50;
                    return;
                }

                if (num < 75)
                {
                    Slider.Value = 75;
                    return;
                }

                if (num < 100)
                {
                    Slider.Value = 100;
                    return;
                }
                int f = num % 100;
                if (f == 0)
                {
                    Slider.Value = num + 100;
                }
                else
                {
                    Slider.Value = num + 100 - f;
                }
            };

            Slider.ValueChanged += (object sender, RoutedPropertyChangedEventArgs<double> args) =>
            {
                int num = (int)Slider.Value;
                ZoomBox.Text = num + "%";
                ScaleTransform scale = new ScaleTransform();
                scale.ScaleX = Slider.Value / 100.0;
                scale.ScaleY = Slider.Value / 100.0;
                ImageView.LayoutTransform = scale;

            };
        }
    }
}
